# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["ExAnFlow.sln", "./"]
COPY ["Source/ExAnFlow.Api/ExAnFlow.Api.csproj", "Source/ExAnFlow.Api/"]
COPY ["Source/Modules/Ocr/ExAnFlow.Ocr.Application/ExAnFlow.Ocr.Application.csproj", "Source/Modules/Ocr/ExAnFlow.Ocr.Application/"]
COPY ["Source/Modules/Ocr/ExAnFlow.Ocr.Domain/ExAnFlow.Ocr.Domain.csproj", "Source/Modules/Ocr/ExAnFlow.Ocr.Domain/"]
COPY ["Source/Modules/Ocr/ExAnFlow.Ocr.Infrastruture/ExAnFlow.Ocr.Infrastruture.csproj", "Source/Modules/Ocr/ExAnFlow.Ocr.Infrastruture/"]

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

# Instalar Tesseract OCR e suas dependências
# A flag --no-install-recommends é usada para evitar a instalação de pacotes não essenciais.
# tesseract-ocr-eng é para o idioma inglês. Se precisar de outros idiomas, adicione-os aqui.
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    tesseract-ocr \
    libleptonica-dev \
    tesseract-ocr-eng && \
    rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "ExAnFlow.Api.dll"]