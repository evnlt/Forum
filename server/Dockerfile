FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /app

COPY src/Forum.WebApi ./src/Forum.WebApi

COPY Forum.sln .

RUN dotnet restore
RUN dotnet build --no-restore --configuration Release
RUN dotnet publish --no-build --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Staging
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Forum.WebApi.dll"]