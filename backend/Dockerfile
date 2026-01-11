FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.22 AS build
WORKDIR /app

COPY *.sln ./

COPY src/WebApi/WebApi.csproj src/WebApi/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/

RUN dotnet restore src/WebApi/WebApi.csproj

COPY src/ src/

RUN dotnet publish src/WebApi/WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.22 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "WebApi.dll"]
