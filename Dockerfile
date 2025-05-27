# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY *.sln .
COPY Source/*/*.csproj ./Source/tmp/
RUN for file in $(ls ./Source/tmp/*.csproj); do mkdir -p $(echo $file | sed 's/tmp\///g' | sed 's/\/[^\/]*$//g') ; mv $file $(echo $file | sed 's/tmp\///g') ; done && rm -rf ./Source/tmp
RUN dotnet restore

# Copy the rest of the source code
COPY Source/. ./Source/

# Build the application
WORKDIR /app/Source/ExAnFlow.Api
RUN dotnet publish -c Release -o /app/out

# Use the .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port the application runs on
EXPOSE 8080

# Set the entry point to run the API
ENTRYPOINT ["dotnet", "ExAnFlow.Api.dll"]
