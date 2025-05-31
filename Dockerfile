# Use a imagem base do .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Instalar Tesseract OCR e suas dependências
RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    libtesseract-dev \
    && rm -rf /var/lib/apt/lists/*

# Criar diretório para os arquivos de treinamento do Tesseract
RUN mkdir -p /usr/share/tesseract-ocr/4.00/tessdata

# Copiar os arquivos de treinamento do português
COPY Source/ExAnFlow.Api/tessdata/por.traineddata /usr/share/tesseract-ocr/4.00/tessdata/

# Definir variável de ambiente para o Tesseract
ENV TESSDATA_PREFIX=/usr/share/tesseract-ocr/4.00/tessdata
ENV ASPNETCORE_URLS=http://+:8080

# Imagem de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Source/ExAnFlow.Api/ExAnFlow.Api.csproj", "Source/ExAnFlow.Api/"]
COPY ["Source/Modules/", "Source/Modules/"]
RUN dotnet restore "Source/ExAnFlow.Api/ExAnFlow.Api.csproj"
COPY . .
WORKDIR "/src/Source/ExAnFlow.Api"
RUN dotnet build "ExAnFlow.Api.csproj" -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish "ExAnFlow.Api.csproj" -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExAnFlow.Api.dll"] 