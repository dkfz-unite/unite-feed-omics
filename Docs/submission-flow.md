# Submission flow

Forms and JSON use the same submission model, model validator and staging path:

```text
multipart form → read files → form converter ─┐
                                            ├→ model validator → Mongo + submission task
JSON body → deserialize submission model ────┘
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
| Sample metadata | `SampleModelValidator` | Metadata, allowed analysis types/scope, optional resources |
| Analysis data | `AnalysisDataModelValidator<TEntry>` | Target/matched samples, required entries, entry validator, optional resources |
| Analysis resources | `AnalysisResourcesModelValidator` | Target/matched samples, required resources |

The two analysis validators inherit shared sample/resource rules from
`AnalysisModelValidator<TModel>`. Sample rules are reused through
`SampleModelValidator`. Entry/resource collections validate individual models
with `SetValidator` and reject null items.

For endpoint-specific rules, extend the corresponding model validator and select
it in the controller. SM's `Models/Dna/Sm/Validators/AnalysisModelValidator`
extends the data validator: it configures WGS/WES, allows SNV/INS/DEL/MNV scope,
and checks entry types against that scope. Omitted or empty scope still means
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
