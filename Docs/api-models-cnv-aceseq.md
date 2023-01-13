# Copy Number Variants (CNV) Data Models in ACESeq Format

## Sequencing Data
Includes information about the analysis, samples and sequencing data in ACESeq Format.

**`Analysis`** - Sequencing analysis data.
- Type: _Object([Analysis](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-cnv-aceseq.md#analysis))_
- Example: `{...}`

**`Samples`*** - Which samples were analysed.
- Type: _Array_
- Element type: _Object([Sample](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-cnv-aceseq.md#sample))_
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

**`Ploidy`** - Sample ploidy.
- Type: _Double_
- Limitations: Should be greater than 0
- Example: `2`

**`Purity`** - Sample purity (TCC) percentage of tumor cells in the tissue.
- Type: _Double_
- Limitations: Should be greater than 0
- Example: `95`

**`Variants`** - Copy number variants found in the sample during the analysis.
- Type: _Array_
- Element type: _Object([CNV](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-cnv-aceseq.md#cnv))_
- Limitations: If set, should contain at leas one element
- Example: `[{...}, {...}]`

## CNV
Copy number variant (CNV) data in ACESeq format.

**`chromosome`*** - Chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"5"`

**`start`*** - Start position.
- Type: _String_
- Limitations: Greater than 0
- Example: `"65498712"`

**`end`*** - End position.
- Type: _String_
- Limitations: Greater than `Start`
- Example: `"65608792"`

**`SV.type`** - Supporting structural variant type.
- Type: _String_
- Possible values: `"INS"`, `"DEL"`, `"DUP"`, `"ITX"`, `"CTX"` or `"NA"` if not available
- Example: `"DEL"`

**`CNA.type`** - Copy number alteration type.
- Type: _String_
- Possible values: `"DUP"`, `"DEL"`, `"TCNNeutral"`, `"LOH"`, `"HomoDEL"` or `"NA"` if not available
- Example: `"DEL;LOH"`

**`c1Mean`** - Estimated mean number of copies at **major** allele (calculated from number of reads and sample ploidy). 
- Type: _String_
- Limitations: Greater or equal to 0 or `"NA"` if not available
- Example: `"1.1265"`

**`c2Mean`** - Estimated mean number of copies at **minor** allele (calculated from number of reads and sample ploidy). 
- Type: _String_
- Limitations: Greater or equal to 0 or `"NA"` if not available
- Example: `"0.0378"`

**`tcnMean`** - Estimated mean total number of copies (`c1Mean` + `c2Mean`). 
- Type: _String_
- Limitations: Greater or equal to 0 or `"NA"` if not available
- Example: `"1.1643"`

**`A`** - Rounded number of copies at **major** allele (Rounded `c1Mean`).
- Limitations: Greater or equal to 0 or `"NA"` if not precise enough ([threshold rule](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-cnv-aceseq.md#threshold-rule))
- Type: _String_
- Example: `"1"`

**`B`** - Rounded number of copies at **minor** allele (Rounded `c2Mean`).
- Limitations: Greater or equal to 0 or `"NA"` if not precise enough ([threshold rule](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-cnv-aceseq.md#threshold-rule))
- Type: _String_
- Example: `"0"`

**`TCN`** - Rounded total number of copies (`A` + `B`).
- Limitations: Greater or equal to 0 or `"NA"` if not precise enough ([threshold rule](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-models-cnv-aceseq.md#threshold-rule))
- Type: _String_
- Example: `"1"`

**`dhMax`** - Estimated decrease of heterozygosity (calculated from number of reads and sample ploidy).
- Limitations: Greater or equal to 0 or `"NA"` if not available
- Type: _String_
- Example: `"NA"`

#### SvType
sv.Type values are:
- `"INS"` - insertion
- `"DEL"` - deletion
- `"DUP"` - duplication
- `"ITX"` - intra-chromosomal translocation
- `"CTX"` - inter-chromosomal translocation

#### CnaType
cna.Type values are:
- `"DUP"` - total number of copies is certainly higher than sample ploidy
- `"DEL"` - total number of copies is certainly lower than sample ploidy
- `"TCNNeutral"` - total number of copies is certainly similar to sample ploidy
- `"LOH"` - loss of heterozygosity
- `"HomoDEL"` - homozygous deletion

#### Threshold Rule
To round double value to integer there is `0.3` certancy threshold applied:
if the value is more than `0.3` far from the nearest integer, it is considered as not precise enought and rounding operation results to `"NA"`.

##
**`*`** - Required fields
