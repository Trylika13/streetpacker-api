# 1. Étape de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copier les fichiers de la solution et TOUS les .csproj
COPY *.sln ./
COPY SP.Core/*.csproj ./SP.Core/
COPY SP.Domain/*.csproj ./SP.Domain/
COPY SP.Infrastructure/*.csproj ./SP.Infrastructure/
COPY SP.Presentation/*.csproj ./SP.Presentation/

# Restaurer les packages NuGet
RUN dotnet restore

# Copier tout le reste du code et compiler
COPY . ./
RUN dotnet publish SP.Presentation/SP.Presentation.csproj -c Release -o out

# 2. Étape d'exécution (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configurer ASP.NET Core pour écouter sur le port par défaut de Render
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "SP.Presentation.dll"]