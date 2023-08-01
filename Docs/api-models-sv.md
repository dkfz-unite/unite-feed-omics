# Structural Variants (SV) Data Models

## Sequencing Data
Includes information about the analysis, samples and sequencing data.

**`Analysis`** - Sequencing analysis data.
- Note: It's recomended to set analysis data (at least it's type), but it's not required.
- Type: _Object([Analysis](api-models-sv.md#analysis))_
- Example: `{...}`

**`Samples`*** - Which samples were analysed.
- Type: _Array_
- Element type: _Object([Sample](api-models-sv.md#sample))_
- Example: `[{...}, {...}]`

## Analysis
Sequencing analysis data.

**`Id`** - Analysis identifier.
- Note: If not set, analysis will be identified by it's type, date and analysed samples.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"AN1"`

**`Type`*** - Analysis type.
- Type: _String_
- Possible values: `"WGS"`, `"WES"`
- Example: `"WES"`

**`Date`** - Date of the analysis.
- Type: _String_
- Format: "YYYY-MM-DDTHH:MM:SS"
- Example: `"2021-01-01T00:00:00"`

#### Analysis Type
Analysis can be of the following types:
- `"WGS"` - whole genome sequencing
- `"WES"` - whole exome sequencing

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

**`MatchedSampleId`** - Matched(control) sample identifier from samples array.
- Type: _String_
- Limitations: Should match single sample identifier from samples array
- Example: `"SA14"`

**`Variants`** - Structural variants found in the sample during the analysis.
- Type: _Array_
- Element type: _Object([SV](api-models-sv.md#sv))_
- Limitations: If set, should contain at leas one element
- Example: `[{...}, {...}]`

#### Specimen Type
Specimen can be of the following types:
- `"Tissue"` - all donor derived specimens
- `"CellLine"` - cell lines
- `"Organoid"` - organoids
- `"Xenograft"` - xenografts

## SV
Structural variant (SV) data.

**`Chromosome1`*** - First breakpoint chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"5"`

**`Start1`*** - First breakpoint start position.
- Type: _Integer_
- Limitations: Greater than 0
- Example: `65498712`

**`End1`** - First breakpoint end position.
- Note: If not set, will be set to `Start1` + 1.
- Type: _Integer_
- Limitations: Greater than `Start1`
- Example: `65498713`

**`Chromosome2`*** - Second breakpoint chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"5"`

**`Start2`*** - Second breakpoint start position.
- Type: _Integer_
- Limitations: Greater than 0
- Example: `65608792`

**`End2`** - Second breakpoint end position.
- Note: If not set, will be set to `Start2` + 1.
- Type: _Integer_
- Limitations: Greater than `Start2`
- Example: `65608793`

**`Type`*** - Structural variant type.
- Type: _String_
- Possible values: `"DUP"`, `"TDUP"`, `"INS"`, `"DEL"`, `"INV"`, `"ITX"`, `"CTX"`, `"COM"`
- Example: `"DUP"`

**`Inverted`** - Event inversion.
- Type: _Boolean_
- Example: `false`

**`FlankingSequence1`** - Flanking genomic sequence 200bp around first breakpoint.
- Type: _String_
- Example: `null`

**`FlankingSequence2`** - Flanking genomic sequence 200bp around second breakpoint.
- Type: _String_
- Example: `null`

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
