# Structural Variants (SV) Data Model

## Sequencing Data
Includes information about analysed samples and variants data.

**`tsample`*** - Target sample data. 
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`msample`** - Matched sample data.
- Type: _Object([Sample](api-models-sample.md))_
- Example: `{...}`

**`entries`*** - Variants found in target sample during the analysis.
- Type: _Array_
- Element type: _Object([Variant](api-models-sv.md#variant))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

## Variant
Structural variant (SV) data.

**`chromosome_1`*** - First breakpoint chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"5"`

**`start_1`*** - First breakpoint start position.
- Type: _Integer_
- Limitations: Greater than 0
- Example: `65498712`

**`end_1`** - First breakpoint end position.
- Note: If not set, will be set to `Start1` + 1.
- Type: _Integer_
- Limitations: Greater than `Start1`
- Example: `65498713`

**`flanking_sequence_1`** - Flanking genomic sequence 200bp around first breakpoint.
- Type: _String_
- Example: `null`

**`chromosome_2`*** - Second breakpoint chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"5"`

**`start_2`*** - Second breakpoint start position.
- Type: _Integer_
- Limitations: Greater than 0
- Example: `65608792`

**`end_2`** - Second breakpoint end position.
- Note: If not set, will be set to `Start2` + 1.
- Type: _Integer_
- Limitations: Greater than `Start2`
- Example: `65608793`

**`flanking_sequence_2`** - Flanking genomic sequence 200bp around second breakpoint.
- Type: _String_
- Example: `null`

**`type`*** - Structural variant type.
- Type: _String_
- Possible values: `"DUP"`, `"TDUP"`, `"INS"`, `"DEL"`, `"INV"`, `"ITX"`, `"CTX"`, `"COM"`
- Example: `"DUP"`

**`inverted`** - Event inversion.
- Type: _Boolean_
- Example: `false`

#### Type
Structural variant type values are:
- `"DUP"` - duplication
- `"TDUP"` - tandem duplication
- `"INS"` - insertion
- `"DEL"` - deletion
- `"INV"` - inversion
- `"ITX"` - intra-chromosomal translocation
- `"CTX"` - inter-chromosomal translocation
- `"COM"` - complex rearrangement

##
**`*`** - Required fields
