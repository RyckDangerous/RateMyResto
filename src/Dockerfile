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
ENV ENVRATE_AdminAccount__Password=""

# Variables d'environnement pour le serveur SMTP (feature Mailing)
ENV ENVRATE_Smtp__Host=""
ENV ENVRATE_Smtp__Port="587"
ENV ENVRATE_Smtp__From=""

# URL publique de l'application — utilisée dans les liens des emails
ENV ENVRATE_Mailing__AppBaseUrl=""

EXPOSE 8080

# Démarrage
# - Par défaut (CMD "--mode=web") : lance le serveur web Blazor.
# - Pour exécuter une commande CLI (ex: en CRON), surcharger le CMD :
#     docker run --rm image --mode=cli --reminder
ENTRYPOINT ["dotnet", "RateMyResto.dll"]
CMD ["--mode=web"]