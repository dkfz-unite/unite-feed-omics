# Simple Somatic Mutations (SSM) Data Models

## Sequencing Data
Includes information about the analysis, samples and sequencing data.

**`Analysis`** - Sequencing analysis data.
- Type: _Object([Analysis](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-genome-models.md#analysis))_
- Example: `{...}`

**`Samples`*** - Which samples were analysed.
- Type: _Array_
- Element type: _Object([Sample](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-genome-models.md#sample))_
- Example: `[{...}, {...}]`

## Analysis
Sequencing analysis data.

**`Type`*** - Analysis type.
- Type: _String_
- Possible values: `"WGS"`, `"WES"`
- Example: `"WES"`

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

**`Variants`** - Mutations found in sample during the analysis.
- Type: _Array_
- Element type: _Object([Mutation](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-ssm.md#mutation))_
- Limitations: If set, should contain at leas one element
- Example: `[{...}, {...}]`

## Mutation
Mutation data.

**`Chromosome`*** - Chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"7"`

**`Position`*** - Position.
- Type: _String_
- Format: Integer number "[number]" or range "[number]-[number]"
- Example (_number_): `"141365018"`
- Example (_range_): `"141365018-141365019"`

**`Ref`** - Reference base.
- Type: _String_
- Limitations: Should be set if 'Alt' is empty, can contain only 'A', 'C', 'G' or 'T' characters
- Example (SNV): `"C"`
- Example (MNV): `"CTAGTTGA"`
- Example (null): `null` - e.g. in case of insertions

**`Alt`** - Alternate base.
- Type: _String_
- Limitations: Should be set if 'Ref' is empty, can contain only 'A', 'C', 'G' or 'T' characters
- Example (SNV): `"G"`
- Example (MNV): `"GTACCTGA"`
- Example (null): `null` - e.g. in case of deletions

##
**`*`** - Required fields
