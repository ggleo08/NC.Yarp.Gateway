# Yarp.Gateway.Demo

##### Introductions

This sample is for learning Yarp、Dapr.

##### Deponds on

- Yarp  Reverse Proxy
- Dapr
- EntityFrameworkCore 
- SqlServer

##### Test Url

- `http://localhost:5200/api/first/WeatherForecast` - will get 401 error, this service  enabled AuthorizationPolicy  
- `http://localhost:5200/api/second/WeatherForecast`


##### Sample Commands

- run app in selfhost with dapr
  - `dapr run --app-id gateway --dapr-http-port 3500 --app-port 5200 -- dotnet run`
- docker: build image 
  -  `docker build -t second-service -f Dockerfile-second --rm .`
- docker: container network connect
  - `docker network connect c-dapr-network mssql_server`
- docker: container inspect
  - `docker container inspect dapr_redis`
##### TODO

- Abp.vnext Framework
- Management Dashborad
##### Thanks

​    @[JaneConan](https://github.com/JaneConan/reverse-proxy/commit/3f2f30aac902e9caa593c25ed69ced2109c75172)  、 @[fanslead](https://github.com/fanslead/ReverseProxy.Store)、@[geffzhang](https://github.com/geffzhang/TyeAndYarp)

