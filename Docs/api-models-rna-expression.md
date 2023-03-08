# Gene Expressions Transcriptomics) Data Models

## Transcriptomics Data
Includes information about the analysis, sample and expression data.

**`Analysis`** - Sequencing analysis data.
- Type: _Object([Analysis](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-rna-expression.md#analysis))_
- Example: `{...}`

**`Sample`*** - Which sample was analysed.
- Type: _Object([Sample](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-rna-expression.md#sample))_
- Element type: 
- Example: `{...}`

**`Expressions`*** - Gene expressions found in the sample during the analysis.
- Type: _Array_
- Element type: _Object([Expression](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-rna-expression.md#expression))_
- Limitations: If set, should contain at leas one element
- Example: `[{...}, {...}]`

## Analysis
Sequencing analysis data.

**`Type`*** - Analysis type.
- Type: _String_
- Possible values: `"RNA-Seq"`
- Example: `"RNA-Seq"`

#### Analysis Type
Analysis can be of the following types:
- `"RNA-Seq"` - RNA sequencing

## Sample
Analysed sample data.

**`Id`*** - Sample identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"SA5"`

**`DonorId`*** - Sample donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"DO1"`

**`SpecimenId`*** - Identifier of the specimen the sample was created from.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"TI1"`

**`SpecimenType`*** - Type of the specimen the sample was created from.
- Type: _String_
- Possible values: `"Tissue"`, `"CellLine"`, `"Organoid"`, `"Xenograft"`
- Example: `"Tissue"`

## Expression
Gene expression data.

The data can be submitted by only single of the following strategies:
- `GeneId` - fastest
- `GeneSymbol` - slower than by `GeneId`
- `TranscriptId` - significantly slower than by `GeneId` or `GeneSymbol`
- `TranscriptSymbol` - slower than by `TranscriptId`

**`GeneId`** - Gene identifier. 
- Type: _String_
- Limitations: Maximum length 100
- Example: `"ENSG00000223972"`

**`GeneSymbol`** - Gene symbol.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"DDX11L1"`

**`TranscriptId`** - Transcript identifier.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"ENST00000456328"`

**`TranscriptSymbol`** - Transcript symbol.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"DDX11L1-002"`

**`Source`** - Source feature identifier or symbol.
- Note: If source is `"Ensembl"`, then this field can be ignored to reduce submission data size.
- Type: _String_
- Possible values: `"Ensembl"`
- Default value: `"Ensembl"`
- Example: `"Ensembl"`

**`ExonicLength`** - Exonic length of the feature.
- Note: If not set, API will caclulate exonic length of the feature (Transcript or gene canonical transcript exonic length).
- Type: _Integer_
- Limitations: Should be greater than 0
- Example: `1657`

**`Reads`*** - Number of reads for the feature.
- Type: _Integer_
- Limitations: Should be greater than or equal to 0
- Example: `238`

##
**`*`** - Required fields
