################################################################## ngnix +loadbalancer
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Api/*.csproj ./
RUN dotnet restore

# Copy everything * and build
COPY . ./
RUN dotnet publish ./Api/Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "Api.dll"]

