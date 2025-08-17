## AspnetCore Microservice

## Prepare environment

* Install dotnet core version in file `global.json`
* IDE: Visual Studio 2022+, Rider, Visual Studio Code
* Docker Desktop
* EF Core tools reference (.NET CLI):

```Powershell
dotnet tool install --global dotnet-ef
```

---

## How to run the project

Run command for build project

```Powershell
dotnet build
```

Go to folder contain file `docker-compose`

1. Using docker-compose

```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

## Application URLs - LOCAL Environment (Docker Container):


- Product API: http://localhost:6002/api/products


## Docker Application URLs - LOCAL Environment (Docker Container):



2. Using Visual Studio 2022

- Open aspnetcore-microservices.sln - `aspnetcore-microservices.sln`
- Run Compound to start multi projects

---

## Application URLs - DEVELOPMENT Environment:

- Product API: http://localhost:5002/api/products


---
