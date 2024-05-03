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
- Possible values: `"Material"`, `"Line"`, `"Organoid"`, `"Xenograft"`
- Example: `"Material"`

**`ploidy`** - Sample ploidy.
- Notes: Available for CNVs data only.
- Type: _Double_
- Limitations: Should be greater than 0
- Example: `2`

**`purity`** - Sample purity (TCC) percentage of tumor cells in the tissue.
- Notes: Available for CNVs data only.
- Type: _Double_
- Limitations: Should be between 0 and 100
- Example: `95`

**`cells_number`** - Number of cells in the sample.
- Notes: Available for single cell RNA sequenicng dat only.
- Type: _Integer_
- Limitations: Should be greater than 1
- Example: `1000`

**`genes_model`** - Genes model used for the analysis.
- Notes: Available for single cell RNA sequenicng data only. Usually model names is known, but if not, it can be calculated as MD5 hash of alphabetically ordered gene symbols.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Ensembl v99"`


#### Specimen Type
Specimen can be of the following types:
- `"Material"` - all donor derived materials
- `"Line"` - cell lines
- `"Organoid"` - organoids
- `"Xenograft"` - xenografts

##
**`*`** - Required fields
