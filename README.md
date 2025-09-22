# HealthTech backend

This repository contains the backend code for the HealthTech application, which gives workers in the industrial sector access to exposure data for a range of exposure types.

## Table of Contents
- [Requirements](#requirements)
- [Setting up the backend and DB](#setting-up-the-backend-and-db)
  - [Locally](#locally)
  - [Docker](#docker)
- [Updating the DB](#updating-the-db)
- [Setting up docker](#setting-up-docker)
- [Running tests](#running-tests)
- [Endpoints](#endpoints)
  - [Docker](#docker-1)
  - [Locally](#locally-1)


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

## Running tests
From the root directory, run 
```sh
dotnet test
```

## Endpoints
### Docker
When your running your application though docker the endpoint starts with
```
http://localhost:8080
```

### Locally
When your running you applciation locally the endpoint starts with
```
http://localhost:5063
```