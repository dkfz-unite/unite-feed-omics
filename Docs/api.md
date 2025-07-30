# Omics Data Feed API
API for uploading omics data to the repository.

> [!Note]
> API is accessible for authorized users only and requires `JWT` token as `Authorization` header (read more about [Identity Service](https://github.com/dkfz-unite/unite-identity)).

API is **proxied** to main API and can be accessed at [[host]/api/omics-feed](http://localhost/api/omics-feed) (**without** `api` prefix).


## Overview
- get:[api](#get-api) - health check.
- post:[api/dna/sample](#post-apidnasample) - submit DNA sample data.
- post:[api/dna/analysis/sms](#post-apidnaanalysissms) - submit DNA SMs data.
- post:[api/dna/analysis/cnvs](#post-apidnaanalysiscnvs) - submit DNA CNVs data.
- post:[api/dna/analysis/svs](#post-apidnaanalysissvs) - submit DNA SVs data.
- post:[api/meth/sample](#post-apimethsample) - submit Methylation sample data.
- post:[api/rna/sample](#post-apirnasample) - submit **bulk** RNA sample data.
- post:[api/rna/analysis/exps](#post-apirnaanalysisexps) - submit **bulk** RNA gene expressions data.
- post:[api/rnasc/sample](#post-apirnascsample) - submit **single cell** RNA sample data.
- post:[api/rnasc/analysis/exps](#post-apirnascanalysisexps) - submit **single cell** RNA gene expressions data.

> [!Note]
> You can upload only one sample per data type (DNA(WES,WGS), RNA, RNASc, Meth) and matched sample (if required).
> Variants callings utilize the same sample alingment files, no need to upload them multiple times.


## GET: [api](http://localhost:5106/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/dna/sample](http://localhost:5106/api/dna/sample)
Submit DNA sequencing sample metadata and resources.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Body
Request body should be in `multipart/form-data` format for [DNA sample](./models-dna.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `analysis_type` - WES
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `resources` - resources.tsv ([Resources](./models-dna.md#resources) metadata file)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/dna/analysis/sms](http://localhost:5106/api/dna/analysis/sms)
Submit simple mutations (SM) analysis metadata and data.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Parameters
- `format` - variants data [format](./models-dna-sm.md#formats):
  - [tsv](./models-dna-sm.md#tsv) - Default format, if the data ais in this format, the parameter can be omitted.
  - [vcf](./models-dna-sm.md#vcf) - Variant Call Format.

### Body
Request body should be in `multipart/form-data` format for [simple mutations (SM)](./models-dna-sm.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `matched_specimen_id` - Normal
- `matched_specimen_type` - Material
- `analysis_type` - WES
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `entries` - variants.tsv (Variants data file in corresponding format)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/dna/analysis/cnvs](http://localhost:5106/api/dna/analysis/cnvs)
Submit copy number variants (CNV) analysis metadata and data.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Parameters
- `format` - variants data [format](./models-dna-cnv.md#formats):
  - [tsv](./models-dna-cnv.md#tsv) - Default format, if the data is in this format, the parameter can be omitted.
  - [aceseq](./models-dna-cnv.md#aceseq) - ACESeq workflow format.

### Body
Request body should be in `multipart/form-data` format for [copy number variants (CNV)](./models-dna-cnv.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `matched_specimen_id` - Normal
- `matched_specimen_type` - Material
- `analysis_type` - WGS
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `purity` - 0.95
- `ploidy` - 2.0
- `entries` - variants.tsv (Variants data file in corresponding format)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/dna/analysis/svs](http://localhost:5106/api/dna/analysis/svs)
Submit structural variants (SV) analysis metadata and data.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Parameters
- `format` - variants data [format](./models-dna-sv.md#formats):
  - [tsv](./models-dna-sv.md#tsv) - Default format, if the data is in this format, the parameter can be omitted.
  - [dkfz-sophia](./models-dna-sv.md#dkfz-sophia) - DKFZ SOPHIA workflow format.

### Body
Request body should be in `multipart/form-data` format for [structural variants (SV)](./models-dna-sv.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `matched_specimen_id` - Normal
- `matched_specimen_type` - Material
- `analysis_type` - WGS
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `entries` - variants.tsv (Variants data file in corresponding format)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/meth/sample](http://localhost:5106/api/meth/sample)
Submit DNA methylation sample metadata and resources.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Body
Request body should be in `multipart/form-data` format for [DNA methylation sample](./models-meth.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `analysis_type` - MethArray
- `analysis_date` - 2023-12-01
- `genome` - GRCh38
- `resources` - resources.tsv ([Resources](./models-meth.md#resources) metadata file)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/rna/sample](http://localhost:5106/api/rna/sample)
Submit bulk RNA sequencing sample metadata and resources.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Body
Request body should be in `multipart/form-data` format for [bulk RNA sample](./models-rna.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `analysis_type` - RNASeq
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `resources` - resources.tsv ([Resources](./models-rna.md#resources) metadata file)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/rna/analysis/exps](http://localhost:5106/api/rna/analysis/exps)
Submit bulk RNA gene expressions analysis metadata and data.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Parameters
- `format` - expressions data [format](./models-rna-exp.md#formats):
  - [tsv](./models-rna-exp.md#tsv) - Default format, if the data is in this format, the parameter can be omitted.
  - [dkfz-rnaseq](./models-rna-exp.md#dkfz-rnaseq) - DKFZ RNASeq workflow format.

### Body
Request body should be in `multipart/form-data` format for [bulk gene expressions](./models-rna-exp.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `analysis_type` - RNASeq
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `entries` - expressions.tsv (Gene expressions data file)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/rnasc/sample](http://localhost:5106/api/rnasc/sample)
Submit single cell RNA sequencing sample metadata and resources.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Body
Request body should be in `multipart/form-data` format for [single cell RNA sample](./models-rnasc.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `analysis_type` - scRNASeq
- `analysis_date` - 2023-12-01
- `genome` - GRCh37
- `resources` - resources.tsv ([Resources](./models-rnasc.md#resources) metadata file)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.


## POST: [api/rnasc/analysis/exps](http://localhost:5106/api/rnasc/analysis/exps)
Submit single cell RNA gene expressions analysis metadata and resources.

Request implements **UPSERT** logic:
- Missing data will be populated.
- Existing data will be updated.

### Body
Request body should be in `multipart/form-data` format for [single cell gene expressions](./models-rnasc-exp.md) data.

#### Example
- `donor_id` - Donor1
- `specimen_id` - Tumor
- `specimen_type` - Material
- `analysis_type` - scRNASeq
- `analysis_date` - 2023-12-01
- `genome` - GRCh38
- `resources` - resources.tsv ([Resources](./models-rnasc.md#resources) metadata file)

### Responses
- `200` - Request was processed successfully.
- `400` - Request data didn't pass validation.
- `401` - Missing JWT token.
- `403` - Missing required permissions.