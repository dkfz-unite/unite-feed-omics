# Genome Data Feed API
API for uploading genome data to the repository.

> [!Note]
> API is accessible for authorized users only and requires `JWT` token as `Authorization` header (read more about [Identity Service](https://github.com/dkfz-unite/unite-identity)).

API is **proxied** to main API and can be accessed at [[host]/api/genome-feed](http://localhost/api/genome-feed) (**without** `api` prefix).


## Overview
- get:[api](#get-api) - health check.
- post:[api/dna/variants/ssms](#post-apidnavariantsssms) - submit SSM sequencing data.
- post:[api/dna/variants/cnvs](#post-apidnavariantscnvs) - submit CNV sequencing data.
- post:[api/dna/variants/svs](#post-apidnavariantssvs) - submit SV sequencing data.
- post:[api/rna/expressions](#post-apirnaexpressions) - submit **bulk** gene expression sequencing data.

> [!Note]
> **Json** is default data type for all requests and will be used if no data type is specified.


## GET: [api](http://localhost:5106/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/dna/variants/ssms](http://localhost:5106/api/dna/variants/ssms)
Submit mutations (SSM) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Body

#### json - application/json
```json
{
    "analysis": {
        "id": "AN1",
        "type": "WGS",
        "date": "2023-12-01",
        "parameters": {
            "key1": "value1",
            "key2": "value2"
        }
    },
    "target_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI1",
        "specimen_type": "Tissue"
    },
    "matched_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI2",
        "specimen_type": "Tissue"
    },
    "variants": [
        {
            "chromosome": "7",
            "position": "141365018",
            "ref": "C",
            "alt": "G"
        },
        {
            "chromosome": "4",
            "position": "110895931",
            "ref": "A",
            "alt": "T"
        },
        {
            "chromosome": "13",
            "position": "100515267",
            "ref": "A",
            "alt": "G"
        }
    ]
}
```
Fields description can be found [here](api-models-ssm.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/variants/cnvs](http://localhost:5106/api/dna/variants/cnvs)
Submit Copy Number Variants (CNV) data (including sequencing analysis data) in default format.

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Body

#### json - application/json
```json
{
    "analysis": {
        "id": "AN1",
        "type": "WGS",
        "date": "2023-12-01",
        "parameters": {
            "key1": "value1",
            "key2": "value2"
        }
    },
    "target_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI1",
        "specimen_type": "Tissue",
        "purity": null,
        "ploidy": 2
    },
    "matched_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI2",
        "specimen_type": "Tissue"
    },
    "variants": [
        {
            "chromosome": "4",
            "start": 164362032,
            "end": 164458144,
            "type": "Gain",
            "loh": false,
            "homo_del": false,
            "c1_mean": 1.2465,
            "c2_mean": 2.8643,
            "tcn_mean": 4.1108,
            "c1": 1,
            "c2": 3,
            "tcn": 4,
            "dh_max": null
        },
        {
            "chromosome": "5",
            "start": 65498712,
            "end": 65608792,
            "type": "Loss",
            "loh": true,
            "homo_del": false,
            "c1_mean": 1.1265,
            "c2_mean": 0.0378,
            "tcn_mean": 1.1643,
            "c1": 1,
            "c2": 0,
            "tcn": 1,
            "dh_max": null
        },
        {
            "chromosome": "6",
            "start": 84236917,
            "end": 84337937,
            "type": "Loss",
            "loh": false,
            "homo_del": true,
            "c1_mean": 0.1247,
            "c2_mean": 0.0129,
            "tcn_mean": 0.1376,
            "c1": 0,
            "c2": 0,
            "tcn": 0,
            "dh_max": null
        }
    ]
}
```
Fields description can be found [here](api-models-cnv.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/variants/svs](http://localhost:5106/api/dna/variants/svs)
Submit Structural Variants (SV) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Body

#### json - application/json
```json
{
    "analysis": {
        "id": "AN1",
        "type": "WGS",
        "date": "2023-12-01",
        "parameters": {
            "key1": "value1",
            "key2": "value2"
        }
    },
    "target_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI1",
        "specimen_type": "Tissue",
        "purity": null,
        "ploidy": 2
    },
    "matched_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI2",
        "specimen_type": "Tissue"
    },
    "variants": [
        {
            "chromosome_1": "6",
            "start_1": 84236917,
            "end_1": 84236918,
            "chromosome_2": "6",
            "start_2": 84337937,
            "end_2": 84337938,
            "type": "DUP",
            "inverted": false,
            "flanking_sequence_1": null,
            "flanking_sequence_2": null
        },
        {
            "chromosome_1": "8",
            "start_1": 65498712,
            "end_1": 65498713,
            "chromosome_2": "8",
            "start_2": 65608792,
            "end_2": 65608793,
            "type": "DEL",
            "inverted": false,
            "flanking_sequence_1": null,
            "flanking_sequence_2": null
        },
        {
            "chromosome_1": "8",
            "start_1": 93246857,
            "end_1": 93246858,
            "chromosome_2": "8",
            "start_2": 93347877,
            "end_2": 93347878,
            "type": "INV",
            "inverted": true,
            "flanking_sequence_1": null,
            "flanking_sequence_2": null
        }
    ]
}
```
Fields description can be found [here](api-models-sv.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/rna/expressions](http://localhost:5106/api/rna/expressions)
Submit Gene Expression (Transcriptomics) data (including sequencing analysis data).

Request implements **OVERRIDE** logic:
- Data will be overriden if existed

### Body

#### json - application/json
```json
{
    "analysis": {
        "id": "AN2",
        "type": "RNASeq",
        "date": "2023-12-01",
        "parameters": {
            "key1": "value1",
            "key2": "value2"
        }
    },
    "target_sample": {
        "donor_id": "DO1",
        "specimen_id": "TI1",
        "specimen_type": "Tissue"
    },
    "expressions": [
        {
            "gene_id": "ENSG00000223972",
            "reads": 238
        },
        {
            "gene_id": "ENSG00000243485",
            "reads": 0
        },
        {
            "gene_id": "ENSG00000274890",
            "reads": 0
        }
    ]
}
```
Fields description can be found [here](api-models-rna-expression.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions
