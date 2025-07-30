# Bulk Gene Expressions
The model is used to upload the data of bulk gene expressions calling result file and the metadata of the analysis.

> [!Note]
> All exact dates are hidden and protected. Relative dates are shown instead, if calculation was possible

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

**`entries`*** - file with the expressions data.
- Type: _File_
- Supported formats: [tsv](#tsv), [vcf](#dkfzrnaseq)
- Limitations: Should be set, should contain at least one element
- Example: `expressions.tsv`

**`*`** - Required fields

**Analysis Types**
- `RNASeq` - Bulk RNA Sequencing


## Formats
Several formats are supported for bulk gene expressions data file.

> [!Note]
> If the **exonic length** is not provided, the Portal will calculate it based on the Ensembl data of corresponding gene or transcript.  
> Normalised read counts, such as **FPKM**, **TPM** will be calculated by the Portal, no external values can be provided.  
> System supports only [Ensembl](https://www.ensembl.org/index.html) gene and transcript [identifiers](https://www.ensembl.org/info/omics/stable_ids/index.html). If such are unavailable, provide gene or transcript symbols.

The data can be submitted by only one of the following strategies (one of this fields should be set):
- `Gene ID` - fastest
- `Gene symbol` - slower than by `Gene ID`
- `Transcript ID` - significantly slower than by `Gene ID` or `Gene symbol`
- `Transcript symbol` - slower than by `Transcript ID`

### TSV
Default UNITE for bulk gene expressions data file.  
It's a tab-separated values (TSV) file with the following columns:

**`gene_id`** - Gene identifier. 
- Type: _String_
- Limitations: Maximum length 100
- Example: `"ENSG00000223972"`

**`gene_symbol`** - Gene symbol.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"DDX11L1"`

**`transcript_id`** - Transcript identifier.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"ENST00000456328"`

**`transcript_symbol`** - Transcript symbol.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"DDX11L1-002"`

**`exonic_length`** - Exonic length of the feature.
- Note: If not set, API will caclulate exonic length of the feature (Transcript or gene canonical transcript exonic length).
- Type: _Integer_
- Limitations: Should be greater than 0
- Example: `1657`

**`reads`*** - Number of reads for the feature (raw reads count).
- Type: _Integer_
- Limitations: Should be greater than or equal to 0
- Example: `238`

**`*`** - Required fields

#### Example
`expressions.tsv`
```tsv
gene_id	reads
ENSG00000223972	238
ENSG00000243485	0
ENSG00000274890	0
```

### DKFZ RNASeq
[DKFZ RNASeq](https://github.com/DKFZ-ODCF/RNAseqWorkflow) is a TSV file format used by the DKFZ RNASeq pipeline for bulk gene expressions calling.