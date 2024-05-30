# Genome Data Feed API
API for uploading genome data to the repository.

> [!Note]
> API is accessible for authorized users only and requires `JWT` token as `Authorization` header (read more about [Identity Service](https://github.com/dkfz-unite/unite-identity)).

API is **proxied** to main API and can be accessed at [[host]/api/genome-feed](http://localhost/api/genome-feed) (**without** `api` prefix).


## Overview
- get:[api](#get-api) - health check.
- post:[api/dna/ssms/{type?}](#post-apidnassmstype) - submit SSMs data.
- post:[api/dna/cnvs/{type?}](#post-apidnacnvstype) - submit CNVs data.
- post:[api/dna/svs/{type?}](#post-apidnasvstype) - submit SVs data.
- post:[api/rna/exps/{type?}](#post-apirnaexpstype) - submit **bulk** gene expressions data.
- post:[api/rnasc/exps/{type?}](#post-apirnascexpstype) - submit **single cell** gene expressions data.

> [!Note]
> **Json** is default data type for all requests and will be used if no data type is specified.  
> **Tsv** utilizes comment lines to specify metadata. It's possible to upload only **one** resource per sample in this format.

> [!Note]
> You can upload only one sample per data type (DNA(WES,WGS), RNA, RNASc).
> Variants calling utilizes the same sample alingment files, so they can be uploaded only once.  
> E.g. if you upload alignement resources (BAM files) for samples with SSMs data, you don't need to upload them again for CNVs or SVs.


## GET: [api](http://localhost:5106/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/dna/ssms/{type?}](http://localhost:5106/api/dna/ssms)
Submit mutations (SSM) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Body

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

#### json - application/json
```json
{
    "tsample": {
        "donor_id": "Donor1",
        "specimen_id": "Material2",
        "specimen_type": "Material",
        "analysis_type": "WGS",
        "analysis_date": "2023-12-01",
        "resources": [
            {
                "type": "dna",
                "format": "bam",
                "url": "example.com/my/file"
            }
        ]
                
    },
    "msample": {
        "donor_id": "Donor1",
        "specimen_id": "Material1",
        "specimen_type": "Material",
        "analysis_type": "WGS",
        "analysis_date": "2023-12-01",
        "resources": [
            {
                "type": "dna",
                "format": "bam",
                "url": "example.com/my/file"
            }
        ]
    },
    "entries": [
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

#### tsv - text/tab-separated-values
```tsv
# tsample_donor_id: Donor1
# tsample_specimen_id: Material2
# tsample_specimen_type: Material
# tsample_analysis_type: WGS
# tsample_analysis_date: 2023-12-01
# tsample_resource_type: dna
# tsample_resource_format: bam
# tsample_resource_url: example.com/my/file
# msample_donor_id: Donor1
# msample_specimen_id: Material1
# msample_specimen_type: Material
# msample_analysis_type: WGS
# msample_analysis_date: 2023-12-01
# msample_resource_type: dna
# msample_resource_format: bam
# msample_resource_url: example.com/my/file
chromosome	position	ref	alt
7	141365018	C	G
4	110895931	A	T
13	100515267	A	G
```

Fields description can be found [here](api-models-dna-ssm.md).


### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/cnvs/{type?}](http://localhost:5106/api/dna/cnvs)
Submit Copy Number Variants (CNV) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

#### json - application/json
```json
{
    "tsample": {
        "donor_id": "Donor1",
        "specimen_id": "Material2",
        "specimen_type": "Material",
        "analysis_type": "WGS",
        "analysis_date": "2023-12-01",
        "purity": 95,
        "ploidy": 2
    },
    "msample": {
        "donor_id": "Donor1",
        "specimen_id": "Material1",
        "specimen_type": "Material",
        "analysis_type": "WGS",
        "analysis_date": "2023-12-01",
    },
    "entries": [
        {
            "chromosome": "4",
            "start": 164362032,
            "end": 164458144,
            "type": "Gain",
            "loh": false,
            "del": false,
            "c1_mean": 1.2465,
            "c2_mean": 2.8643,
            "tcn_mean": 4.1108,
            "c1": 1,
            "c2": 3,
            "tcn": 4
        },
        {
            "chromosome": "5",
            "start": 65498712,
            "end": 65608792,
            "type": "Loss",
            "loh": true,
            "del": false,
            "c1_mean": 1.1265,
            "c2_mean": 0.0378,
            "tcn_mean": 1.1643,
            "c1": 1,
            "c2": 0,
            "tcn": 1
        },
        {
            "chromosome": "6",
            "start": 84236917,
            "end": 84337937,
            "type": "Loss",
            "loh": false,
            "del": true,
            "c1_mean": 0.1247,
            "c2_mean": 0.0129,
            "tcn_mean": 0.1376,
            "c1": 0,
            "c2": 0,
            "tcn": 0
        }
    ]
}
```

#### tsv - text/tab-separated-values
```tsv
# tsample_donor_id: Donor1
# tsample_specimen_id: Material2
# tsample_specimen_type: Material
# tsample_analysis_type: WGS
# tsample_analysis_date: 2023-12-01
# tsample_purity: 95
# tsample_ploidy: 2
# msample_donor_id: Donor1
# msample_specimen_id: Material1
# msample_specimen_type: Material
# msample_analysis_type: WGS
# msample_analysis_date: 2023-12-01
chromosome	start	end	type	loh	del	c1_mean	c2_mean	tcn_mean	c1	c2	tcn
4	164362032	164458144	Gain	false	false	1.2465	2.8643	4.1108	1	3	4
5	65498712	65608792	Loss	true	false	1.1265	0.0378	1.1643	1	0	1
6	84236917	84337937	Loss	false	true	0.1247	0.0129	0.1376	0	0	0
```

Fields description can be found [here](api-model-dna-cnv.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/svs/{type?}](http://localhost:5106/api/dna/svs)
Submit Structural Variants (SV) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

#### json - application/json
```json
{
    "tsample": {
        "donor_id": "Donor1",
        "specimen_id": "Material2",
        "specimen_type": "Material",
        "analysis_type": "WGS",
        "analysis_date": "2023-12-01"
    },
    "matched_sample": {
        "donor_id": "Donor1",
        "specimen_id": "Material1",
        "specimen_type": "Material",
        "analysis_type": "WGS",
        "analysis_date": "2023-12-01"
    },
    "entries": [
        {
            "chromosome_1": "6",
            "start_1": 84236917,
            "end_1": 84236918,
            "flanking_sequence_1": null,
            "chromosome_2": "6",
            "start_2": 84337937,
            "end_2": 84337938,
            "flanking_sequence_2": null,
            "type": "DUP",
            "inverted": false
        },
        {
            "chromosome_1": "8",
            "start_1": 65498712,
            "end_1": 65498713,
            "flanking_sequence_1": null,
            "chromosome_2": "8",
            "start_2": 65608792,
            "end_2": 65608793,
            "flanking_sequence_2": null,
            "type": "DEL",
            "inverted": false
        },
        {
            "chromosome_1": "8",
            "start_1": 93246857,
            "end_1": 93246858,
            "flanking_sequence_1": null,
            "chromosome_2": "8",
            "start_2": 93347877,
            "end_2": 93347878,
            "flanking_sequence_2": null,
            "type": "INV",
            "inverted": true
        }
    ]
}
```

#### tsv - text/tab-separated-values
```tsv
# tsample_donor_id: Donor1
# tsample_specimen_id: Material2
# tsample_specimen_type: Material
# tsample_analysis_type: WGS
# tsample_analysis_date: 2023-12-01
# msample_donor_id: Donor1
# msample_specimen_id: Material1
# msample_specimen_type: Material
# msample_analysis_type: WGS
# msample_analysis_date: 2023-12-01
chromosome_1	start_1	end_1	flanking_sequence_1	chromosome_2	start_2	end_2	flanking_sequence_2	type	inverted
6	84236917	84236918	.	6	84337937	84337938	.	DUP	false
8	65498712	65498713	.	8	65608792	65608793	.	DEL	false
8	93246857	93246858	.	8	93347877	93347878	.	INV	true
```

Fields description can be found [here](api-models-dna-sv.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/rna/exps/{type?}](http://localhost:5106/api/rna/exps)
Submit Bulk Gene Expression (Transcriptomics) data (including sequencing analysis data).

Request implements **OVERRIDE** logic:
- Data will be overriden if existed

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

#### json - application/json
```json
{
    "target_sample": {
        "donor_id": "Donor1",
        "specimen_id": "Material2",
        "specimen_type": "Material",
        "analysis_type": "RNASeq",
        "analysis_date": "2023-12-01",
        "resources": [
            {
                "type": "rna",
                "format": "bam",
                "url": "example.com/my/file"
            }
        ]
    },
    "entries": [
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

#### tsv - text/tab-separated-values
```tsv
# tsample_donor_id: Donor1
# tsample_specimen_id: Material2
# tsample_specimen_type: Material
# tsample_analysis_type: RNASeq
# tsample_analysis_date: 2023-12-01
# tsample_resource_type: rna
# tsample_resource_format: bam
# tsample_resource_url: example.com/my/file
gene_id	reads
ENSG00000223972	238
ENSG00000243485	0
ENSG00000274890	0
```

Fields description can be found [here](api-models-rna-exp.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/rnasc/exps/{type?}](http://localhost:5106/api/rnasc/exps)
Submit Single Cell Gene Expression (Transcriptomics) data (including sequencing analysis data).

Request implements **OVERRIDE** logic:
- Data will be overriden if existed

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

#### json - application/json
```json
{
    "target_sample": {
        "donor_id": "Donor1",
        "specimen_id": "Material2",
        "specimen_type": "Material",
        "analysis_type": "RNASeqSc",
        "analysis_date": "2023-12-01",
        "cells_number": 5000,
        "genes_model": "Marker genes",
        "resources": [
            {
                "type": "rnasc",
                "format": "bam",
                "url": "example.com/my/file"
            },
            {
                "type": "rnasc/exp",
                "format": "mex",
                "url": "example.com/my/file"
            }
        ]
    }
}
```

#### tsv - text/tab-separated-values
```tsv
# tsample_donor_id: Donor1
# tsample_specimen_id: Material2
# tsample_specimen_type: Material
# tsample_analysis_type: RNASeqSc
# tsample_analysis_date: 2023-12-01
# tsample_cells_number: 5000
# tsample_genes_model: Marker genes
# tsample_resource_type: rnasc
# tsample_resource_format: bam
# tsample_resource_url: example.com/my/file
```

Fields description can be found [here](api-models-rnasc-exp.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions
