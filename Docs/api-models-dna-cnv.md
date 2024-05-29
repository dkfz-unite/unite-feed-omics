# Copy Number Variants (CNV) Data Models

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
- Element type: _Object([Variant](api-models-cnv.md#variant))_
- Limitations: Should contain at leas one element
- Example: `[{...}, {...}]`

## Variant
Copy number variant (CNV) data.

**`chromosome`*** - Chromosome.
- Type: _String_
- Possible values: `"1"`, ..., `"22"`, `"X"`, `"Y"`
- Example: `"5"`

**`start`*** - Start position.
- Type: _Integer_
- Limitations: Greater than 0
- Example: `65498712`

**`end`*** - End position.
- Type: _Integer_
- Limitations: Greater than `Start`
- Example: `65608792`

**`type`*** - Copy number alteration type.
- Note: If not set, the api will try to calculate the value from `Tcn` and sample `Ploidy`.
- Type: _String_
- Possible values: `"Gain"`, `"Loss"`, `"Neutral"`, `"Undetermined"`
- Example: `"Loss"`

**`loh`** - Loss of heterozygosity.
- Note: If not set, the api will try to calculate the value from `C1`, `C2` and sample `Ploidy`.
- Type: _Boolean_
- Example: `true`

**`homo_del`** - Homozygous deletion.
- Note: If not set, the api will try to calculate the value from `C1`, `C2` and sample `Ploidy`.
- Type: _Boolean_
- Example: `false`

**`c1_mean`** - Estimated mean number of copies at **major** allele (calculated from number of reads and sample ploidy). 
- Type: _Double_
- Limitations: Greater or equal to 0
- Example: `1.1265`

**`c2_mean`** - Estimated mean number of copies at **minor** allele (calculated from number of reads and sample ploidy). 
- Type: _Double_
- Limitations: Greater or equal to 0
- Example: `0.0378`

**`tcn_mean`** - Estimated mean total number of copies (`C1Mean` + `C2Mean`). 
- Type: _Double_
- Limitations: Greater or equal to 0
- Example: `1.1643`

**`c1`** - Rounded number of copies at **major** allele (Rounded `C1Mean`).
- Note: If not set, the api will try to calculate the value from `C1Mean` with [threshold rule](api-models-cnv.md#threshold-rule).
- Limitations: Greater or equal to `0` or `-1` if not precise enough ([threshold rule](api-models-cnv.md#threshold-rule))
- Type: _Integer_
- Example: `1`

**`c2`** - Rounded number of copies at **minor** allele (Rounded `C2Mean`).
- Note: If not set, the api will try to calculate the value from `C2Mean` with [threshold rule](api-models-cnv.md#threshold-rule).
- Limitations: Greater or equal to `0` or `-1` if not precise enough ([threshold rule](api-models-cnv.md#threshold-rule))
- Type: _Integer_
- Example: `0`

**`tcn`** - Rounded total number of copies (`C1` + `C2`).
- Note: If not set, the api will try to calculate the value from `C1` and `C2` or `TcnMean` with [threshold rule](api-models-cnv.md#threshold-rule).
- Limitations: Greater or equal to `0` or `-1` if not precise enough ([threshold rule](api-models-cnv.md#threshold-rule))
- Type: _Integer_
- Example: `1`

**`dh_max`** - Esstimated decrease of heterozygosity (calculated from number of reads and sample ploidy).
- Limitations: Greater or equal to 0
- Type: _Integer_
- Example: `1`

#### Type
Type values are:
- `"Gain"` - total number of copies is certainly higher than sample ploidy
- `"Loss"` - total number of copies is certainly lower than sample ploidy
- `"Neutral"` - total number of copies is certainly similar to sample ploidy
- `"Undetermined"` - variant type is not certain

#### Threshold Rule
To round double value to integer there is `0.3` certancy threshold applied:
if the value is more than `0.3` far from the nearest integer, it is considered as not precise enought and rounding operation results to `-1`, 
this makes further processing of the value easier for the API.

##
**`*`** - Required fields
