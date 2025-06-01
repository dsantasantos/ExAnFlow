# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# --- ESTÁGIO BASE ---
# Este estágio é usado quando rodando do VS em modo rápido (Default para configuração Debug)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# --- ATENÇÃO AQUI: Temporariamente mudar para root para instalar pacotes ---
USER root

# Install libs for Tesseract
RUN apt-get update
RUN apt-get install -y git cmake build-essential
RUN mkdir leptonica
RUN git clone https://github.com/DanBloomberg/leptonica.git /leptonica

WORKDIR /leptonica
RUN mkdir build
WORKDIR /leptonica/build
RUN cmake ..

RUN apt-get install -y libleptonica-dev libtesseract-dev

# --- ATENÇÃO AQUI: Voltar para o usuário não-root para a execução da aplicação ---
USER $APP_UID

# --- ESTÁGIO DE BUILD ---
# Este estágio é usado para construir o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos de projeto .csproj e restaura as dependências
# Isso aproveita o cache do Docker para restaurar as dependências apenas quando o .csproj muda
COPY ["Source/ExAnFlow.Api/ExAnFlow.Api.csproj", "Source/ExAnFlow.Api/"]
RUN dotnet restore "./Source/ExAnFlow.Api/ExAnFlow.Api.csproj"

# Copia o restante dos arquivos da sua aplicação
COPY . .
WORKDIR "/src/Source/ExAnFlow.Api"

# Constrói a aplicação. Se você adicionou o ItemGroup no .csproj,
# ele já vai incluir as .so files no diretório de build.
RUN dotnet build "./ExAnFlow.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- ESTÁGIO DE PUBLICAÇÃO ---
# Este estágio é usado para publicar o projeto de serviço para ser copiado para o estágio final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Publica a aplicação. O bloco no .csproj irá garantir que as .so files
# sejam copiadas para o diretório de publicação (e.g., /app/publish/x64).
RUN dotnet publish "./ExAnFlow.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- ESTÁGIO FINAL (PRODUÇÃO / MODO REGULAR) ---
# Este estágio é usado em produção ou quando rodando do VS em modo regular (Default quando não usando a configuração Debug)
FROM base AS final
# Link libs for Tesseract
WORKDIR /app/x64
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 libleptonica-1.80.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 libtesseract41.so
WORKDIR /app
# Copia os arquivos publicados do estágio de 'publish' para o estágio final.
# Isso inclui sua aplicação, e as bibliotecas .so (que o csproj copiou para /app/publish/x64).
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ExAnFlow.Api.dll"]