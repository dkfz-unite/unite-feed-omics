# Analysis Data Model
Includes the information about sequencing analysis.

>[!NOTE]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`id`** - Analysis identifier.
- Note: If not set, analysis will be identified by it's type, date and analysed samples.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"AN1"`

**`type`*** - Analysis type.
- Type: _String_
- Possible values: `"WGS"`, `"WES"`, `"RNASeq"`, `"ScRNASeq"`, `"Other"`
- Example: `"WES"`

**`date`** - Date of the analysis.
- Type: _String_
- Limitations: Either 'date' or 'day' should be set.
- Format: "YYYY-MM-DD"
- Example: `"2023-12-01"`

**`day`** - Relative number of days from the diagnosis statement.
- Type: _Integer_
- Limitations: Integet, greater than or equal to 1, either 'date' or 'day' should be set.
- Example: `22`

#### Analysis Type
Analysis can be of the following types:
- `"WGS"` - whole genome sequencing
- `"WES"` - whole exome sequencing
- `"RNASeq"` - bulk RNA sequencing
- `"ScRNASeq"` - single cell RNA sequencing
- `"Other"` - other analysis types

##
**`*`** - Required fields
