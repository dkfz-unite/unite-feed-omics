# UNITE repository instructions

Before changing this repository, read the shared UNITE instructions and
relevant project documentation maintained in
[dkfz-unite/.github](https://github.com/dkfz-unite/.github/blob/main/README.md).

1. Prefer the local shared checkout: `../unite-meta/AGENTS.md`, relative to this
   repository root. A sibling `.github/AGENTS.md` or a user-provided path is also
   valid. Resolve its document links relative to that shared checkout.
2. Read the shared `AGENTS.md`, project overview, repository map, and topic
   documents relevant to the task, following the shared README's task index.
3. If no shared checkout is accessible, use the GitHub versions beginning with
   [shared AGENTS.md](https://github.com/dkfz-unite/.github/blob/main/AGENTS.md).
   If those are unavailable too, report the missing context and proceed only
   with what local documentation/code establishes; do not invent shared rules.
4. Read this repository's README and relevant local docs; inspect Git status.
   More specific instructions here supplement the shared guidance.

Keep common rules in `.github`; keep component details in this repository.

## Omics feed

Status: reviewed with the owner (2026-10-07/08). Defects found during the
review are tracked in Jira (UNITE-DEV, epic "Known Issues"); see the shared
[Track defects](../unite-meta/AGENTS.md#track-defects).

The omics feed ingests digital samples, their analysis results and resource
links, writes them to the domain database, annotates variants, and builds the
omics indexes. It is the most complex feed: modalities differ in models,
formats, write semantics and indexing. Shared background:
[Data ingestion](../unite-meta/docs/data-ingestion.md),
[Processing pipeline](../unite-meta/docs/processing-pipeline.md) and
[Domain model](../unite-meta/docs/domain.md#digital-samples-and-results).

### Projects

| Project | Contents |
| --- | --- |
| `Unite.Omics.Feed.Web` | API controllers, submission models/readers/validators, Mongo staging (`Submissions/`), workers and handlers |
| `Unite.Omics.Feed` | SQL writers and repositories per modality (`Data/Writers`, `Data/Repositories`) |
| `Unite.Omics.Annotations` | Ensembl VEP and Ensembl Data clients; variant, RNA and protein annotation services |
| `Unite.Omics.Indices` | Index entity builders and indexing caches for genes, proteins, SM/CNV/SV and CNV profiles |

There are no test projects. `Clients/Uniprot` in Annotations is currently
unused but kept on purpose, for possible future protein annotation; do not
remove it as dead code.

### Build and run

```sh
dotnet build Unite.Omics.Feed.sln
dotnet run --project Unite.Omics.Feed.Web
```

- Local URL `http://localhost:5106`; Docker `omics.feed.unite.net`, published on
  `127.0.0.1:5106`. Needs PostgreSQL, MongoDB, Elasticsearch, Ensembl Data and
  Ensembl VEP.
- NuGet restore needs the `https://nuget.pkg.github.com/dkfz-unite/index.json`
  source with GitHub credentials in the developer's NuGet configuration; the
  Dockerfile adds it from build arguments.
- Docker: `applications/unite-feed-omics/` in `unite-env`.

### Configuration and API

Environment variables, Docker setup and access ports are in the
[README](README.md); endpoints, request formats, data models and input formats
(`format=` readers) are in [Docs/api.md](Docs/api.md) and the `Docs/models-*.md`
files. Read those first. Developer-specific notes:

- `UNITE_GENOME_BUILD` is the instance's build (Ensembl VEP/Data builds; used by
  the writers and repositories, e.g. for chromosome-arm detection). Each sample
  declares its own `genome`, validated only as `GRCh37`/`GRCh38` and not yet
  compared with the instance build. Rules for which data must match are in the
  [Domain model](../unite-meta/docs/domain.md#genome-build).
- `Docs/api.md` documents the public submission endpoints. It deliberately
  omits `api/indexing/*` (administrator-only reindexing) and
  `api/meth/analysis/levels` (not used yet; Data Source currently sends
  methylation samples such as IDAT files). The portal's omics upload functions
  in `unite` use outdated plural routes but have no callers.

Controller → submission task type → handler, as needed when tracing a
submission (`Controllers/<Modality>/`, `Handlers/Submission/`):

| Route | Task type | Notes |
| --- | --- | --- |
| `api/dna/sample` | `DNA` | |
| `api/dna/analysis/sm` | `DNA_SM` | Annotation through VEP before indexing |
| `api/dna/analysis/cnv` | `DNA_CNV` | Annotation through VEP before indexing |
| `api/dna/analysis/sv` | `DNA_SV` | Annotation through VEP before indexing |
| `api/dna/analysis/cnvp` | `DNA_CNVP` | Also accepts MethArray samples |
| `api/meth/sample`, `api/meth/analysis/levels` | `METH`, `METH_LVL` | Resources only (loose integration); levels not used yet |
| `api/rna/sample`, `api/rna/analysis/exp` | `RNA`, `RNA_EXP` | Enriched through Ensembl Data during submission |
| `api/rnasc/sample`, `api/rnasc/analysis/exp` | `RNASC`, `RNASC_EXP` | Resources only (loose integration) |
| `api/prot/sample`, `api/prot/analysis/exp` | `PROT`, `PROT_EXP` | Sample resources must be `mzml` (checked in the controller); sample endpoint on `feature/proteomics-sample` |
| `api/indexing/genes`, `proteins`, `variants` | – | Delete the index and recreate indexing tasks |

The base `SubmissionController` stages payloads in Mongo database
`submissions` (one collection per data type) and creates the SQL task;
`review=false` creates it as `Prepared`.

### Submission validation

See [Submission flow](Docs/submission-flow.md) for controller configuration,
validator inheritance, conversion, reader behavior and JSON/form examples in
[API](Docs/api.md#json-submissions).

- Both multipart forms and JSON are supported. Forms are read/converted into
  the same models as JSON, then both paths use the endpoint's `ModelValidator`
  and the same staging method. Do not reintroduce form validators or domain
  validation in controller hooks.
- All endpoints inherit directly from the generic `SubmissionController`.
  Existing model types and serialization attributes are retained for Mongo
  staging and GET responses as well as JSON uploads.
- File-reader failures and unsupported formats are transport errors added by
  the controller. Data validation belongs in model validators, including SM
  scope membership. Readers keep their existing format implementations; do not
  duplicate TSV parsing or collect per-cell parsing failures.
- Converters run before validation. Keep nullable enum results and partially
  supplied matched samples so validators can report errors. Binding errors
  must survive conversion. Preserve every metadata field, including `batch`.
- Resource `type` is assigned from the endpoint (`DataType`) before staging.
  Resource `name`, `format` and `url` are lowercased by `ResourceModel`, following
  the shared [coding conventions](../unite-meta/AGENTS.md#coding-conventions).


### Processing

Workers: `SubmissionsWorker` (one handler per task type),
`VariantsAnnotationWorker` (SM/CNV/SV through VEP) and `IndexingWorker` (genes,
proteins, SM, CNV, SV, CNV profiles; gene and protein expression documents are
built by the gene and protein indexing). Routes, gating and retry behavior are
in [Processing pipeline](../unite-meta/docs/processing-pipeline.md); repeated
upload semantics per result kind are in the
[Domain model](../unite-meta/docs/domain.md#intentional-constraints).

Writers create missing donors and specimens: a new donor is assigned to the
default project `Other` (`Project.DefaultName`), created as public if missing,
and the specimen type comes from the sample model. The same applies in the
donors, images and specimens feeds. Whether submissions of other feeds should
wait for pending donor or specimen submissions, so that donors are created with
their real projects first, is under consideration by the owner; do not change
the ordering without agreement.

Nonempty result submissions replace CNV, SV, CNV profiles, gene expression and
protein expression per sample. SM submissions replace their declared
`Scope` (`SNV`, `INS`, `DEL`, `MNV`), or all SM types when scope is omitted,
null or empty; the API contract is in
[SM submissions](Docs/models-dna-sm.md). Empty entries remain invalid, and
data endpoints require entries. The shared analysis form keeps scope as an
optional string array. The SM submission model validator configures supported mutation definitions
and checks entry types against the declared scope. Other endpoints reject nonempty scope unless explicitly
configured to support it. Removal
methods return deleted association entity IDs (profile IDs for CNV profiles).
Writers union these with incoming IDs in the audit, and handlers schedule tasks
once after writing. Keep new variant IDs in the annotation sets and affected
association variant IDs in the indexing sets. Existing result task services
populate related donor/specimen and other indexing tasks from the audited IDs.
CNV-profile task creation must retain IDs of deleted SQL rows for index cleanup.

The sample `genome` should be checked against `UNITE_GENOME_BUILD` for
position-based data; the existing issue remains tracked in Jira.

### Where to change things

Use `Cnvp`/`Cnvps` for repository-owned CNV profile identifiers, and keep
`Dna.Cnv.Profile`/`ProfileModel` within the CNV namespace. Shared-library API
names such as `CnvProfileIndex` retain their upstream names until those libraries
are updated separately.

| Change | Location |
| --- | --- |
| New input format for a result kind | `Models/<Modality>/<Kind>/Readers/<Format>/` (implement `IReader<T>`), register in the controller's `Readers` |
| Validation of submitted entries | `Models/<Modality>/.../Validators`, base rules in `Models/Base/Validators` |
| How data is written / merged | `Unite.Omics.Feed/Data/Writers/<Modality>/AnalysisWriter.cs` and repositories |
| Annotation | `Unite.Omics.Annotations/Services` and `Handlers/Annotation` |
| Index document shape | `unite-indices` models, then `Unite.Omics.Indices/Services/*IndexEntityBuilder.cs` |
| New result kind end to end | Entity/mapper and migration (`unite-data`, `unite-data-migrations`), task type, controller, submission repository and handler, writer, index builder and indexing handler, portal |

### Shared libraries

All projects pin `Unite.Data.Context` 12.10.33 (bumped from 12.10.3 on `fix/default-project-public`),
`Unite.Indices.Context` 13.10.0 (latest 13.10.20) and `Unite.Cache` 1.2.3.
Check whether a model change in `unite-data`/`unite-indices` needs a package
bump here. `Clients/JsonHttpClient.cs` duplicates the copy in Composer.

### CI

`docker-images.yml` builds amd64 and arm64 images under one manifest on pushes
to `main` (the older `docker-image.yml` and `docker-image1.yml` were removed on
2026-10-07).
