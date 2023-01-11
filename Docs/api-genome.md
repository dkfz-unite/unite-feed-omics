# Genome Data Feed API

## GET: [api](http://localhost:5106/api)

Health check.


**Response**

`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/mutations](http://localhost:5106/api/mutations)

Submit mutations (SSM) data (including sequencing analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

**Boby** (_application/json_)
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
Fields description can be found [here](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-genome-models.md).

**Response**
- `200` - request was processed successfully
- `400` - request data didn't pass validation
