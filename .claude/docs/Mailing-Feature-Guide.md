# Guide d'implémentation — Feature Mailing

**Version : 1.0.0**
**Basé sur : RateMyResto (Blazor Server / .NET 10 / SQL Server)**

---

## Vue d'ensemble

La feature Mailing permet :
1. **Notification de création d'événement** : email envoyé à tous les membres actifs de l'équipe dès qu'un nouvel événement est créé.
2. **Rappel de vote** : email envoyé (via une commande CLI/CRON) aux participants confirmés d'un événement passé n'ayant pas encore voté.

L'envoi se fait via **MailKit** vers un serveur SMTP interne Docker (pas d'authentification, pas de TLS). Les **templates d'email sont des composants Blazor** rendus en HTML par **BlazorMail**.

---

## 1. Packages NuGet

Ajouter dans le fichier `.csproj` :

```xml
<PackageReference Include="MailKit" Version="4.14.0" />
<PackageReference Include="BlazorMail" Version="0.0.1" />
```

- **MailKit** : client SMTP open-source pour .NET
- **BlazorMail** : rendu de composants Blazor en HTML (pour les templates d'email)

---

## 2. Structure des fichiers

```
Features/Mailing/
├── Components/
│   ├── NewEventEmailComponent.razor          # Template email nouvel événement
│   ├── NewEventEmailComponent.razor.cs
│   ├── VoteReminderEmailComponent.razor      # Template email rappel de vote
│   └── VoteReminderEmailComponent.razor.cs
├── Configurations/
│   └── MailingConfiguration.cs              # Enregistrement DI
├── Models/
│   ├── DbModels/
│   │   ├── NewEventParticipantDb.cs          # Données brutes DB pour les destinataires
│   │   └── PendingReminderDb.cs              # Données brutes DB pour les rappels
│   ├── MailingAppSettings.cs                 # URL de base de l'application
│   └── SmtpSettings.cs                      # Paramètres SMTP
├── Repositories/
│   ├── IEventNotificationRepository.cs
│   ├── EventNotificationRepository.cs
│   ├── IReminderRepository.cs
│   └── ReminderRepository.cs
└── Services/
    ├── IMailSender.cs
    ├── MailSender.cs                         # Envoi bas niveau via MailKit
    ├── IEventNotificationService.cs
    ├── EventNotificationService.cs           # Orchestration notification événement
    ├── IReminderService.cs
    └── ReminderService.cs                    # Orchestration rappels de vote
```

---

## 3. Modèles de configuration

### `SmtpSettings.cs`

```csharp
namespace YourApp.Features.Mailing.Models;

/// <summary>
/// Paramètres de connexion au serveur SMTP.
/// Lié à la section "Smtp" de la configuration (variables d'env Smtp__*).
/// </summary>
public sealed class SmtpSettings
{
    /// <summary>Nom d'hôte du serveur SMTP (ex: mail_postfix).</summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>Port du serveur SMTP (par défaut 587).</summary>
    public int Port { get; set; } = 587;

    /// <summary>Adresse d'expéditeur utilisée dans le champ From des messages.</summary>
    public string From { get; set; } = string.Empty;
}
```

### `MailingAppSettings.cs`

```csharp
namespace YourApp.Features.Mailing.Models;

/// <summary>
/// Paramètres applicatifs utilisés par la feature Mailing.
/// Lié à la section "Mailing" de la configuration (variable d'env Mailing__AppBaseUrl).
/// </summary>
public sealed class MailingAppSettings
{
    /// <summary>
    /// URL de base de l'application, utilisée pour construire les liens dans les emails.
    /// Exemple : https://mon-app.example.com
    /// </summary>
    public string AppBaseUrl { get; set; } = string.Empty;
}
```

---

## 4. Service bas niveau — `MailSender`

### Interface

```csharp
namespace YourApp.Features.Mailing.Services;

/// <summary>
/// Abstraction bas niveau pour l'envoi d'un email HTML.
/// </summary>
public interface IMailSender
{
    /// <summary>
    /// Envoie un email au format HTML à un destinataire unique.
    /// </summary>
    /// <param name="to">Adresse email du destinataire.</param>
    /// <param name="subject">Sujet du message.</param>
    /// <param name="htmlBody">Corps HTML du message.</param>
    Task SendHtmlAsync(string to, string subject, string htmlBody);
}
```

### Implémentation

```csharp
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using YourApp.Features.Mailing.Models;

namespace YourApp.Features.Mailing.Services;

/// <summary>
/// Implémentation MailKit de <see cref="IMailSender"/>.
/// Cible un serveur SMTP interne Docker (pas d'auth, pas de TLS).
/// </summary>
public sealed class MailSender : IMailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<MailSender> _logger;

    public MailSender(IOptions<SmtpSettings> settings, ILogger<MailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendHtmlAsync(string to, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            throw new InvalidOperationException(
                "SMTP non configuré : Smtp:Host est vide. Vérifier les variables d'env Smtp__*.");
        }

        if (string.IsNullOrWhiteSpace(_settings.From))
        {
            throw new InvalidOperationException(
                "SMTP non configuré : Smtp:From est vide. Vérifier la variable d'env Smtp__From.");
        }

        MimeMessage message = new();
        message.From.Add(MailboxAddress.Parse(_settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        _logger.LogInformation("Envoi SMTP vers {To} via {Host}:{Port}",
            to, _settings.Host, _settings.Port);

        using SmtpClient client = new();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None);
        await client.SendAsync(message);
        await client.DisconnectAsync(quit: true);

        _logger.LogInformation("Email envoyé à {To}", to);
    }
}
```

> **Note** : `SecureSocketOptions.None` = pas de TLS. Pour un serveur SMTP externe avec TLS (ex: SendGrid, SMTP Office 365), remplacer par `SecureSocketOptions.StartTls` et ajouter l'authentification avec `client.AuthenticateAsync(user, password)`.

---

## 5. Templates d'email — Composants Blazor

Les emails sont rendus via BlazorMail qui exécute un composant Blazor et retourne le HTML produit.

### Règles pour les composants email

- Composants **autonomes** : aucun service injecté (`@inject` interdit), uniquement des `[Parameter]`
- Styles **inline uniquement** (les clients email ne supportent pas les feuilles de style externes)
- Design **responsive** avec largeur max (580px recommandé)
- Tester l'affichage sur les principaux clients email (Gmail, Outlook)

### Exemple — `VoteReminderEmailComponent.razor.cs`

```csharp
using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace YourApp.Features.Mailing.Components;

/// <summary>
/// Composant Blazor rendu en HTML par BlazorMail pour le rappel de vote.
/// Autonome : aucun service injecté, uniquement des paramètres.
/// </summary>
public sealed partial class VoteReminderEmailComponent : ComponentBase
{
    /// <summary>Nom d'affichage du destinataire.</summary>
    [Parameter]
    public required string DisplayName { get; set; }

    /// <summary>Nom de l'équipe.</summary>
    [Parameter]
    public required string NomEquipe { get; set; }

    /// <summary>Nom du restaurant.</summary>
    [Parameter]
    public required string NomRestaurant { get; set; }

    /// <summary>Date de l'événement.</summary>
    [Parameter]
    public required DateOnly DateEvenement { get; set; }

    /// <summary>URL complète vers la page de détail de l'événement.</summary>
    [Parameter]
    public required string EventDetailUrl { get; set; }

    private string FormattedDate => DateEvenement.ToString("dddd d MMMM yyyy",
                                                           CultureInfo.GetCultureInfo("fr-FR"));
}
```

### Rendu du template dans un service

```csharp
// Injection de IBlazorMailRenderer (fourni par BlazorMail)
private readonly IBlazorMailRenderer _renderer;

// Rendu du composant en HTML
string htmlBody = await _renderer.RenderAsync<VoteReminderEmailComponent>(parameters =>
{
    parameters.Add(p => p.DisplayName, reminder.DisplayName);
    parameters.Add(p => p.NomEquipe, reminder.NomEquipe);
    parameters.Add(p => p.NomRestaurant, reminder.NomRestaurant);
    parameters.Add(p => p.DateEvenement, reminder.DateEvenement);
    parameters.Add(p => p.EventDetailUrl, $"{_mailingSettings.AppBaseUrl}/event/detail/{reminder.EventId}");
});

await _mailSender.SendHtmlAsync(reminder.Email, "Sujet du mail", htmlBody);
```

---

## 6. Services d'orchestration

### `IReminderService` / `ReminderService`

Orchestre l'envoi des rappels de vote en mode CLI :

```csharp
public interface IReminderService
{
    /// <summary>
    /// Récupère les rappels en attente puis envoie un email à chaque participant.
    /// Chaque envoi réussi est tracé en base pour garantir l'idempotence.
    /// </summary>
    /// <returns>Nombre de rappels envoyés.</returns>
    Task<int> SendPendingRemindersAsync();
}
```

**Algorithme de `SendPendingRemindersAsync`** :
1. Récupérer les rappels en attente via le repository (procédure stockée)
2. Pour chaque rappel :
   - Rendre le template Blazor en HTML
   - Envoyer l'email via `IMailSender`
   - Marquer le rappel comme envoyé en base (idempotence)
3. Retourner le nombre de rappels envoyés avec succès

### `IEventNotificationService` / `EventNotificationService`

Envoie une notification à tous les membres actifs lors de la création d'un événement :

```csharp
public interface IEventNotificationService
{
    /// <summary>
    /// Envoie un email de notification à tous les membres actifs de l'équipe
    /// pour un événement nouvellement créé.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement nouvellement créé.</param>
    /// <returns>Nombre d'emails envoyés avec succès.</returns>
    Task<int> SendNewEventNotificationsAsync(Guid eventId);
}
```

**Appel dans le service métier (non bloquant)** :

```csharp
// Après la création réussie de l'événement
try
{
    int sent = await _eventNotificationService.SendNewEventNotificationsAsync(idEvent);
    _logger.LogInformation("Notifications envoyées pour l'événement {EventId} : {Count}", idEvent, sent);
}
catch (Exception ex)
{
    // La création a réussi — ne pas bloquer l'utilisateur pour un échec de notification
    _logger.LogError(ex, "Erreur lors de l'envoi des notifications pour l'événement {EventId}", idEvent);
}
```

> **Important** : le catch est intentionnel. Un échec d'envoi d'email ne doit **jamais** annuler l'action métier qui l'a déclenché.

---

## 7. Repositories

### `IReminderRepository`

```csharp
public interface IReminderRepository
{
    /// <summary>Récupère la liste des rappels de vote en attente d'envoi.</summary>
    Task<ResultOf<List<PendingReminderDb>>> GetPendingRemindersAsync();

    /// <summary>Enregistre qu'un rappel a été envoyé pour un couple (événement, participant).</summary>
    Task<ResultOf> MarkReminderSentAsync(Guid eventId, int userTeamsId, DateTime sentAt);
}
```

### `IEventNotificationRepository`

```csharp
public interface IEventNotificationRepository
{
    /// <summary>
    /// Retourne la liste des destinataires à notifier pour un événement nouvellement créé.
    /// Seuls les membres actifs disposant d'une adresse email sont inclus.
    /// </summary>
    Task<ResultOf<List<NewEventParticipantDb>>> GetNewEventRecipientsAsync(Guid eventId);
}
```

---

## 8. Enregistrement DI — `MailingConfiguration.cs`

```csharp
using BlazorMail.Extensions;
using YourApp.Features.Mailing.Models;
using YourApp.Features.Mailing.Repositories;
using YourApp.Features.Mailing.Services;

namespace YourApp.Features.Mailing.Configurations;

/// <summary>
/// Enregistrement des dépendances de la feature Mailing.
/// </summary>
public static class MailingConfiguration
{
    /// <summary>
    /// Ajoute les services de la feature Mailing au conteneur DI.
    /// </summary>
    public static IServiceCollection AddMailingFeatures(this IServiceCollection services,
                                                         IConfiguration configuration)
    {
        // Binding des sections de configuration
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<MailingAppSettings>(configuration.GetSection("Mailing"));

        // BlazorMail : rendu de composants Blazor en HTML
        services.AddBlazorMail();

        // Repositories
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IEventNotificationRepository, EventNotificationRepository>();

        // Services
        services.AddTransient<IMailSender, MailSender>();       // Transient : pas d'état, connexion SMTP par appel
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<IEventNotificationService, EventNotificationService>();

        return services;
    }
}
```

### Appel dans `Program.cs`

```csharp
// Ajouter après les autres features
builder.Services.AddMailingFeatures(builder.Configuration);
```

---

## 9. Mode CLI pour les rappels CRON

Le projet utilise un mode CLI pour exécuter les rappels sans démarrer le pipeline HTTP :

### Parsing des arguments dans `Program.cs`

```csharp
// Exemple simplifié de parsing
bool isCli = args.Contains("--mode=cli");
bool isReminder = args.Contains("--reminder");

if (isCli)
{
    // Construire l'app sans démarrer le pipeline HTTP
    WebApplication app = builder.Build();

    using IServiceScope cliScope = app.Services.CreateScope();

    if (isReminder)
    {
        IReminderService reminderService =
            cliScope.ServiceProvider.GetRequiredService<IReminderService>();
        int sent = await reminderService.SendPendingRemindersAsync();
        logger?.LogInformation("CLI --reminder terminée. Rappels envoyés : {Count}", sent);
    }

    return; // Terminer sans démarrer le serveur web
}

// Mode normal : démarrer le serveur web
app.Run();
```

### Utilisation

```bash
# Mode web (par défaut)
docker run image --mode=web

# Mode CLI : envoi des rappels
docker run --rm image --mode=cli --reminder
```

---

## 10. Configuration Docker

### Variables d'environnement à ajouter dans le Dockerfile

```dockerfile
# Serveur SMTP
ENV APP_Smtp__Host=""
ENV APP_Smtp__Port="587"
ENV APP_Smtp__From=""

# URL publique de l'application — utilisée dans les liens des emails
ENV APP_Mailing__AppBaseUrl=""
```

> Remplacer `APP_` par le préfixe de variables d'environnement de votre application (dans RateMyResto, c'est `ENVRATE_`).

### CMD pour permettre le mode CLI

```dockerfile
# Démarrage web par défaut
ENTRYPOINT ["dotnet", "YourApp.dll"]
CMD ["--mode=web"]
```

Le `CMD` peut être surchargé au `docker run` :
```bash
docker run --rm image --mode=cli --reminder
```

### Configuration du provider d'env dans `Program.cs`

```csharp
// Ajouter le préfixe de vos variables d'environnement
builder.Configuration.AddEnvironmentVariables(prefix: "APP_");
```

---

## 11. Base de données SQL Server

### Table `ReminderSent` (idempotence)

```sql
CREATE TABLE dbo.ReminderSent
(
    Id           INT IDENTITY(1,1) NOT NULL,
    EventRepasId UNIQUEIDENTIFIER  NOT NULL,
    UserTeamsId  INT               NOT NULL,
    SentAt       DATETIME2         NOT NULL,

    CONSTRAINT PK_ReminderSent
        PRIMARY KEY (Id),

    CONSTRAINT FK_ReminderSent_EventRepas
        FOREIGN KEY (EventRepasId) REFERENCES dbo.EventRepas(Id),

    CONSTRAINT FK_ReminderSent_UserTeams
        FOREIGN KEY (UserTeamsId) REFERENCES dbo.UserTeams(Id),

    CONSTRAINT UK_ReminderSent_Event_User
        UNIQUE (EventRepasId, UserTeamsId)
);

CREATE INDEX IX_ReminderSent_EventRepasId
    ON dbo.ReminderSent (EventRepasId);
```

> La contrainte `UK_ReminderSent_Event_User` garantit qu'un seul rappel est envoyé par couple (événement, participant).

### Procédure stockée — Rappels en attente

```sql
CREATE PROCEDURE sp_GetPendingVoteReminders
AS
BEGIN
    SET NOCOUNT ON;

    SELECT evt.Id                                  AS EventId,
           evt.DateEvenement                       AS DateEvenement,
           r.Nom                                   AS NomRestaurant,
           eq.Nom                                  AS NomEquipe,
           ut.Id                                   AS UserTeamsId,
           usr.Id                                  AS AspNetUserId,
           usr.Email                               AS Email,
           COALESCE(usr.DisplayName, usr.UserName) AS DisplayName
    FROM dbo.EventRepas     AS evt
    INNER JOIN dbo.Participants AS p   ON p.EventRepasId = evt.Id
    INNER JOIN dbo.UserTeams    AS ut  ON ut.Id = p.UserId
    INNER JOIN dbo.AspNetUsers  AS usr ON usr.Id = ut.UserId
    INNER JOIN dbo.Restaurants  AS r   ON r.Id = evt.RestaurantId
    INNER JOIN dbo.Teams        AS eq  ON eq.Id = evt.TeamId
    LEFT JOIN  dbo.ReminderSent AS rs  ON rs.EventRepasId = evt.Id
                                      AND rs.UserTeamsId = ut.Id
    WHERE evt.DateEvenement < CAST(GETDATE() AS DATE)  -- Événement passé
      AND p.StatusParticipationId = 2                  -- Participant confirmé
      AND p.Note IS NULL                               -- Pas encore voté
      AND ut.DateFinPresence IS NULL                   -- Membre actif
      AND usr.Email IS NOT NULL
      AND usr.Email <> ''
      AND rs.Id IS NULL                                -- Pas déjà envoyé
    ORDER BY evt.DateEvenement ASC, usr.Email ASC
    FOR JSON PATH, INCLUDE_NULL_VALUES;
END
```

### Procédure stockée — Marquer rappel envoyé

```sql
CREATE PROCEDURE sp_MarkReminderSent
    @EventRepasId UNIQUEIDENTIFIER,
    @UserTeamsId  INT,
    @SentAt       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ReminderSent (EventRepasId, UserTeamsId, SentAt)
    VALUES (@EventRepasId, @UserTeamsId, @SentAt);
END
```

### Procédure stockée — Destinataires d'un nouvel événement

```sql
CREATE PROCEDURE sp_GetNewEventEmailData
    @EventRepasId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT usr.Email                                       AS Email,
           COALESCE(usr.DisplayName, usr.UserName)         AS DisplayName,
           COALESCE(ini_usr.DisplayName, ini_usr.UserName) AS NomOrganisateur,
           t.Nom                                           AS NomEquipe,
           r.Nom                                           AS NomRestaurant,
           r.Adresse                                       AS AdresseRestaurant,
           evt.DateEvenement                               AS DateEvenement
    FROM dbo.EventRepas  AS evt
    INNER JOIN dbo.Participants  AS p       ON p.EventRepasId = evt.Id
    INNER JOIN dbo.UserTeams     AS ut      ON ut.Id = p.UserId
    INNER JOIN dbo.AspNetUsers   AS usr     ON usr.Id = ut.UserId
    INNER JOIN dbo.Teams         AS t       ON t.Id = evt.TeamId
    INNER JOIN dbo.Restaurants   AS r       ON r.Id = evt.RestaurantId
    INNER JOIN dbo.UserTeams     AS ini_ut  ON ini_ut.Id = evt.InitiateurId
    INNER JOIN dbo.AspNetUsers   AS ini_usr ON ini_usr.Id = ini_ut.UserId
    WHERE evt.Id = @EventRepasId
      AND usr.Email IS NOT NULL
      AND usr.Email <> ''
      AND ut.DateFinPresence IS NULL  -- Membres actifs uniquement
    FOR JSON PATH;
END
```

---

## 12. Serveur SMTP interne Docker — Postfix

Pour un environnement Docker, utiliser un conteneur Postfix comme relais SMTP local :

### `docker-compose.yml` (exemple)

```yaml
services:
  app:
    image: your-app:latest
    environment:
      - APP_Smtp__Host=mail_postfix
      - APP_Smtp__Port=587
      - APP_Smtp__From=noreply@votre-domaine.com
      - APP_Mailing__AppBaseUrl=https://votre-app.example.com
    depends_on:
      - mail_postfix

  mail_postfix:
    image: boky/postfix
    environment:
      - ALLOWED_SENDER_DOMAINS=votre-domaine.com
      - RELAYHOST=[smtp.provider.com]:587  # Optionnel : relayer vers SMTP externe
    ports:
      - "587:587"
```

> L'image `boky/postfix` est un relais SMTP léger et populaire pour Docker. Sans `RELAYHOST`, les emails sont envoyés directement (nécessite une configuration DNS correcte).

---

## 13. Configuration locale (développement)

Ajouter dans `appsettings.Development.json` :

```json
{
    "Smtp": {
        "Host": "localhost",
        "Port": 1025,
        "From": "dev@localhost"
    },
    "Mailing": {
        "AppBaseUrl": "https://localhost:5001"
    }
}
```

Pour le développement, utiliser **MailHog** ou **Mailpit** comme serveur SMTP local qui intercepte les emails sans les envoyer :

```bash
# Mailpit (plus récent, recommandé)
docker run -d -p 1025:1025 -p 8025:8025 axllent/mailpit

# Interface web : http://localhost:8025
# SMTP : localhost:1025
```

---

## 14. Checklist de mise en place

### Packages et configuration

- [ ] Ajouter `MailKit` et `BlazorMail` au `.csproj`
- [ ] Créer `SmtpSettings.cs` et `MailingAppSettings.cs`
- [ ] Créer `IMailSender.cs` et `MailSender.cs`
- [ ] Créer `MailingConfiguration.cs` avec `AddMailingFeatures()`
- [ ] Appeler `builder.Services.AddMailingFeatures(builder.Configuration)` dans `Program.cs`
- [ ] Ajouter `builder.Configuration.AddEnvironmentVariables(prefix: "APP_")` dans `Program.cs`

### Templates d'email

- [ ] Créer les composants Blazor dans `Features/Mailing/Components/`
- [ ] Vérifier : aucun `@inject` dans les templates, uniquement des `[Parameter]`
- [ ] Utiliser des styles inline dans les templates (compatibilité clients email)

### Services métier

- [ ] Créer les interfaces et implémentations des services d'orchestration
- [ ] Créer les repositories et leurs interfaces
- [ ] Appeler les services de notification de façon non bloquante (try/catch)

### Base de données

- [ ] Créer la table `ReminderSent` avec contrainte d'unicité
- [ ] Créer les procédures stockées `sp_GetPendingVoteReminders` et `sp_MarkReminderSent`
- [ ] Créer la procédure stockée `sp_GetNewEventEmailData`

### Docker

- [ ] Ajouter les variables d'environnement SMTP dans le Dockerfile
- [ ] Configurer le `CMD` pour permettre le mode CLI (`--mode=cli --reminder`)
- [ ] Configurer Postfix ou un SMTP relay dans le `docker-compose.yml`

### CRON (rappels de vote)

- [ ] Configurer un job CRON sur le serveur ou via Kubernetes CronJob
- [ ] Commande : `docker run --rm image --mode=cli --reminder`
- [ ] Fréquence recommandée : 1x par jour (ex: 8h00)

---

## 15. Flux d'envoi résumés

### Flux 1 — Notification de création d'événement

```
Utilisateur crée un événement
    → EventService.CreateEventAsync()
    → Succès : EventNotificationService.SendNewEventNotificationsAsync(eventId)
        → EventNotificationRepository.GetNewEventRecipientsAsync(eventId)
            → sp_GetNewEventEmailData (membres actifs avec email)
        → Pour chaque membre :
            → IBlazorMailRenderer.RenderAsync<NewEventEmailComponent>(...)
            → IMailSender.SendHtmlAsync(email, sujet, html)
        → Retourne le nombre d'emails envoyés
    → Exception silencieuse : log erreur, ne bloque pas l'utilisateur
```

### Flux 2 — Rappels de vote (CLI/CRON)

```
CRON déclenche : docker run --rm image --mode=cli --reminder
    → Program.cs détecte le mode CLI
    → ReminderService.SendPendingRemindersAsync()
        → ReminderRepository.GetPendingRemindersAsync()
            → sp_GetPendingVoteReminders (participants sans vote, sans rappel déjà envoyé)
        → Pour chaque rappel en attente :
            → IBlazorMailRenderer.RenderAsync<VoteReminderEmailComponent>(...)
            → IMailSender.SendHtmlAsync(email, sujet, html)
            → ReminderRepository.MarkReminderSentAsync(eventId, userTeamsId, now)
        → Retourne le nombre de rappels envoyés
    → Arrêt du processus (pas de serveur web démarré)
```

---

## 16. Points d'attention

| Point | Détail |
|-------|--------|
| **Idempotence** | La table `ReminderSent` avec contrainte `UNIQUE` garantit qu'un rappel n'est envoyé qu'une seule fois par couple (événement, participant). |
| **Non-bloquant** | Les erreurs d'envoi d'email ne doivent jamais annuler l'action métier déclenchante. Toujours entourer d'un `try/catch`. |
| **Styles email** | Utiliser uniquement des styles inline dans les templates. Les feuilles CSS externes ne sont pas supportées par la plupart des clients email. |
| **Pas d'auth SMTP** | La configuration actuelle cible un SMTP interne Docker sans authentification. Pour un SMTP externe (SendGrid, Office 365, etc.), ajouter `SecureSocketOptions.StartTls` et `client.AuthenticateAsync()`. |
| **URL de base** | L'`AppBaseUrl` doit correspondre à l'URL publique réelle de l'application (pas `localhost`). Elle est utilisée pour construire les liens dans les emails. |
| **Membres actifs** | Ne notifier que les membres dont `DateFinPresence IS NULL` (membres toujours actifs dans l'équipe). |
| **Email requis** | Filtrer `usr.Email IS NOT NULL AND usr.Email <> ''` dans les procédures stockées avant tout envoi. |
