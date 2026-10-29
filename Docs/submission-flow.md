# Submission flow

Forms and JSON use the same submission model, model validator and staging path:

```text
multipart form → read files → form converter ─┐
                                              ├→ model validator → Mongo + submission task
JSON body → deserialize submission model ─────┘
```

Binding and file-reading errors are retained alongside model validation errors.
Any error returns 400 before staging. The converter builds a C# model object;
there is no intermediate JSON serialization/deserialization for form requests.

## Controller configuration

Concrete endpoints inherit directly from
`SubmissionController<TModel, TForm, TEntry>`. They configure `ModelValidator`,
`FormConverter`, `EntryReaders`, `ResourceReader`, `DataType` and their submission
repository/task type. Both POST actions call the same model validation and
staging method. Controllers contain no metadata, entry or scope validation rules.

The first configured entry reader is the default when `format` is omitted.
An explicitly unsupported `format` produces a file-format error; it does not
fall back to the default. Endpoints without entry readers ignore supplied entry
files, as with sample metadata and resource-only analysis submissions.

## Model validators

| Submission kind | Validator | Contents |
| --- | --- | --- |
| Sample metadata | `SampleModelValidator` | Metadata, allowed analysis types, optional resources |
| Analysis data | `AnalysisDataModelValidator<TEntry>` | Target/matched samples, required entries, entry validator, optional resources |
| Analysis resources | `AnalysisResourcesModelValidator` | Target/matched samples, required resources |

The two analysis validators inherit shared sample/resource rules from
`AnalysisModelValidator<TModel>`. Sample rules are reused through
`SampleModelValidator`. Entry/resource collections validate individual models
with `SetValidator` and reject null items.

For endpoint-specific rules, extend the corresponding model validator and select
it in the controller. SM's `Models/Dna/Sm/Validators/AnalysisModelValidator`
extends the data validator: it configures WGS/WES, allows SNV/INS/DEL/MNV scope,
and checks entry types against that scope. Omitted or empty `EntriesScope` still means
replacement of all SM types. Other endpoints reject nonempty scope unless their
model validator explicitly supports it.

Errors use model property paths, such as `TargetSample.SpecimenType` and
`Entries[2].Reads`, for both JSON and form submissions. There is no mapping back
to flat form field names.

## Form conversion and reading

Converters copy metadata and parsed files into the submission model before
validation. Invalid enum text converts to null; validators reject required
missing values. A matched sample is created when either identifying field is
provided, so partial metadata is validated rather than discarded. Binding errors
for dates/numbers remain in ModelState even when the converted value is null.

`IReader<T>.Read` returns an array. TSV reading delegates to
`Unite.Essentials.Tsv.TsvReader.Read`; format-specific readers retain their
existing parsing/conversion behavior. Read failures (including null results
from existing readers) add one general error under `Entries` or `Resources`:
"File could not be read in '<format>' format". Readers do not collect per-cell
validation failures. Successfully read models are validated afterwards.

Sample resources are optional. Analysis data requires a nonempty entries
collection, and analysis resources requires a nonempty resources collection.
Resources may be submitted independently; a complete IDAT pair or scRNA matrix
bundle is not required in one request. Resource types are assigned from the
endpoint before staging.

## Typed data payloads and writers

The Web model is converted into a typed data-layer `AnalysisModel<TEntry>` with
`TargetSample`, optional `MatchedSample`, analysis `Resources`, and `Entries`.
`SampleModel` contains only sample/assay metadata and sample resources.
Web sample models and forms use `AssayType`, `AssayDate` and `AssayDay`; the
JSON and multipart field names remain `analysis_type`, `analysis_date` and
`analysis_day`. Existing Mongo staging field names are also retained.
`AssayModel` holds assay type, date/day and parameters; it is separate
from the result-submission payload.

The SM data payload extends `AnalysisModel<Sm.VariantModel>` with
`SmType[] EntriesScope`. Its Web counterpart uses an optional string array named
`EntriesScope` (JSON/form field `entries_scope`). Both properties belong to the
Web `AnalysisModel<TEntryModel>`; the non-generic `AnalysisModel` contains only
common sample/resource metadata. Other analysis validators reject nonempty
entries scope by default.

Forms accept one comma-separated `entries_scope` value (`SNV, INS, DEL`);
conversion trims values and removes empty items. JSON uses an array of strings.

| Entries | Data writer |
| --- | --- |
| SM variants | `Writers.Dna.AnalysisSmWriter` |
| CNV variants | `Writers.Dna.AnalysisCnvWriter` |
| SV variants | `Writers.Dna.AnalysisSvWriter` |
| CNV profiles | `Writers.Dna.AnalysisCnvpWriter` |
| Gene expressions | `Writers.Rna.AnalysisWriter` |
| Protein expressions | `Writers.Prot.AnalysisWriter` |
| Analysis resources | `Writers.RnaSc.AnalysisWriter` (also used for methylation levels) |

These writers share `AnalysisWriter<TModel, TAudit>`, which writes the target and
matched-sample association, invokes `WriteEntries`, and writes sample/analysis
resources in the existing `DataWriter` transaction. Repositories are initialized
against that transaction's DbContext before processing. Constructors do not
initialize result repositories.

A supplied matched sample is found by donor, specimen ID/type and assay type,
or created when missing. Its link is set on new and existing target samples;
omitting the matched sample preserves the existing link. Reusing a matched
sample does not update its metadata. Forms inherit only the target's assay
type for the matched assay, leaving its date/day unset. Submit the matched
sample separately to provide or update its metadata.

Concrete writers initialize their own entry repositories and override
`WriteEntries`. Replacement, batching and audit rules remain local to the result
kind. Empty converted entries preserve existing data; API data submissions still
require entries before staging. RNA/protein handlers annotate their submitted
expressions before assigning the typed data payload's `Entries`.

The Web converter is generic over source entry and target analysis model types.
DNA converters populate `target.Entries`; SM additionally converts
`source.EntriesScope` into its typed enum array. Resource-only conversions need
only the common sample/resource mapping.

DNA analysis writers share `AnalysisWriteAudit`. `Variants` contains new variant
IDs requiring annotation; `Entries` contains affected variant entity IDs (or
CNVP profile IDs), including removed IDs, for reindexing. The common counters
are `VariantsCreated` and `EntriesCreated`. Keep the annotation and indexing
sets distinct when updating writers or submission handlers.

Variant task population schedules affected genes and proteins alongside the
variant and related project/donor/specimen tasks, including after annotation.
