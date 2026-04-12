# Schéma de la Base de Données - RateMyResto

## Vue d'ensemble
Base de données SQL Server pour une application de notation et partage d'expériences culinaires en équipe. Basée sur ASP.NET Identity pour la gestion des utilisateurs et des rôles.

---

## Tables Métier

### Teams
Représente les équipes qui organisent des événements repas collectifs.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | uniqueidentifier | NO | Identifiant unique de l'équipe (PK) |
| Nom | nvarchar | NO | Nom de l'équipe |
| Description | nvarchar | YES | Description optionnelle de l'équipe |
| OwnerTeamId | nvarchar | NO | ID de l'utilisateur propriétaire (FK → AspNetUsers.Id) |

**Relations:**
- N utilisateurs via `UserTeams` (1:N)
- N événements repas via `EventRepas` (1:N)

---

### UserTeams
Représente l'adhésion d'un utilisateur à une équipe.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | int | NO | Identifiant unique (PK) |
| UserId | nvarchar | NO | ID de l'utilisateur (FK → AspNetUsers.Id) |
| TeamId | uniqueidentifier | NO | ID de l'équipe (FK → Teams.Id) |
| DateFinPresence | date | YES | Date de fin de présence dans l'équipe (départ optionnel) |

**Relations:**
- Relie `AspNetUsers` à `Teams` (M:N)
- N participants d'événements via `Participants` (1:N)
- N rappels via `ReminderSent` (1:N)

---

### Restaurants
Catalogue des restaurants pouvant être associés aux événements.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | int | NO | Identifiant unique (PK) |
| Nom | nvarchar | NO | Nom du restaurant |
| Adresse | nvarchar | NO | Adresse du restaurant |
| LienGoogleMaps | nvarchar | YES | Lien Google Maps du restaurant |

**Relations:**
- N événements repas via `EventRepas` (1:N)

---

### EventRepas
Représente un événement repas collectif organisé par une équipe.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | uniqueidentifier | NO | Identifiant unique de l'événement (PK) |
| TeamId | uniqueidentifier | NO | ID de l'équipe (FK → Teams.Id) |
| InitiateurId | int | NO | ID de l'utilisateur initiateur (FK → UserTeams.Id) |
| RestaurantId | int | NO | ID du restaurant choisi (FK → Restaurants.Id) |
| DateEvenement | date | NO | Date de l'événement repas |
| Note | decimal | YES | Note globale optionnelle de l'événement |

**Relations:**
- N participants via `Participants` (1:N)
- N rappels via `ReminderSent` (1:N)

---

### Participants
Représente la participation d'un utilisateur à un événement repas avec sa notation.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | int | NO | Identifiant unique (PK) |
| EventRepasId | uniqueidentifier | NO | ID de l'événement (FK → EventRepas.Id) |
| UserId | int | NO | ID de l'utilisateur participant (FK → UserTeams.Id) |
| Note | decimal | YES | Note attribuée par le participant |
| Commentaire | nvarchar | YES | Commentaire/avis du participant |
| DateReview | date | YES | Date de l'avis |
| StatusParticipationId | tinyint | NO | Statut de participation (FK → StatusParticipation.Id) |

**Relations:**
- Reference `EventRepas` (N:1)
- Reference `UserTeams` (N:1)
- Reference `StatusParticipation` (N:1)

---

### StatusParticipation
Référentiel des statuts de participation possibles.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | tinyint | NO | Identifiant unique (PK) |
| Libelle | nvarchar | NO | Libellé du statut (ex: "Confirmé", "Refusé", "En attente") |

**Relations:**
- N participants via `Participants` (1:N)

---

### ReminderSent
Historique des rappels/notifications envoyés aux participants.

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| Id | int | NO | Identifiant unique (PK) |
| EventRepasId | uniqueidentifier | NO | ID de l'événement (FK → EventRepas.Id) |
| UserTeamsId | int | NO | ID du membre de l'équipe (FK → UserTeams.Id) |
| SentAt | datetime2 | NO | Timestamp d'envoi du rappel |

**Relations:**
- Reference `EventRepas` (N:1)
- Reference `UserTeams` (N:1)

---

## Tables de Gestion (ASP.NET Identity)

### AspNetUsers
Utilisateurs de l'application avec authentification.

| Colonne | Type | Nullable |
|---------|------|----------|
| Id | nvarchar | NO (PK) |
| UserName | nvarchar | YES |
| NormalizedUserName | nvarchar | YES |
| Email | nvarchar | YES |
| NormalizedEmail | nvarchar | YES |
| EmailConfirmed | bit | NO |
| PasswordHash | nvarchar | YES |
| SecurityStamp | nvarchar | YES |
| ConcurrencyStamp | nvarchar | YES |
| PhoneNumber | nvarchar | YES |
| PhoneNumberConfirmed | bit | NO |
| TwoFactorEnabled | bit | NO |
| LockoutEnd | datetimeoffset | YES |
| LockoutEnabled | bit | NO |
| AccessFailedCount | int | NO |
| DisplayName | nvarchar | YES |

**Relations:**
- Propriétaire de Teams (1:N)
- Membre via UserTeams (N:M)

---

### AspNetRoles
Rôles disponibles dans l'application.

| Colonne | Type | Nullable |
|---------|------|----------|
| Id | nvarchar | NO (PK) |
| Name | nvarchar | YES |
| NormalizedName | nvarchar | YES |
| ConcurrencyStamp | nvarchar | YES |

---

### AspNetUserRoles
Assignation des rôles aux utilisateurs (M:N).

| Colonne | Type | Nullable |
|---------|------|----------|
| UserId | nvarchar | NO (PK + FK) |
| RoleId | nvarchar | NO (PK + FK) |

---

### AspNetUserClaims
Revendications associées aux utilisateurs.

| Colonne | Type | Nullable |
|---------|------|----------|
| Id | int | NO (PK) |
| UserId | nvarchar | NO (FK) |
| ClaimType | nvarchar | YES |
| ClaimValue | nvarchar | YES |

---

### AspNetRoleClaims
Revendications associées aux rôles.

| Colonne | Type | Nullable |
|---------|------|----------|
| Id | int | NO (PK) |
| RoleId | nvarchar | NO (FK) |
| ClaimType | nvarchar | YES |
| ClaimValue | nvarchar | YES |

---

### AspNetUserLogins
Connexions externes (OAuth, etc.).

| Colonne | Type | Nullable |
|---------|------|----------|
| LoginProvider | nvarchar | NO (PK) |
| ProviderKey | nvarchar | NO (PK) |
| ProviderDisplayName | nvarchar | YES |
| UserId | nvarchar | NO (FK) |

---

### AspNetUserTokens
Tokens de l'utilisateur (réinitialisation mot de passe, etc.).

| Colonne | Type | Nullable |
|---------|------|----------|
| UserId | nvarchar | NO (PK + FK) |
| LoginProvider | nvarchar | NO (PK) |
| Name | nvarchar | NO (PK) |
| Value | nvarchar | YES |

---

### AspNetUserPasskeys
Données des clés de passe (WebAuthn/FIDO2).

| Colonne | Type | Nullable |
|---------|------|----------|
| CredentialId | varbinary | NO (PK) |
| UserId | nvarchar | NO (FK) |
| Data | nvarchar | NO |

---

## Tables Système

### __EFMigrationsHistory
Historique des migrations Entity Framework Core.

| Colonne | Type |
|---------|------|
| MigrationId | nvarchar (PK) |
| ProductVersion | nvarchar |

---

### SchemaVersions
Historique des versions du schéma.

| Colonne | Type |
|---------|------|
| Id | int (PK) |
| ScriptName | nvarchar |
| Applied | datetime |

---

## Diagramme des Relations Métier

```
┌─────────────────────────────────────────────────────────┐
│                      UTILISATEURS                        │
│                    AspNetUsers (Id)                      │
└─────────────────────────────────────────────────────────┘
                            │
                            │ 1:N
                            ├──────────────┬──────────────┐
                            │              │              │
                    ┌───────┴─────┐  ┌─────┴──────┐  ┌────┴──────┐
                    │   EQUIPES   │  │ USERS_TEAM │  │   ROLES   │
                    │  Teams (Id) │  │UserTeams   │  │  AspNet   │
                    └───────┬─────┘  │  (Id)      │  │ UserRoles │
                            │        └────────────┘  └───────────┘
                            │
                    ┌───────┴─────────────┐
                    │                     │ 1:N
                    │                     │
            ┌───────┴──────────┐   ┌──────┴──────────────────┐
            │  EVENT_REPAS     │   │   REMINDER_SENT         │
            │ EventRepas (Id)  │   │  ReminderSent (Id)      │
            └───────┬──────────┘   └──────────────────────────┘
                    │ N:1
                    │
            ┌───────┴──────────────┐
            │   PARTICIPANTS       │
            │ Participants (Id)    │
            │   + Note + Avis      │
            └──────────────────────┘
                    │
                    │ N:1
            ┌───────┴──────────────┐
            │STATUS_PARTICIPATION  │
            │StatusParticipation   │
            └──────────────────────┘

            ┌──────────────────┐
            │  RESTAURANTS     │
            │ Restaurants (Id) │
            └──────────────────┘
                    ▲
                    │ N:1
            ┌───────┴──────────────┐
            │   EVENT_REPAS        │
            │ EventRepas (Id)      │
            └──────────────────────┘
```

---

## Notes de Conception

### Clés et Identifiants
- **uniqueidentifier** : Pour Teams et EventRepas (GUID)
- **int** : Pour les identifiants autoincrémentés standards
- **nvarchar** : Pour tous les identifiants AspNetUsers (standard Identity)
- **tinyint** : Pour les énumérés (StatusParticipation)

### Intégrité Référentielle
- Toutes les clés étrangères sont présentes
- Les tables métier dépendent d'une structure Team centralisée
- Les participants sont liés à UserTeams plutôt qu'à AspNetUsers directement (meilleur historique)

### Conventions
- Préfixes FK pour identifier les clés étrangères
- Utilisation d'Entity Framework Core (migrations)
- Structure conforme aux meilleures pratiques ASP.NET
