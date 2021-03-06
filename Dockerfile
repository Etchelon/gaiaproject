FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY Backend/Endpoint/Endpoint.csproj Backend/Endpoint/
COPY Backend/Libraries/Common/Common.csproj Backend/Libraries/Common/
COPY Backend/Libraries/Engine/Engine.csproj Backend/Libraries/Engine/
COPY Backend/Libraries/MongoDbGenericRepository/MongoDbGenericRepository.csproj Backend/Libraries/MongoDbGenericRepository/
COPY Backend/Libraries/ViewModels/ViewModels.csproj Backend/Libraries/ViewModels/
RUN dotnet restore Backend/Endpoint/Endpoint.csproj

COPY . ./
WORKDIR /app/Backend/Endpoint
RUN dotnet publish -c Release -o ../../out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "GaiaProject.Endpoint.dll"]
