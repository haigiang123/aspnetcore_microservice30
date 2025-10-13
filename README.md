# AspnetCore Microservice

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

 ### 1. Using docker-compose

Start all container (consume high memory)
```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```
Stop all container
```Powershell
docker-compose -f docke-compose.yml -f docker-compose.override.yml down
```
Stop specific containers
```Powershell
docker-compose stop container1 container2
```
Build, start and deattach specific containers
```Powershell
docker-compose up -d --build container1 container2
```
Run redis-cli
```Powershell
docker-compose exec baskerdb redis-cli
```

#### 1.1 Application URLs - LOCAL Environment (Docker Container):


- Product API: http://localhost:6002/api/products
- Customer API: http://localhost:6003/api/products
- Basket API: http://localhost:6004/swagger/index.html

#### 1.2 Docker Application URLs - LOCAL Environment (Docker Container):

---

### 2. Using Visual Studio 2022 

- Open aspnetcore-microservices.sln - `aspnetcore-microservices.sln`
- Run Compound to start multi projects



#### 2.1 Application URLs - DEVELOPMENT Environment:

- Product API: http://localhost:5002/api/products
- Customer API: http://localhost:5003/api/products
- Basket API: http://localhost:5288/swagger/index.html



