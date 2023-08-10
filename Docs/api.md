# Genome Data Feed API

## GET: [api](http://localhost:5106/api) - [api/genome-feed](http://localhost/api/genome-feed)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/dna/variants/ssms](http://localhost:5106/api/dna/variants/ssms) - [api/genome-feed/dna/variants/ssms](http://localhost/api/genome-feed/dna/variants/ssms)
Submit mutations (SSM) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```json
[
    {
        "Analysis": {
            "Type": "WGS"
        },
        "Samples": [
            {
                "Id": "SA14",
                "DonorId": "DO1",
                "SpecimenId": "TI2",
                "SpecimenType": "Tissue",
                "MatchedSampleId": null,
                "Variants": null
            },
            {
                "Id": "SA5",
                "DonorId": "DO1",
                "SpecimenId": "TI1",
                "SpecimenType": "Tissue",
                "MatchedSampleId": "SA14",
                "Variants": [
                    {
                        "Chromosome": "7",
                        "Position": "141365018",
                        "Ref": "C",
                        "Alt": "G"
                    },
                    {
                        "Chromosome": "4",
                        "Position": "110895931",
                        "Ref": "A",
                        "Alt": "T"
                    },
                    {
                        "Chromosome": "13",
                        "Position": "100515267",
                        "Ref": "A",
                        "Alt": "G"
                    }
                ]
            }
        ]
    }
]
```
Fields description can be found [here](api-models-ssm.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/variants/cnvs](http://localhost:5106/api/dna/variants/cnvs) - [api/genome-feed/dna/variants/cnvs](http://localhost/api/genome-feed/dna/variants/cnvs)
Submit Copy Number Variants (CNV) data (including sequencing analysis data) in default format.

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```json
[
    {
        "Analysis": {
            "Type": "WGS"
        },
        "Samples": [
            {
                "Id": "SA14",
                "DonorId": "DO1",
                "SpecimenId": "TI2",
                "SpecimenType": "Tissue",
                "MatchedSampleId": null,
                "Variants": null
            },
            {
                "Id": "SA5",
                "DonorId": "DO1",
                "SpecimenId": "TI1",
                "SpecimenType": "Tissue",
                "MatchedSampleId": "SA14",
                "Ploidy": 2,
                "Purity": null,
                "Variants": [
                    {
                        "Chromosome": "4",
                        "Start": 164362032,
                        "End": 164458144,
                        "Type": "Gain",
                        "Loh": false,
                        "HomoDel": false,
                        "C1Mean": 1.2465,
                        "C2Mean": 2.8643,
                        "TcnMean": 4.1108,
                        "C1": 1,
                        "C2": 3,
                        "Tcn": 4,
                        "DhMax": null
                    },
                    {
                        "Chromosome": "5",
                        "Start": 65498712,
                        "End": 65608792,
                        "Type": "Loss",
                        "Loh": true,
                        "HomoDel": false,
                        "C1Mean": 1.1265,
                        "C2Mean": 0.0378,
                        "TcnMean": 1.1643,
                        "C1": 1,
                        "C2": 0,
                        "Tcn": 1,
                        "DhMax": null
                    },
                    {
                        "Chromosome": "6",
                        "Start": 84236917,
                        "End": 84337937,
                        "Type": "Loss",
                        "Loh": false,
                        "HomoDel": true,
                        "C1Mean": 0.1247,
                        "C2Mean": 0.0129,
                        "TcnMean": 0.1376,
                        "C1": 0,
                        "C2": 0,
                        "Tcn": 0,
                        "DhMax": null
                    }
                ]
            }
        ]
    }
]
```
Fields description can be found [here](api-models-cnv.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/variants/cnvs/aceseq](http://localhost:5106/api/dna/variants/cnvs/aceseq) - [api/genome-feed/dna/variants/cnvs/aceseq](http://localhost/api/genome-feed/dna/variants/cnvs/aceseq)
Submit Copy Number Variants (CNV) data (including sequencing analysis data) in ACESeq format.

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```json
[
    {
        "Analysis": {
            "Type": "WGS"
        },
        "Samples": [
            {
                "Id": "SA14",
                "DonorId": "DO1",
                "SpecimenId": "TI2",
                "SpecimenType": "Tissue",
                "MatchedSampleId": null,
                "Variants": null
            },
            {
                "Id": "SA5",
                "DonorId": "DO1",
                "SpecimenId": "TI1",
                "SpecimenType": "Tissue",
                "MatchedSampleId": "SA14",
                "Ploidy": 2,
                "Purity": null,
                "Variants": [
                    {
                        "chromosome": "4",
                        "start": "164362032",
                        "end": "164458144",
                        "sv.Type": "DUP",
                        "cna.Type": "DUP",
                        "c1Mean": "1.2465",
                        "c2Mean": "2.8643",
                        "tcnMean": "4.1108",
                        "A": "1",
                        "B": "3",
                        "TCN": "4",
                        "dhMax": "NA"
                    },
                    {
                        "chromosome": "5",
                        "start": "65498712",
                        "end": "65608792",
                        "sv.Type": "NA",
                        "cna.Type": "DEL;LOH",
                        "c1Mean": "1.1265",
                        "c2Mean": "0.0378",
                        "tcnMean": "1.1643",
                        "A": "1",
                        "B": "0",
                        "TCN": "1",
                        "dhMax": "NA"
                    },
                    {
                        "chromosome": "6",
                        "start": "84236917",
                        "end": "84337937",
                        "sv.Type": "DEL",
                        "cna.Type": "DEL;HomoDEL",
                        "c1Mean": "0.1247",
                        "c2Mean": "0.0129",
                        "tcnMean": "0.1376",
                        "A": "0",
                        "B": "0",
                        "TCN": "0",
                        "dhMax": "NA"
                    }
                ]
            }
        ]
    }
]
```
Fields description can be found [here](api-models-cnv-aceseq.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/dna/variants/svs](http://localhost:5106/api/dna/variants/svs) - [api/genome-feed/dna/variants/svs](http://localhost/api/genome-feed/dna/variants/svs)
Submit Structural Variants (SV) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```json
[
    {
        "Analysis": {
            "Type": "WGS"
        },
        "Samples": [
            {
                "Id": "SA14",
                "DonorId": "DO1",
                "SpecimenId": "TI2",
                "SpecimenType": "Tissue",
                "MatchedSampleId": null,
                "Variants": null
            },
            {
                "Id": "SA5",
                "DonorId": "DO1",
                "SpecimenId": "TI1",
                "SpecimenType": "Tissue",
                "MatchedSampleId": "SA14",
                "Variants": [
                    {
                        "Chromosome1": "6",
                        "Start1": 84236917,
                        "End1": 84236918,
                        "Chromosome2": "6",
                        "Start2": 84337937,
                        "End2": 84337938,
                        "Type": "DUP",
                        "Inverted": false,
                        "FlankingSequence1": null,
                        "FlankingSequence2": null
                    },
                    {
                        "Chromosome1": "8",
                        "Start1": 65498712,
                        "End1": 65498713,
                        "Chromosome2": "8",
                        "Start2": 65608792,
                        "End2": 65608793,
                        "Type": "DEL",
                        "Inverted": false,
                        "FlankingSequence1": null,
                        "FlankingSequence2": null
                    },
                    {
                        "Chromosome1": "8",
                        "Start1": 93246857,
                        "End1": 93246858,
                        "Chromosome2": "8",
                        "Start2": 93347877,
                        "End2": 93347878,
                        "Type": "INV",
                        "Inverted": true,
                        "FlankingSequence1": null,
                        "FlankingSequence2": null
                    }
                ]
            }
        ]
    }
]
```
Fields description can be found [here](api-models-sv.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/rna/expressions](http://localhost:5106/api/rna/expressions) - [api/genome-feed/rna/expressions](http://localhost/api/genome-feed/rna/expressions)
Submit Gene Expression (Transcriptomics) data (including sequencing analysis data).

Request implements **OVERRIDE** logic:
- Data will be overriden if existed

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```json
[
    {
        "Analysis": {
            "Type": "RNA-Seq"
        },
        "Sample": {
            "Id": "SA5",
            "DonorId": "DO1",
            "SpecimenId": "TI2",
            "SpecimenType": "Tissue",
        },
        "Expressions": [
            {
                "GeneId": "ENSG00000223972",
                "Reads": 238
            },
            {
                "GeneId": "ENSG00000243485",
                "Reads": 0
            },
            {
                "GeneId": "ENSG00000274890",
                "Reads": 0
            }
        ]
    }
]
```
Fields description can be found [here](api-models-rna-expression.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions
