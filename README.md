# Order Manager

## Event Sourcing demo project using Marten and Wolverine

This is a demo order management project based on the event sourcing architecture

Backend is built on .Net using Marten and Wolverine packages.  
Microservices messaging is handled by RabbitMQ  
Frontend is using React with React Router  
Authorization logic is handled by Keycloak

## Working with the Code

Before getting started, you will need the following:
### 1. .NET 8.0 SDK
Download link: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
### 2. Docker 
You can use Docker Desktop: https://www.docker.com/products/docker-desktop/

### Docker compose (Recommended)
1. Generate a trusted dev certificate: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs  
You can use the following command: ``dotnet dev-certs https --trust -ep aspnetapp.pfx -p P@ssw0rd``
2. Replace a default path (``E:/Dev/Certs/https``) to the certificate inside a docker-compose.yml file
2. ``cd`` into the root folder, where ```.slnx``` is located
3. Execute ``docker compose -f .\deploy\docker-compose.yml up``

### Manual (Not recommended)
1. Launch required services: ``Keycloak``, ``Prostgres`` and ``RabbitMQ`` (Using configuration in ``docker-compose.yml``)
2. Launch .Net services: ``OrderManager.WriteModel.Api`` and ``OrderManager.ReadModel.Api``. Pay attention to ``launchSettings.json``
3. Launch Vite React project