# Omics Data Feed Service

## General
Omics data feed service provides the following functionality:
- [Omics data feed web API](Docs/api.md) - REST API for uploading sequencing data to the portal (including input data validation).
- Gene expressions annotation service - background service responsible for gene expressions annotation.
  - Gene expressions are annotated with local installation of Ensembl Data service.
- Genes data indexing service - background service responsible for gene-centric data index creation.
- Variants data annotation service - background service responsible for variants annotation.
  - Variants are annotated with local installation of Ensembl VEP service.
  - Genes and transcripts are annotated with local installation of Ensembl Data service.
- Variants data indexing service - background service responsible for variant-centric data index creation.

## Dependencies
- [SQL](https://github.com/dkfz-unite/unite-environment/tree/main/programs/postgresql) - SQL server with domain data and user identity data.
- [Elasticsearch](https://github.com/dkfz-unite/unite-environment/tree/main/programs/elasticsearch) - Elasticsearch server with indices of domain data.
- [Ensembl Data](https://github.com/dkfz-unite/unite-environment/tree/main/applications/unite-ensembl-data) - Local installation of Ensembl Data service.
- [Ensembl VEP](https://github.com/dkfz-unite/unite-environment/tree/main/applications/unite-ensembl-vep) - Local installation of Ensembl VEP service.

## Access
Environment|Address|Port
-----------|-------|----
Host|http://localhost:5106|5106
Docker|http://feed.omics.unite.net|80

## Configuration
To configure the application, change environment variables in either docker or [launchSettings.json](./Unite.Omics.Feed.Web/Properties/launchSettings.json) file (if running locally):

- `ASPNETCORE_ENVIRONMENT` - ASP.NET environment (`Release`).
- `UNITE_API_KEY` - API key for decription of JWT token and user authorization.
- `UNITE_ELASTIC_HOST` - Elasticsearch service host (`es.unite.net:9200`).
- `UNITE_ELASTIC_USER` - Elasticsearch service user.
- `UNITE_ELASTIC_PASSWORD` - Elasticsearch service password.
- `UNITE_SQL_HOST` - SQL server host (`sql.unite.net`).
- `UNITE_SQL_PORT` - SQL server port (`5432`).
- `UNITE_SQL_USER` - SQL server user.
- `UNITE_SQL_PASSWORD` - SQL server password.
- `UNITE_MONGO_HOST` - MongoDB server host (`mongo.unite.net`).
- `UNITE_MONGO_PORT` - MongoDB server port (`27017`).
- `UNITE_MONGO_USER` - MongoDB server user.
- `UNITE_MONGO_PASSWORD` - MongoDB server password.
- `UNITE_ENSEMBL_DATA_HOST` - Local Ensembl Data host (`data.ensembl.unite.net`).
- `UNITE_ENSEMBL_VEP_HOST` - Local Ensembl VEP host (`vep.ensembl.unite.net`).
- `UNITE_GENES_INDEXING_BUCKET_SIZE` - Genes indexing bucket size (`100`).
- `UNITE_SM_ANNOTATION_BUCKET_SIZE` - SM annotation bucket size (`100`).
- `UNITE_SM_INDEXING_BUCKET_SIZE` - SM indexing bucket size (`300`).
- `UNITE_CNV_ANNOTATION_BUCKET_SIZE` - CNV annotation bucket size (`10`).
- `UNITE_CNV_INDEXING_BUCKET_SIZE` - CNV indexing bucket size (`100`).
- `UNITE_SV_ANNOTATION_BUCKET_SIZE` - SV annotation bucket size (`10`).
- `UNITE_SV_INDEXING_BUCKET_SIZE` - SV indexing bucket size (`100`).
- `UNITE_GENOME_BUILD` - Reference genome build (`GRCh37`).

> [!Note]
> Reference genome build defines, which version of Ensembl VEP and Ensembl Data services to use in the system.  
> All the data uploaded to the system should be in this genome build.  
> It's not possible to mix the data from different versions of the genome build in the system.


## Installation

### Docker Compose
The easiest way to install the application is to use docker-compose:
- Environment configuration and installation scripts: https://github.com/dkfz-unite/unite-environment
- Omics data feed service configuration and installation scripts: https://github.com/dkfz-unite/unite-environment/tree/main/applications/unite-omics-feed

### Docker
The image of the service is available in our [registry](https://github.com/dkfz-unite/unite-omics-feed/pkgs/container/unite-omics-feed).

[Dockerfile](./Dockerfile) is used to build an image of the application.
To build an image run the following command:
```
docker build -t unite.omics.feed:latest .
```

All application components should run in the same docker network.
To create common docker network if not yet available run the following command:
```bash
docker network create unite
```

To run application in docker run the following command:
```bash
docker run \
--name unite.omics.feed \
--restart unless-stopped \
--net unite \
--net-alias feed.omics.unite.net \
-p 127.0.0.1:5106:80 \
-e ASPNETCORE_ENVIRONMENT=Release \
-e UNITE_API_KEY=[api_key] \
-e UNITE_ELASTIC_HOST=http://es.unite.net:9200 \
-e UNITE_ELASTIC_USER=[elasticsearch_user] \
-e UNITE_ELASTIC_PASSWORD=[elasticsearch_password] \
-e UNITE_SQL_HOST=sql.unite.net \
-e UNITE_SQL_PORT=5432 \
-e UNITE_SQL_USER=[sql_user] \
-e UNITE_SQL_PASSWORD=[sql_password] \
-e UNITE_MONGO_HOST=mongo.unite.net \
-e UNITE_MONGO_PORT=27017 \
-e UNITE_MONGO_USER=[mongo_user] \
-e UNITE_MONGO_PASSWORD=[mongo_password] \
-e UNITE_ENSEMBL_DATA_HOST=data.ensembl.unite.net \
-e UNITE_ENSEMBL_VEP_HOST=vep.ensembl.unite.net \
-e UNITE_GENES_INDEXING_BUCKET_SIZE=100 \
-e UNITE_SM_ANNOTATION_BUCKET_SIZE=100 \
-e UNITE_SM_INDEXING_BUCKET_SIZE=300 \
-e UNITE_CNV_ANNOTATION_BUCKET_SIZE=10 \
-e UNITE_CNV_INDEXING_BUCKET_SIZE=100 \
-e UNITE_SV_ANNOTATION_BUCKET_SIZE=10 \
-e UNITE_SV_INDEXING_BUCKET_SIZE=100 \
-e UNITE_GENOME_BUILD=GRCh37
-d \
unite.omics.feed:latest
```
