# Proteomics Sample
The model is used to upload the data of mass spectrometry (MS) proteomics sample metadata and files.

A sample is one LC-MS/MS run of the specimen material. Its resources are the run's spectra files in the open [mzML](https://www.psidev.info/mzML) format. Vendor-specific raw formats (e.g. Thermo `.raw`, Bruker `.d`, Sciex `.wiff`) are not supported, convert them to mzML first (e.g. with ProteoWizard `msconvert`). Protein expressions calculated from the run are uploaded separately as [protein expressions](./models-prot-exp.md).

> [!Note]
> All exact dates are hidden and protected. Relative dates are shown instead, if calculation was possible.

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

**`resources`*** - file with the sample resources metadata.
- Type: _File_
- Supported formats: [tsv](#resources)
- Limitations: Should be set, should contain at least one element
- Example: `resources.tsv`

**`*`** - Required fields

**Analysis Types**
- `MS` - Mass Spectrometry


## Resources
The resources file contains resource file metadata required for remote file access and processing.  
It's a tab-separated values (TSV) file with the following columns:

**`name`*** - Resource file name (with extension).
- Type: _String_
- Limitations: Maximum length 100
- Example: `tumor.mzML`

**`format`*** - Resource format (case-insensitive).
- Type: _String_
- Available values: `mzml`
- Example: `"mzml"`

**`url`*** - Resource URL on remote a server.
- Type: _String_
- Example: `https://example.com/file/abcd101`

**`*`** - Required fields

If the material was fractionated or measured in technical replicates, list one resource per run file.

### Example
```tsv
name	format	url
tumor.mzML	mzml	https://example.com/file/abcd101
```
