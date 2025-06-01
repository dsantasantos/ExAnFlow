# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["ExAnFlow.sln", "./"]
COPY ["Source/ExAnFlow.Api/ExAnFlow.Api.csproj", "Source/ExAnFlow.Api/"]
COPY ["Source/Modules/*/*.csproj", "Source/Modules/"]

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "ExAnFlow.sln" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ExAnFlow.sln" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "ExAnFlow.Api.dll"] 