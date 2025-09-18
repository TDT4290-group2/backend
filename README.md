# HealthTech backend

This repository contains the backend code for the HealthTech application, which gives workers in the industrial sector access to exposure data for a range of exposure types.


## Requirements
This project requires .NET 9.0.x


## Running the backend and DB
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

Then navigate to /src/Backend and run 
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
docker compose up -d
```

## Setting up docker
Run the command under in the terminal from root:
```sh
docker compose up -d
```

## Running tests
From the root directory, run 
```sh
dotnet test
```