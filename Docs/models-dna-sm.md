# Simple Mutations (SM)
The model is used to upload the data of simple mutations (SM) calling result file and the metadata of the analysis.

> [!Note]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`donor_id`*** - Sample donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `Donor1`

**`specimen_id`*** - Identifier of the specimen the sample was created from.
- Type: _String_
- Limitations: Maximum length 255
- Example: `Tumor`

**`specimen_type`*** - Type of the specimen the sample was created from.
- Type: _String_
- Possible values: `Material`, `Line`, `Organoid`, `Xenograft`
- Example: `Material`

**`matched_specimen_id`** - Identifier of the matched specimen the sample was created from.
- Type: _String_
- Limitations: Maximum length 255
- Example: `Normal`

**`matched_specimen_type`** - Type of the matched specimen the sample was created from.
- Type: _String_
- Possible values: `Material`, `Line`, `Organoid`, `Xenograft`
- Example: `Material`

**`analysis_type`*** - Type of the analysis performed on the sample.
- Type: _String_
- Possible values: `WGS`, `WES`
- Example: `WES`

**`analysis_date`** - Date when the sample was analysed.
- Type: _Date_
- Limitations: Either 'analysis_date' or 'analysis_day' should be set.
- Format: "YYYY-MM-DD"
- Example: `2023-12-01`

**`analysis_day`** - Relative number of days since donor enrollment when the sample was analysed.
- Type: _Integer_
- Limitations: Integet, greater than or equal to 1, either 'date' or 'day' should be set.
- Example: `22`

**`genome`*** - Reference genome.
- Type: _String_
- Possible values: `GRCh37`, `GRCh38`
- Example: `GRCh37`

**`scope`** - Mutation types completely covered by this upload. Optional; omitted, null or empty scope covers all SM types.
- Type: _Array of strings_
- Possible values: `SNV`, `INS`, `DEL`, `MNV`
- Example: `["SNV"]` for an SNV caller; `["INS", "DEL"]` for an INDEL caller.
- For JSON requests, set `scope` inside the target sample: `"tsample": { ..., "scope": ["INS", "DEL"] }`.
- For multipart forms, repeat the field: `scope=INS` and `scope=DEL`.
- Every entry must belong to a declared type; otherwise the entire request is rejected.
- Existing associations for the target sample and declared types are replaced. Other mutation types and other samples are preserved.
- A declared type with no entries in a nonempty upload is cleared. For example, `["SNV", "INS", "DEL"]` with only SNV entries also clears previous insertions and deletions.
- Scope is submission metadata, not a separate stored analysis or result set. It is not inferred from the entries or previous uploads.
- Submitting entries without scope replaces all SM associations for the target sample, including for existing clients and staged submissions. Separate callers should send an explicit scope to preserve other mutation types.

**`entries`*** - file with the variants data.
- Type: _File_
- Supported formats: [tsv](#tsv), [vcf](#vcf)
- Limitations: Entries or resources must be supplied. An entries file must contain at least one element; empty uploads are not used to delete results.
- Example: `variants.tsv`

**`*`** - Required fields

**Analysis Types**
- `WGS` - Whole Genome Sequencing
- `WES` - Whole Exome Sequencing


## Formats
Several formats are supported for the simple mutations data file.

### TSV
Default UNITE format for simple mutations data file.  
It's a tab-separated values (TSV) file with the following columns:

**`chromosome`*** - Chromosome.
- Type: _String_
- Possible values: `1`, ..., `22`, `X`, `Y`
- Example: `"7"`

**`position`*** - Position.
- Type: _String_
- Format: Integer number "[number]" or range "[number]-[number]"
- Example (_number_): `141365018`
- Example (_range_): `141365018-141365019`

**`ref`** - Reference base.
- Type: _String_
- Limitations: Should be set if 'Alt' is empty, can contain only 'A', 'C', 'G' or 'T' characters
- Example (SNV): `C`
- Example (MNV): `CTAGTTGA`
- Example (null): `null` - e.g. in case of insertions

**`alt`** - Alternate base.
- Type: _String_
- Limitations: Should be set if 'Ref' is empty, can contain only 'A', 'C', 'G' or 'T' characters
- Example (SNV): `G`
- Example (MNV): `GTACCTGA`
- Example (null): `null` - e.g. in case of deletions

**`*`** - Required fields

#### Example
`variants.tsv`
```tsv
chromosome	position	ref	alt
7	141365018	C	G
7    141365025	A	
7    141365032        C
```

> [!Note]
> Information should follow [HGVS](https://hgvs-nomenclature.org/stable/) nomenclature.


### VCF
[Variant Call Format](https://samtools.github.io/hts-specs/VCFv4.2.pdf) (VCF) is a text file format for storing gene sequence variations.  
Columns other than `CHROM`, `POS`, `REF` and `ALT` are ignored.
