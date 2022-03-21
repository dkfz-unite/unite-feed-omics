# Genome Data Feed Service

## General
Genome data feed service provides the following functionality:
- [Genome data feed web API](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Docs/api-genome.md) - REST API for uploading sequencing data to the portal (including input data validation).
- Genes data indexing service - background service responsible for gene-centric data index creation.
- Mutations data annotation service (_requires internet access_) - background service responsible for annotation of mutations, genes and transcripts.
  - Mutations are annotated with local installation of Ensembl VEP, as they might be treated as personal information.
  - Genes and transcripts are annotated with public Ensembl web API.
- Mutations data indexing service - background service responsible for mutation-centric data index creation.

Genome data feed service is written in ASP.NET (.NET 5)

## Dependencies
- [SQL](https://github.com/dkfz-unite/unite-environment/tree/main/programs/postgresql) - SQL server with domain data and user identity data.
- [Elasticsearch](https://github.com/dkfz-unite/unite-environment/tree/main/programs/elasticsearch) - Elasticsearch server with indices of domain data.
- [Ensembl(VEP)](https://github.com/dkfz-unite/unite-environment/tree/main/applications/unite-vep) - Local installation of Ensembl VEP service.

## Access
Environment|Address|Port
-----------|-------|----
Host|http://localhost:5106|5106
Docker|http://feed.genome.unite.net|80

## Configuration
To configure the application, change environment variables in either docker or [launchSettings.json](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Unite.Genome.Feed.Web/Properties/launchSettings.json) file (if running locally):
Variable|Description|Default(Local)|Default(Docker)
--------|-----------|--------------|---------------
ASPNETCORE_ENVIRONMENT|ASP.NET environment|Debug|Release
UNITE_SQL_HOST|SQL server host|localhost|sql.unite.net
UNITE_SQL_PORT|SQL server port|5432|5432
UNITE_SQL_USER|SQL server user||
UNITE_SQL_PASSWORD|SQL server password||
UNITE_ELASTIC_HOST|ES service host|http://localhost:9200|es.unite.net:9200
UNITE_ELASTIC_USER|ES service user||
UNITE_ELASTIC_PASSWORD|ES service password||
UNITE_VEP_HOST|Local Ensembl VEP host|http://localhost:5200|ensembl.unite.net|
UNITE_ENSEMBL_HOST|Public Ensembl host|https://grch37.rest.ensembl.org|...|
UNITE_GENES_INDEXING_INTERVAL|Genes indexing interval (seconds)|10|
UNITE_GENES_INDEXING_BUCKET_SIZE|Genes indexing bucket size|300|
UNITE_MUTATIONS_ANNOTATION_INTERVAL|Mutations annotation interval (seconds)|10|
UNITE_MUTATIONS_ANNOTATION_BUCKET_SIZE|Mutations annotation bucket size|100|
UNITE_MUTATIONS_INDEXING_INTERVAL|Mutations indexing interval (seconds)|10|
UNITE_MUTATIONS_INDEXING_BUCKET_SIZE|Mutations indexing bucket size|300|

## Installation

### Docker Compose
The easiest way to install the application is to use docker-compose:
- Environment configuration and installation scripts: https://github.com/dkfz-unite/unite-environment
- Genome data feed service configuration and installation scripts: https://github.com/dkfz-unite/unite-environment/tree/main/applications/unite-genome-feed

### Docker
[Dockerfile](https://github.com/dkfz-unite/unite-genome-feed/blob/main/Dockerfile) is used to build an image of the application.
To build an image run the following command:
```
docker build -t unite.genome.feed:latest .
```

All application components should run in the same docker network.
To create common docker network if not yet available run the following command:
```bash
docker network create unite
```

To run application in docker run the following command:
```bash
docker run \
--name unite.genome.feed \
--restart unless-stopped \
--net unite \
--net-alias feed.genome.unite.net \
-p 127.0.0.1:5100:80 \
-e ASPNETCORE_ENVIRONMENT=Release \
-e UNITE_ELASTIC_HOST=http://es.unite.net:9200 \
-e UNITE_ELASTIC_USER=[elasticsearch_user] \
-e UNITE_ELASTIC_PASSWORD=[elasticsearch_password] \
-e UNITE_SQL_HOST=sql.unite.net \
-e UNITE_SQL_PORT=5432 \
-e UNITE_SQL_USER=[sql_user] \
-e UNITE_SQL_PASSWORD=[sql_password] \
-e UNITE_VEP_HOST=ensembl.unite.net \
-e UNITE_ENSEMBL_HOST=https://grch37.rest.ensembl.org \
-e UNITE_GENES_INDEXING_INTERVAL=10 \
-e UNITE_GENES_INDEXING_BUCKET_SIZE=300 \
-e UNITE_MUTATIONS_ANNOTATION_INTERVAL=10 \
-e UNITE_MUTATIONS_ANNOTATION_BUCKET_SIZE=100 \
-e UNITE_MUTATIONS_INDEXING_INTERVAL=10 \
-e UNITE_MUTATIONS_INDEXING_BUCKET_SIZE=300 \
-d \
unite.genome.feed:latest
```
