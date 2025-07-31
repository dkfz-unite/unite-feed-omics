# Structural Variants (SV)
The model is used to upload the data of structural variants (SV) calling result file and the metadata of the analysis.

> [!note]
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

**`matched_specimen_id`** - Identifier of the matched specimen the sample was created from.
- Type: _String_
- Limitations: Maximum length 255
- Example: `Normal`

**`matched_specimen_type`** - Type of the matched specimen the sample was created from.
- Type: _String_
- Possible values: `Material`, `Line`, `Organoid`, `Xenograft`
- Example: `Material`

**`analysis_type`*** - Type of the analysis performed on the sample.
- Type: _String_
- Possible values: `WGS`, `WES`
- Example: `WES`

**`analysis_date`** - Date when the sample was analysed.
- Type: _Date_
- Limitations: Either 'analysis_date' or 'analysis_day' should be set.
- Format: "YYYY-MM-DD"
- Example: `2023-12-01`

**`analysis_day`** - Relative number of days since donor enrollment when the sample was analysed.
- Type: _Integer_
- Limitations: Integet, greater than or equal to 1, either 'date' or 'day' should be set.
- Example: `22`

**`genome`*** - Reference genome.
- Type: _String_
- Possible values: `GRCh37`, `GRCh38`
- Example: `GRCh37`

**`entries`*** - file with the variants data.
- Type: _File_
- Supported formats: [tsv](#tsv), [dkfz/sophia](#dkfzsophia)
- Limitations: Should be set, should contain at least one element
- Example: `variants.tsv`

**`*`** - Required fields

**Analysis Types**
- `WGS` - Whole Genome Sequencing
- `WES` - Whole Exome Sequencing


## Formats
Several formats are supported for the structural variants data file.

### TSV
Default UNITE format for structural variants data file.  
Tab-separated values (TSV) file with the following columns:

**`chromosome_1`*** - First breakpoint chromosome.
- Type: _String_
- Possible values: `1`, ..., `22`, `X`, `Y`
- Example: `5`

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
- Possible values: `1`, ..., `22`, `X`, `Y`
- Example: `5`

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
- Possible values: `DUP`, `TDUP`, `INS`, `DEL`, `INV`, `ITX`, `CTX`, `COM`
- Example: `DUP`

**`inverted`** - Event inversion.
- Type: _Boolean_
- Example: `false`

**`*`** - Required fields

#### Type
Structural variant type values are:
- `DUP` - duplication
- `TDUP` - tandem duplication
- `INS` - insertion
- `DEL` - deletion
- `INV` - inversion
- `ITX` - intra-chromosomal translocation
- `CTX` - inter-chromosomal translocation
- `COM` - complex rearrangement

#### Example
`variants.tsv`
```tsv
chromosome_1	start_1	end_1	flanking_sequence_1	chromosome_2	start_2	end_2	flanking_sequence_2	type	inverted
5	65498712	65498713	null	5	65499710	65499711	null	DUP	false
5	65700000	65700001	null	5	65800000	65800001	null	TDUP	false
5	65900000	65900001	null	5	66000000	66000001	null	INS	false
5	66100000	66100001	null	5	66200000	66200001	null	DEL	false
5	66300000	66300001    null	5	66400000	66400001	null	INV	false
5	66500000	66500001	null	5	69600000    69600001	null	ITX	false
5	66700000	66700001	null	7   36700000	36700001	null	CTX	false
5	66900000	66900001	null	5	67000000	67000001	null	COM	false
```

### DKFZ SOPHIA
[DKFZ SOPHIA](https://github.com/DKFZ-ODCF/sophia) is a TSV file format used by the DKFZ developed Sohia workflow for structural variants calling.