# Guide : Application ASP.NET / Blazor en mode dual Web + CLI

> **Destination** : Ce document est destiné à une instance Claude chargée de mettre en place
> le même pattern de démarrage dual (web / CLI) dans un autre projet .NET.
> Il décrit précisément l'architecture, les fichiers à créer, et comment les assembler.

---

## Vue d'ensemble du concept

L'application est un **binaire unique** qui peut démarrer de deux façons :

| Mode | Démarrage | Comportement |
|------|-----------|--------------|
| `web` | `dotnet App.dll --mode=web` (ou aucun argument) | Lance le serveur HTTP / Blazor normalement |
| `cli` | `dotnet App.dll --mode=cli --ma-commande` | Exécute le traitement demandé, puis **s'arrête proprement** |

En mode CLI, le pipeline HTTP n'est jamais démarré. L'application initialise quand même
toute l'infrastructure (DI, base de données, migrations…), exécute le traitement,
puis retourne. C'est la même image Docker, le même binaire : seule la commande de démarrage
diffère.

---

## Structure des fichiers à créer

```
Features/
└── Shared/
    └── Cli/
        ├── StartupMode.cs      ← enum : Web | Cli
        ├── CliCommand.cs       ← enum des sous-commandes disponibles
        └── StartupOptions.cs   ← record avec parsing des args
```

Ces fichiers appartiennent à l'espace de noms partagé car ils concernent le point d'entrée,
pas une feature métier.

---

## Étape 1 — Créer `StartupMode.cs`

Enum simple à deux valeurs.

```csharp
namespace MonApp.Features.Shared.Cli;

/// <summary>
/// Mode de démarrage de l'application.
/// </summary>
public enum StartupMode
{
    /// <summary>
    /// Mode par défaut : lancement du serveur web.
    /// </summary>
    Web,

    /// <summary>
    /// Mode ligne de commande : exécution d'une sous-commande puis arrêt.
    /// </summary>
    Cli
}
```

---

## Étape 2 — Créer `CliCommand.cs`

Enum des sous-commandes disponibles en mode CLI.
Ajouter une valeur ici à chaque nouvelle commande CLI.

```csharp
namespace MonApp.Features.Shared.Cli;

/// <summary>
/// Sous-commandes disponibles en mode CLI.
/// </summary>
public enum CliCommand
{
    /// <summary>
    /// Aucune sous-commande fournie.
    /// </summary>
    None,

    /// <summary>
    /// Description de ce que fait cette commande.
    /// Exemple : envoi de rappels par email.
    /// </summary>
    MaCommande
}
```

> **Convention de nommage** : chaque valeur de l'enum correspond à un argument CLI
> du type `--ma-commande` (tirets dans l'argument, PascalCase dans l'enum).

---

## Étape 3 — Créer `StartupOptions.cs`

C'est le cœur du mécanisme. Ce `record sealed` expose une méthode `Parse(string[] args)`
statique qui transforme les arguments de la ligne de commande en options typées.

```csharp
namespace MonApp.Features.Shared.Cli;

/// <summary>
/// Options de démarrage déduites de la ligne de commande.
/// Le mode par défaut (aucun argument) est <see cref="StartupMode.Web"/>.
/// </summary>
public sealed record StartupOptions
{
    private const string ModeArgumentPrefix = "--mode=";
    private const string MaCommandeArgument = "--ma-commande";
    private const string ModeValueWeb       = "web";
    private const string ModeValueCli       = "cli";

    /// <summary>Mode de démarrage.</summary>
    public required StartupMode Mode { get; init; }

    /// <summary>Sous-commande CLI à exécuter.</summary>
    public required CliCommand Command { get; init; }

    /// <summary>Message d'erreur si le parsing a échoué. Null si tout est valide.</summary>
    public string? ParsingError { get; init; }

    /// <summary>Indique si les options sont valides.</summary>
    public bool IsValid => ParsingError is null;

    /// <summary>
    /// Parse les arguments de la ligne de commande.
    /// Aucun argument → mode Web (défaut).
    /// Formats reconnus : --mode=web | --mode=cli [--ma-commande]
    /// </summary>
    /// <param name="args">Arguments du point d'entrée.</param>
    /// <returns>Options parsées. Vérifier <see cref="IsValid"/> avant usage.</returns>
    public static StartupOptions Parse(string[] args)
    {
        if (args is null || args.Length is 0)
        {
            return new StartupOptions
            {
                Mode    = StartupMode.Web,
                Command = CliCommand.None
            };
        }

        StartupMode? parsedMode    = null;
        CliCommand   parsedCommand = CliCommand.None;

        foreach (string arg in args)
        {
            if (arg.StartsWith(ModeArgumentPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string value = arg[ModeArgumentPrefix.Length..];

                if (string.Equals(value, ModeValueWeb, StringComparison.OrdinalIgnoreCase))
                    parsedMode = StartupMode.Web;
                else if (string.Equals(value, ModeValueCli, StringComparison.OrdinalIgnoreCase))
                    parsedMode = StartupMode.Cli;
                else
                    return Invalid($"Valeur de --mode inconnue : '{value}'. Attendu : web | cli.");

                continue;
            }

            if (string.Equals(arg, MaCommandeArgument, StringComparison.OrdinalIgnoreCase))
            {
                parsedCommand = CliCommand.MaCommande;
                continue;
            }

            return Invalid($"Argument inconnu : '{arg}'.");
        }

        StartupMode effectiveMode = parsedMode ?? StartupMode.Web;

        if (effectiveMode is StartupMode.Web)
        {
            return new StartupOptions
            {
                Mode    = StartupMode.Web,
                Command = CliCommand.None
            };
        }

        if (parsedCommand is CliCommand.None)
            return Invalid("Le mode CLI nécessite une sous-commande (ex: --ma-commande).");

        return new StartupOptions
        {
            Mode    = StartupMode.Cli,
            Command = parsedCommand
        };
    }

    private static StartupOptions Invalid(string error)
    {
        return new StartupOptions
        {
            Mode         = StartupMode.Web,
            Command      = CliCommand.None,
            ParsingError = error
        };
    }
}
```

### Règles de parsing

- **Aucun argument** → mode `Web`, `Command = None` (comportement par défaut Docker)
- `--mode=web` → idem
- `--mode=cli --ma-commande` → mode `Cli`, `Command = MaCommande`
- `--mode=cli` seul sans sous-commande → `IsValid = false`, message d'erreur
- Argument inconnu → `IsValid = false`, message d'erreur

---

## Étape 4 — Créer le service CLI

Chaque sous-commande est implémentée par un service dédié, enregistré dans le conteneur DI.

```csharp
// Interface
public interface IMaCommandeService
{
    /// <summary>
    /// Exécute le traitement CLI.
    /// </summary>
    /// <returns>Nombre d'éléments traités (utile pour les logs).</returns>
    Task<int> ExecuterAsync();
}

// Implémentation
public sealed class MaCommandeService : IMaCommandeService
{
    private readonly ILogger<MaCommandeService> _logger;
    // … autres dépendances injectées

    public MaCommandeService(ILogger<MaCommandeService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> ExecuterAsync()
    {
        _logger.LogInformation("Début du traitement CLI MaCommande");

        // logique métier ici…

        _logger.LogInformation("Traitement terminé");
        return 0;
    }
}
```

Enregistrer le service dans la configuration DI de la feature concernée :

```csharp
services.AddScoped<IMaCommandeService, MaCommandeService>();
```

---

## Étape 5 — Modifier `Program.cs`

C'est ici que tout s'assemble. La structure générale est :

```
1. Parse des arguments
2. Validation (arrêt si invalide)
3. Construction de l'application (builder, DI, pipeline HTTP)
4. Migrations de base de données (toujours, quel que soit le mode)
5. Branchement selon le mode :
   - Mode CLI → exécuter la commande, puis return
   - Mode Web → démarrer le serveur HTTP
```

```csharp
using MonApp.Features.Shared.Cli;

ILogger? logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<Program>();

try
{
    // ── 1. Parse ──────────────────────────────────────────────────────────────
    StartupOptions startupOptions = StartupOptions.Parse(args);

    if (!startupOptions.IsValid)
    {
        logger?.LogCritical("Arguments invalides : {Error}", startupOptions.ParsingError);
        return;
    }

    logger?.LogInformation("Mode de démarrage : {Mode}", startupOptions.Mode);

    // ── 2. Construction de l'application ──────────────────────────────────────
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // … enregistrement des services (DI), configuration, etc.
    // Enregistrer TOUS les services ici, y compris ceux utilisés en mode CLI.
    // Le mode CLI a besoin de la même infrastructure que le mode Web
    // (base de données, repositories, services…).

    WebApplication app = builder.Build();

    // ── 3. Migrations (toujours, quel que soit le mode) ───────────────────────
    using (IServiceScope scope = app.Services.CreateScope())
    {
        // EF Core migrations, DbUp, création du compte admin, etc.
    }

    // ── 4. Branchement selon le mode ──────────────────────────────────────────
    if (startupOptions.Mode is StartupMode.Cli)
    {
        using IServiceScope cliScope = app.Services.CreateScope();

        switch (startupOptions.Command)
        {
            case CliCommand.MaCommande:
                IMaCommandeService service =
                    cliScope.ServiceProvider.GetRequiredService<IMaCommandeService>();
                int count = await service.ExecuterAsync();
                logger?.LogInformation("CLI terminée. Éléments traités : {Count}", count);
                break;

            default:
                logger?.LogCritical("Sous-commande CLI inconnue : {Command}", startupOptions.Command);
                break;
        }

        return; // ← l'application s'arrête ici en mode CLI
    }

    // ── 5. Mode Web : pipeline HTTP ───────────────────────────────────────────
    // Configure pipeline, middlewares, Razor, etc.
    app.UseAntiforgery();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    // …

    await app.RunAsync();
}
catch (Exception ex)
{
    logger?.LogCritical(ex, "MAIN Exception - Stopped program because of exception");
}
```

### Points clés de `Program.cs`

- **`return` après le bloc CLI** : c'est ce qui arrête proprement l'application.
  Sans ce `return`, le code continuerait vers `app.RunAsync()`.
- **Migrations avant le branchement** : le mode CLI a besoin de la base de données à jour,
  exactement comme le mode web.
- **`using IServiceScope cliScope`** : créer un scope dédié pour le mode CLI
  afin que les services `Scoped` soient correctement instanciés et libérés.
- **`switch` sur `startupOptions.Command`** : centralise le dispatch des commandes.
  Ajouter un `case` ici à chaque nouvelle commande CLI.

---

## Étape 6 — Configurer le Dockerfile

La configuration Docker utilise deux instructions distinctes :
`ENTRYPOINT` (fixe, ne change pas) et `CMD` (remplaçable au `docker run`).

```dockerfile
# Étape de build multi-stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS buildapp
WORKDIR /src

COPY MonApp/ MonApp/
RUN dotnet restore "MonApp/MonApp.csproj"

RUN mkdir /publish
RUN dotnet publish "MonApp/MonApp.csproj" -c Release -o /publish

# Image finale runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=buildapp /publish .

# Port du serveur web
ENV ASPNETCORE_URLS=http://+:8080

# Variables d'environnement de l'application
ENV MONAPP_ConnectionStrings__Server=""
ENV MONAPP_ConnectionStrings__Login=""
# … autres variables

EXPOSE 8080

# ENTRYPOINT est fixe : on appelle toujours dotnet MonApp.dll
# CMD est l'argument par défaut, surchargeable au docker run
ENTRYPOINT ["dotnet", "MonApp.dll"]
CMD ["--mode=web"]
```

### Pourquoi `ENTRYPOINT` + `CMD` et pas `CMD` seul ?

Avec `ENTRYPOINT ["dotnet", "MonApp.dll"]` et `CMD ["--mode=web"]` :

```bash
# Mode web (défaut — CMD utilisé tel quel)
docker run monapp

# Mode CLI — CMD est remplacé par l'argument fourni
docker run --rm monapp --mode=cli --ma-commande
```

Si on utilisait seulement `CMD ["dotnet", "MonApp.dll", "--mode=web"]`,
la surcharge remplacerait l'intégralité de la commande, forçant à répéter `dotnet MonApp.dll`
à chaque `docker run`.

### Usage en pratique

```bash
# Lancer le serveur web
docker run -d \
  -e MONAPP_ConnectionStrings__Server="..." \
  -p 8080:8080 \
  monapp

# Exécuter une commande CLI (mode one-shot, conteneur détruit après)
docker run --rm \
  -e MONAPP_ConnectionStrings__Server="..." \
  monapp \
  --mode=cli --ma-commande
```

---

## Étape 7 — Utilisation en CRON (cas d'usage typique)

Le mode CLI est conçu pour être appelé par un planificateur de tâches (cron Linux,
GitHub Actions Schedule, Kubernetes CronJob…).

**Exemple crontab Linux** (exécution chaque jour à 8h) :

```cron
0 8 * * * docker run --rm \
  -e MONAPP_ConnectionStrings__Server="..." \
  anthonyryck/monapp:latest \
  --mode=cli --ma-commande >> /var/log/monapp-cli.log 2>&1
```

**Exemple GitHub Actions Schedule** :

```yaml
on:
  schedule:
    - cron: '0 8 * * *'

jobs:
  run-cli:
    runs-on: ubuntu-latest
    steps:
      - name: Execute CLI command
        run: |
          docker run --rm \
            -e MONAPP_ConnectionStrings__Server="${{ secrets.DB_SERVER }}" \
            anthonyryck/monapp:latest \
            --mode=cli --ma-commande
```

---

## Ajouter une nouvelle commande CLI (check-list)

Quand il faut ajouter une nouvelle commande (ex: `--purge-archives`) :

- [ ] **`CliCommand.cs`** : ajouter `PurgeArchives` à l'enum
- [ ] **`StartupOptions.cs`** : ajouter la constante `private const string PurgeArchivesArgument = "--purge-archives"` et le `if` correspondant dans la boucle `foreach`
- [ ] **Service** : créer `IPurgeArchivesService` + `PurgeArchivesService` dans la feature concernée
- [ ] **Configuration DI** : enregistrer le service dans `XxxConfiguration.cs`
- [ ] **`Program.cs`** : ajouter `case CliCommand.PurgeArchives:` dans le `switch`
- [ ] **Dockerfile** : mettre à jour le commentaire d'usage si nécessaire

---

## Résumé du flux d'exécution

```
docker run monapp --mode=cli --ma-commande
         │
         ▼
    Program.cs
         │
    Parse(args) → StartupOptions { Mode=Cli, Command=MaCommande, IsValid=true }
         │
    WebApplicationBuilder.Build() → injection de TOUS les services
         │
    Migrations (EF Core, DbUp…)
         │
    if (Mode is Cli)
         │
    switch(Command)
         │
    case MaCommande:
         │
    IMaCommandeService.ExecuterAsync()
         │
    return  ← l'application s'arrête proprement
```

```
docker run monapp   (ou --mode=web)
         │
    Parse(args) → StartupOptions { Mode=Web, Command=None, IsValid=true }
         │
    (même build + migrations)
         │
    if (Mode is Cli) → false
         │
    app.UseAntiforgery()
    app.MapRazorComponents<App>()
    await app.RunAsync()  ← le serveur tourne indéfiniment
```

---

## Points d'attention

1. **Idempotence** : une commande CLI peut être exécutée plusieurs fois d'affilée sans effet
   indésirable. Mettre en place un mécanisme de traçage en base (ex: table `CommandLog`)
   pour éviter les doubles traitements.

2. **Code de retour** : si la commande échoue, `logger.LogCritical` suffit pour le diagnostic,
   mais penser à définir `Environment.ExitCode = 1` pour que le cron ou CI détecte l'échec.

3. **Timeout** : en mode CLI, si le traitement peut durer longtemps, s'assurer que
   le conteneur Docker n'est pas tué prématurément (pas de `--stop-timeout` trop court).

4. **Variables d'environnement** : le mode CLI a besoin des mêmes variables que le mode web
   (connexion DB, SMTP, etc.). Les passer toutes au `docker run --rm`.

5. **Pas de `UseHttpsRedirection`** en mode CLI : le pipeline HTTP n'est pas construit,
   ces appels sont dans la section web uniquement.
