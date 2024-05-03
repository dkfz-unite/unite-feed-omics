# Single Cell Transcriptomics Data Models

## Sequencing Data
Includes information about the analysis, sample, expression data and resources.

**`analysis`*** - Sequencing analysis data.
- Type: _Object([Analysis](api-models-analysis.md))_
- Example: `{...}`

**`target_sample`*** - Target sample data. 
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`resources`*** - resource files with analysis results.
- Type: _Array_
- Element type: _Object([Resource](api-models-resource.md))_
- Limitations: If set, should contain at leas one element
- Example: `[{...}, {...}]`

##
**`*`** - Required fields
