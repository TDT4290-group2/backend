# HealthTech backend

This repository contains the backend code for the HealthTech application, which gives workers in the industrial sector access to exposure data for a range of exposure types.

## Table of Contents
- [HealthTech backend](#healthtech-backend)
  - [Table of Contents](#table-of-contents)
  - [Requirements](#requirements)
  - [Setup](#setup)
    - [Running backend in Docker](#running-backend-in-docker)
    - [Running backend outside Docker](#running-backend-outside-docker)
    - [Seeding the Database with sample data](#seeding-the-database-with-sample-data)
  - [Running tests](#running-tests)
  - [Endpoints](#endpoints)
  - [Hypertables](#hypertables)
    - [Making a new hypertable](#making-a-new-hypertable)
    - [Verifying the hypertable exists](#verifying-the-hypertable-exists)


## Requirements
This project requires .NET 9.0.x for development

## Setup
The are two ways of running the backend; in docker or outside docker. The simplest is in docker. For development, it is recommended to run outside docker. **No matter which approach, you still need docker installed on your machine to run the database.**

### Running backend in Docker

#### Set up environment variables
Make a copy of the [.env.example](./.env.example) file and rename the new file to *.env*. Remember to change the default password.

#### Run the compose stack
Run this command in the terminal from the root directory
```sh
docker compose up --build -d
```

Now you can [seed the database](#seeding-the-database-with-sample-data)

### Running backend outside Docker
NB: Requires .NET 9.0.x installed locally

You can run the application without docker by commenting out this section in [the docker-compose file](./docker-compose.yml)
```
  dotnet_app:
    build:
      context: ./src/Backend
      dockerfile: Dockerfile
    ports:
      - "${DOTNET_APP_PORT:-5063}:8080"
    environment:
      ConnectionStrings__DefaultConnection: "Host=timescaledb;Port=${POSTGRES_PORT:-5432};Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"
      AllowedHost: ${ALLOWED_HOST}
    container_name: dotnet_app
    depends_on:
      timescaledb:
        condition: service_healthy
```

Then start the rest of the compose stack from the root directory with
```sh
docker compose up --build -d
```

Lastly, navigate to *src/Backend/* and run
```sh
dotnet run
```

or if you want to enable hot reload run
```sh
dotnet watch run
```

Now you can [seed the database](#seeding-the-database-with-sample-data)

### Seeding the Database with sample data
First, place your sample data in the *seed* directory as csv-files: *NoiseData.csv*, *DustData.csv*, *VibrationData.csv*
Then run 

```sh
docker exec -it timescaledb psql -U postgres -d mydb -f /seed/seed.sql
```

Now your application is ready! You can open it at http://localhost:8080

## Running tests
From the root directory, run 
```sh
dotnet test
```

## Endpoints
The default backend base URL is
```
http://localhost:5063
```

## Hypertables

### Making a new hypertable
After creating the migration for the new table, you have to manually add this SQL script to the migration file. It needs to be placed in the *Up* method. See an example of this in the InitialCreate.cs file.

```cs
migrationBuilder.Sql(
    "SELECT create_hypertable('\"<TableName>\"', by_range('Time', INTERVAL '1 day'));"
);
```
Change \<TableName\> to the name of your new table. It is also possible to do range partitioning on another property than *Time*, but this will likely be most used. Remember to have the range partitioning property (in this case *Time*) as a part of the primary key for the table.

### Verifying the hypertable exists
To verify that the hypertables have been set up, one can connect to the database from within the container with:

```sh
docker exec -it timescaledb psql -U postgres -d mydb
```
and then running
```
SELECT * FROM _timescaledb_catalog.hypertable;
```

Check that all the correct *table_name* entries are there for your tables

