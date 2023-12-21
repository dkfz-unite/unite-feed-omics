# Sample Data Model
Includes the information about analysed sample.

**`donor_id`*** - Sample donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"DO1"`

**`specimen_id`*** - Identifier of the specimen the sample was created from.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"TI1"`

**`specimen_type`*** - Type of the specimen the sample was created from.
- Type: _String_
- Possible values: `"Tissue"`, `"CellLine"`, `"Organoid"`, `"Xenograft"`
- Example: `"Tissue"`

**`purity`** - Sample purity (TCC) percentage of tumor cells in the tissue.
- Type: _Double_
- Limitations: Should be between 0 and 100
- Example: `95`

**`ploidy`** - Sample ploidy.
- Type: _Double_
- Limitations: Should be greater than 0
- Example: `2`

#### Specimen Type
Specimen can be of the following types:
- `"Tissue"` - all donor derived specimens
- `"CellLine"` - cell lines
- `"Organoid"` - organoids
- `"Xenograft"` - xenografts