# Methylation Levels Data Model

## Sequencing Data
Includes information about analysed sample.

**`tsample`*** - Target sample data. 
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`resources`*** - Analysis result resource files.
- Type: _Array_
- Element type: _Object([Resource](api-models-resource.md))_
- Limitations: Should contain at leas one element of type `"meth-lvl"`
- Example: `[{...}, {...}]`

##
**`*`** - Required fields
