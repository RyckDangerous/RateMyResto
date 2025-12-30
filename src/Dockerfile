# Étape de build multi-stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS buildapp
WORKDIR /src

# Copie du code source et restauration des dépendances
COPY RateMyResto/RateMyResto/ RateMyResto/
RUN dotnet restore "RateMyResto/RateMyResto.csproj"

# Publication vers un dossier dédié /publish
RUN mkdir /publish
RUN dotnet publish "RateMyResto/RateMyResto.csproj" -c Release -o /publish

# Image finale runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# Crée le dossier img
RUN mkdir -p img

# Copie des artefacts publiés depuis l'étape buildapp
COPY --from=buildapp /publish .

# Configuration du port
ENV ASPNETCORE_URLS=http://+:8080

# Variables d'environnement pour SQL Server
ENV ENVRATE_ConnectionStrings__Server=""
ENV ENVRATE_ConnectionStrings__Login=""
ENV ENVRATE_ConnectionStrings__Password=""

EXPOSE 8080

# Démarrage
ENTRYPOINT ["dotnet", "RateMyResto.dll"]