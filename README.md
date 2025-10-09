# HealthTech backend

This repository contains the backend code for the HealthTech application, which gives workers in the industrial sector access to exposure data for a range of exposure types.

## Table of Contents
- [HealthTech backend](#healthtech-backend)
  - [Table of Contents](#table-of-contents)
  - [Requirements](#requirements)
  - [Setting up the backend and DB](#setting-up-the-backend-and-db)
    - [Locally](#locally)
    - [Docker](#docker)
  - [Updating the DB](#updating-the-db)
  - [Setting up docker](#setting-up-docker)
  - [Seeding the Database with sample data](#seeding-the-database-with-sample-data)
  - [Running tests](#running-tests)
  - [Endpoints](#endpoints)
  - [Hypertables](#hypertables)
    - [Making a new hypertable](#making-a-new-hypertable)
    - [Verifying the hypertable exists](#verifying-the-hypertable-exists)


## Requirements
This project requires .NET 9.0.x


## Setting up the backend and DB
### Locally
You can either run the application locally by commenting out this section in docker-compose.yml
```
dotnet_app:
    build:
      context: ./src/Backend
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      ConnectionStrings__DefaultConnection: "Host=timescaledb;Port=5432;Database=mydb;Username=postgres;Password=postgres"
    container_name: dotnet_app
    depends_on:
      - timescaledb

```
and then set up docker by following the steps in [setting up docker](#setting-up-docker).

Then navigate to /src/Backend and run this if the **Migrations folder doesn´t exist**
```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If the **Migrations folder exist** run this
```sh
dotnet ef database update
```

Then you have to run
```sh
dotnet run
```

or if you want to enable hot reload run
```sh
dotnet watch run
```

### Docker
Run this command in the terminal from the root
```sh
docker compose up --build -d
```

Then navigate to /src/Backend and run this if the **Migrations folder doesn´t exist**
```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If the **Migrations folder exist** run this
```sh
dotnet ef database update
```

## Updating the DB
If you add new tables or change an already existing table you have to run this command
```sh
dotnet ef migrations add <name of migration>
dotnet ef database update
```

## Setting up docker
Run the command under in the terminal from root:
```sh
docker compose up --build -d
```

## Seeding the Database with sample data
First, place your sample data in the *seed* directory as csv-files: *NoiseData.csv*, *DustData.csv*, *VibrationData.csv*
Then run 

```sh
docker exec -it timescaledb psql -U postgres -d mydb -f /seed/seed.sql
```

## Running tests
From the root directory, run 
```sh
dotnet test
```

## Endpoints
The endpoint is
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

