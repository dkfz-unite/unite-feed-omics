# Single Cell RNA Sample
The model is used to upload the data of single cell RNA sample metadata and files.

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
- Possible values: `scRNASeq`, `snRNASeq`
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

**`genome`*** - Reference genome.
- Type: _String_
- Possible values: `GRCh37`, `GRCh38`
- Example: `GRCh38`

**`resources`*** - file with the sample resources metadata.
- Type: _File_
- Supported formats: [tsv](#resources)
- Limitations: Should be set, should contain at least one element
- Example: `resources.tsv`

**`*`** - Required fields

**Analysis Types**
- `scRNASeq` - Single Cell RNA Sequencing
- `snRNASeq` - Single Nucleus RNA Sequencing


## Resources
The resources file contains resource file metadata required for remote file access and processing.  
It's a tab-separated values (TSV) file with the following columns:

**`name`*** - Resource file name (with extension).
- Type: _String_
- Limitations: Maximum length 255
- Example: `tumor.bam`

**`format`*** - Resource format.
- Type: _String_
- Available values: `fasta`, `fastq`, `bam`, `bam.bai`, `bam.bai.md5`
- Example: `"bam"`

**`url`*** - Resource URL on remote a server.
- Type: _String_
- Example: `https://example.com/file/abcd101`

**`*`** - Required fields

> [!Note]
> Do not compress `bam` files. Portal may try to access the data sending range queries, which is not possible if the data is compressed.

### Example
```tsv
name	format	url
tumor.bam	bam	https://example.com/file/abcd101
tumor.bam.bai	bam.bai	https://example.com/file/abcd102
tumor.bam.bai.md5	bam.bai.md5	https://example.com/file/abcd103
```