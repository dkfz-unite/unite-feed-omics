# Bulk Transcriptomics Data Models

## Sequencing Data
Includes information about analysed sample and expression data.

**`tsample`*** - Target sample data. 
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`resources`** - Analysis result resource files.
- Type: _Array_
- Element type: _Object([Resource](api-models-resource.md))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

**`entries`*** - Gene expressions found in the sample during the analysis.
- Type: _Array_
- Element type: _Object([Expression](api-models-rna-expression.md#expression))_
- Limitations: If set, should contain at leas one element
- Example: `[{...}, {...}]`

## Expression
Gene expression data.

> [!Note]
> System supports only [Ensembl](https://www.ensembl.org/index.html) gene and transcript [identifiers](https://www.ensembl.org/info/genome/stable_ids/index.html). If such are unavailable, provide gene or transcript symbols.

The data can be submitted by only one of the following strategies (one of this fields should be set):
- `gene_id` - fastest
- `gene_symbol` - slower than by `gene_id`
- `transcript_id` - significantly slower than by `gene_id` or `gene_symbol`
- `transcript_symbol` - slower than by `transcript_id`

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

**`reads`*** - Number of reads for the feature.
- Type: _Integer_
- Limitations: Should be greater than or equal to 0
- Example: `238`

##
**`*`** - Required fields
