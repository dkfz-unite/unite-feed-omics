# Protein Expressions
The model is used to upload the data of protein expressions (mass spectrometry proteomics) and the metadata of the analysis.

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
- Possible values: `MS`
- Example: `MS`

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

**`entries`*** - file with the expressions data.
- Type: _File_
- Supported formats: [tsv](#tsv), [diann](#dia-nn)
- Limitations: Should be set, should contain at least one element
- Example: `expressions.tsv`

**`*`** - Required fields

**Analysis Types**
- `MS` - Mass Spectrometry


## Formats
Several formats are supported for protein expressions data file.

> [!Note]
> Provide raw intensities. Normalised intensities (log2-transformed and median-centred) are calculated by the Portal, no external values can be provided.  
> Proteins are matched with [Ensembl](https://www.ensembl.org/index.html) protein data: by Ensembl protein identifier, by [UniProt](https://www.uniprot.org/) (Swiss-Prot) accession, or by symbol.

The data can be submitted by only one of the following strategies (one of these fields should be set):
- `id` - Ensembl protein identifier
- `accession` - UniProt accession
- `symbol` - Protein symbol

### TSV
Default UNITE format for protein expressions data file.  
It's a tab-separated values (TSV) file with the following columns:

**`id`** - Ensembl protein identifier. A version suffix (`.1`) is ignored.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"ENSP00000269305"`

**`accession`** - UniProt (Swiss-Prot) accession.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"P04637"`

**`symbol`** - Protein symbol.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"TP53"`

**`intensity`*** - Raw protein intensity.
- Type: _Number_
- Limitations: Should be greater than or equal to 0
- Example: `152340.5`

**`*`** - Required fields

#### Example
`expressions.tsv`
```tsv
accession	intensity
P04637	152340.5
P38398	8421.0
Q9Y6K9	0
```

### DIA-NN
[DIA-NN](https://github.com/vdemichev/DiaNN) protein groups output. The following columns are used:

- `Protein.Group` - UniProt accessions of the protein group, separated by `;`, `,` or spaces.
- `Quantity` - Protein group quantity, used as raw intensity.

Groups with a single accession are used as they are. For ambiguous groups with several accessions, each accession receives the group quantity, unless it is already identified by a single-accession group.
