# Simple Mutations (SM) Data Model

## Sequencing Data
Includes information about analysed samples and variants data.

**`tsample`*** - Target sample data. 
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`msample`** - Matched sample data.
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`resources`** - Analysis result resource files.
- Type: _Array_
- Element type: _Object([Resource](api-models-resource.md))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

**`entries`*** - Variants found in target sample during the analysis.
- Type: _Array_
- Element type: _Object([Variant](api-models-sm.md#variant))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

## Variant
Simple mutation (SM) data.

**`chromosome`*** - Chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"7"`

**`position`*** - Position.
- Type: _String_
- Format: Integer number "[number]" or range "[number]-[number]"
- Example (_number_): `"141365018"`
- Example (_range_): `"141365018-141365019"`

**`ref`** - Reference base.
- Type: _String_
- Limitations: Should be set if 'Alt' is empty, can contain only 'A', 'C', 'G' or 'T' characters
- Example (SNV): `"C"`
- Example (MNV): `"CTAGTTGA"`
- Example (null): `null` - e.g. in case of insertions

**`alt`** - Alternate base.
- Type: _String_
- Limitations: Should be set if 'Ref' is empty, can contain only 'A', 'C', 'G' or 'T' characters
- Example (SNV): `"G"`
- Example (MNV): `"GTACCTGA"`
- Example (null): `null` - e.g. in case of deletions

##
**`*`** - Required fields
