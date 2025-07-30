# Single Cell Gene Expressions
The model is used to upload the data of single cell gene expressions calling result files and the metadata of the analysis.

> [!Note]
> All exact dates are hidden and protected. Relative dates are shown instead, if calculation was possible.

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

**`analysis_type`*** - Type of the analysis performed on the sample.
- Type: _String_
- Possible values: `RNASeq`
- Example: `RNASeq`

**`analysis_date`** - Date when the sample was analysed.
- Type: _Date_
- Limitations: Either 'analysis_date' or 'analysis_day' should be set.
- Format: "YYYY-MM-DD"
- Example: `2023-12-01`

**`analysis_day`** - Relative number of days since donor enrollment when the sample was analysed.
- Type: _Integer_
- Limitations: Integet, greater than or equal to 1, either 'date' or 'day' should be set.
- Example: `22`

**`genome`** - Reference genome.
- Type: _String_
- Possible values: `GRCh37`, `GRCh38`
- Limitations: Maximum length 100
- Example: `GRCh37`

**`resources`*** - file with cell expressions files metadata.
- Type: _File_
- Supported formats: [tsv](#resources)
- Limitations: Should be set, should contain at least one element
- Example: `resources.tsv`

**`*`** - Required fields

**Analysis Types**
- `scRNASeq` - Single Cell RNA Sequencing
- `snRNASeq` - Single Nucleus RNA Sequencing


## Resources
The resources file contains result files metadata required for remote access and processing.  
It's a tab-separated values (TSV) file with the following columns:

**`name`*** - Resource file name (with extension).
- Type: _String_
- Limitations: Maximum length 255
- Example: `barcodes.mtx.gz`

**`format`*** - Resource format.
- Type: _String_
- Available values: `tsv`, `mtx`
- Example: `"idat"`

**`url`*** - Resource URL on remote a server.
- Type: _String_
- Example: `https://example.com/file/abcd101`

**`*`** - Required fields

> [!Note]
> Uploading single cell RNA sequencing data requires 3 files: `barcodes.tsv.gz`, `features.tsv.gz` and `matrix.tsv.gz`.  
> Do not change the file names, and do not decompress them, otherwise the Portal won't be able to process them correctly.

### Example
```tsv
name	format	url
barcodes.tsv.gz	tsv	https://example.com/file/abcd101
features.tsv.gz	tsv	https://example.com/file/abcd102
matrix.mtx.gz	mtx	https://example.com/file/abcd103
```