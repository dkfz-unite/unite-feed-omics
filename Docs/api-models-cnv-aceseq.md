# Copy Number Variants (CNV) Data Models in ACESeq Format

## Sequencing Data
Includes information about the analysis, samples and sequencing data in ACESeq Format.

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
- Element type: _Object([Variant](api-models-cnv-aceseq.md#variant))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

## Variant
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
- Limitations: Greater or equal to 0 or `"NA"` if not precise enough ([threshold rule](api-models-cnv-aceseq.md#threshold-rule))
- Type: _String_
- Example: `"1"`

**`B`** - Rounded number of copies at **minor** allele (Rounded `c2Mean`).
- Limitations: Greater or equal to 0 or `"NA"` if not precise enough ([threshold rule](api-models-cnv-aceseq.md#threshold-rule))
- Type: _String_
- Example: `"0"`

**`TCN`** - Rounded total number of copies (`A` + `B`).
- Limitations: Greater or equal to 0 or `"NA"` if not precise enough ([threshold rule](api-models-cnv-aceseq.md#threshold-rule))
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
