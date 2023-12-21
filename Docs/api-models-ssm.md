# Simple Somatic Mutations (SSM) Data Model

## Sequencing Data
Includes information about the analysis, analysed samples and sequencing data.

**`analysis`*** - Sequencing analysis data.
- Type: _Object([Analysis](api-models-analysis.md))_
- Example: `{...}`

**`target_sample`*** - Target sample data. 
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`matched_sample`** - Matched sample data.
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`variants`*** - Variants found in target sample during the analysis.
- Type: _Array_
- Element type: _Object([Variant](api-models-ssm.md#variant))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

## Variant
Simple somatic mutation (SSM) data.

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
