# CNV Profiles
The model is used to upload copy number variant (CNV) profiles of a sample and the metadata of the analysis.

A CNV profile describes, per chromosome arm, which fraction of the arm is gained, lost or neutral. Profiles can be derived from DNA sequencing or methylation array data and represent the same specimen-level data, regardless of the analysis they come from.

> [!Note]
> All exact dates are hidden and protected. Relative dates are shown instead, if calculation was possible

**`donor_id`*** - Sample donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `Donor1`

**`specimen_id`*** - Identifier of the specimen the sample was created from.
- Type: _String_
- Limitations: Maximum length 255
- Example: `Tumor`

**`specimen_type`*** - Type of the specimen the sample was created from.
- Type: _String_
- Possible values: `Material`, `Line`, `Organoid`, `Xenograft`
- Example: `Material`

**`analysis_type`*** - Type of the analysis performed on the sample.
- Type: _String_
- Possible values: `WGS`, `WES`, `MethArray`
- Example: `WGS`

**`analysis_date`** - Date when the sample was analysed.
- Type: _Date_
- Limitations: Either 'analysis_date' or 'analysis_day' should be set.
- Format: "YYYY-MM-DD"
- Example: `2023-12-01`

**`analysis_day`** - Relative number of days since donor enrollment when the sample was analysed.
- Type: _Integer_
- Limitations: Integer, greater than or equal to 1, either 'date' or 'day' should be set.
- Example: `22`

**`genome`*** - Reference genome.
- Type: _String_
- Possible values: `GRCh37`, `GRCh38`
- Example: `GRCh37`

**`entries`*** - file with the CNV profiles data.
- Type: _File_
- Supported formats: [tsv](#tsv)
- Limitations: Should be set, should contain at least one element
- Example: `profiles.tsv`

**`*`** - Required fields

**Analysis Types**
- `WGS` - Whole Genome Sequencing
- `WES` - Whole Exome Sequencing
- `MethArray` - Methylation Array


## Formats
CNV profiles data file is supported in the following format.

### TSV
Default UNITE format for CNV profiles data file.  
It's a tab-separated values (TSV) file with the following columns:

**`chromosome`*** - Chromosome.
- Type: _String_
- Possible values: `1`-`22`, `X`, `Y`, `MT`
- Example: `7`

**`chromosome_arm`*** - Chromosome arm.
- Type: _String_
- Possible values: `P`, `Q`
- Example: `P`

**`gain`** - Fraction of the arm with copy number gain.
- Type: _Number_
- Limitations: In range [0, 1]
- Example: `0.85`

**`loss`** - Fraction of the arm with copy number loss.
- Type: _Number_
- Limitations: In range [0, 1]
- Example: `0.0`

**`neutral`** - Fraction of the arm without copy number change.
- Type: _Number_
- Limitations: In range [0, 1]
- Example: `0.15`

**`*`** - Required fields

#### Example
`profiles.tsv`
```tsv
chromosome	chromosome_arm	gain	loss	neutral
1	P	0.0	0.12	0.88
1	Q	0.31	0.0	0.69
7	P	0.85	0.0	0.15
7	Q	0.92	0.0	0.08
```
