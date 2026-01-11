FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.22 AS build
WORKDIR /app

COPY *.sln ./
COPY src/WebApi/WebApi.csproj src/WebApi/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/

RUN dotnet restore src/WebApi/WebApi.csproj

COPY src/ src/

RUN dotnet publish src/WebApi/WebApi.csproj \
    -c Release \
    -r linux-musl-x64 \
    --self-contained false \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.22 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
