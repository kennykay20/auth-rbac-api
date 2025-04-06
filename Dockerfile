# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Final stage: use base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5284

ENTRYPOINT [ "dotnet", "crud-api.dll" ]
