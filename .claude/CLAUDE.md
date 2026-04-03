# Règles de développement pour RateMyResto

**Version : 1.0.0**

---

## 🛠️ Projet

Application web Blazor Server de notation d'événements de restaurants en équipe.

- **Framework** : .NET 10 / ASP.NET Core + Blazor Server
- **Base de données** : SQL Server (EF Core 10 + DbUp)
- **Auth** : ASP.NET Core Identity (cookie, pas de confirmation email)
- **Déploiement** : Docker + GitHub Actions → Docker Hub

### Commandes utiles

```bash
# Depuis src/RateMyResto
dotnet restore RateMyResto.slnx
dotnet build RateMyResto.slnx --configuration Debug
dotnet run --project RateMyResto/RateMyResto.csproj

# Docker (depuis src/)
docker build -f Dockerfile -t anthonyryck/ratemyresto:latest .
docker run -e ENVRATE_ConnectionStrings__Server="..." \
           -e ENVRATE_ConnectionStrings__Login="..." \
           -e ENVRATE_ConnectionStrings__Password="..." \
           -e ENVRATE_AdminAccount__Password="..." \
           -p 8080:8080 \
           anthonyryck/ratemyresto:latest
```

### Structure du projet

```
src/RateMyResto/RateMyResto/
├── Core/                    # Infrastructure transversale
│   ├── Data/                # Repository de base
│   ├── Logging/             # Utilitaires de log
│   └── Models/              # Result pattern, types d'erreurs
├── Features/                # Vertical Slice Architecture
│   ├── Account/             # Authentification, gestion des utilisateurs
│   ├── Data/                # DbContext + migrations EF Core
│   ├── DbMigration/         # Scripts DbUp
│   ├── Event/               # Gestion des événements de vote
│   ├── EventDetail/         # Détails, avis, photos
│   ├── Team/                # Gestion des équipes
│   ├── Shared/              # Services et composants partagés
│   └── Layout/              # Composants de mise en page
└── Program.cs
```

---

## 🏗️ Architecture

### Vertical Slice Architecture
- **TOUJOURS** suivre la Vertical Slice Architecture
- Structure obligatoire : `Features/{Feature}/{Components,Services,Models,Repository,Configurations}`
- Chaque feature est autonome et contient tout ce dont elle a besoin
- Pas de dossiers "Shared" sauf pour les vrais utilitaires partagés

### Services et couches
- Services métier : contiennent la logique applicative
- Services de vue (ViewService) : gèrent l'état et la logique de présentation
- Repositories : accès aux données uniquement
- Pas de logique métier dans les repositories

---

## 📝 Standards de code C#

### Classes et Records
- **Toutes les classes et records doivent être `sealed` par défaut**
- Utiliser `record` pour les modèles immuables (DTOs, ViewModels, etc.)
- Utiliser `class` pour les services et repositories
- N'enlever `sealed` QUE si l'héritage est explicitement nécessaire

```csharp
// ✅ BON
public sealed record EventViewModel { ... }
public sealed class EventService { ... }

// ❌ MAUVAIS
public record EventViewModel { ... }
public class EventService { ... }
```

### Comparaisons et tests de nullité
- **TOUJOURS** utiliser `is` ou `is not` pour les comparaisons d'enum et les tests de nullité
- Plus d'intention et de lisibilité qu'avec `==` ou `!=`

**EXCEPTIONS** :
- **LINQ to Entities (EntityFramework)** : Utiliser `==` et `!=` dans les projections car EF ne sait pas traduire `is`/`is not` en SQL correctement
- **LINQ to Objects (en mémoire)** : Utiliser `==` et `!=` dans les expressions lambda (`.Where()`, `.Select()`, `.FirstOrDefault()`, etc.) pour éviter les problèmes de compatibilité

```csharp
// ✅ BON - Tests de nullité en C# standard
if (myVariable is null) { ... }
if (myVariable is not null) { ... }

// ✅ BON - Comparaisons d'enum
if (status is EventStatus.Active) { ... }

// ✅ BON - LINQ to Entities (projections EntityFramework)
var query = _context.Events
    .Select(e => new EventDto
    {
        IsActive = e.Status == EventStatus.Active  // OK dans une projection EF
    });

// ✅ BON - LINQ to Objects (en mémoire)
var activeEvents = events.Where(e => e.Status == EventStatus.Active).ToList();

// ❌ MAUVAIS - En dehors de LINQ, en C# standard
if (myVariable == null) { ... }
if (status == EventStatus.Active) { ... }

// ❌ MAUVAIS - LINQ avec is/is not (peut causer des problèmes)
var activeEvents = events.Where(e => e.Status is EventStatus.Active).ToList();
```

**Résumé** :
- Code C# standard (if, switch, etc.) → Utiliser `is` / `is not`
- Expressions LINQ (Where, Select, FirstOrDefault, etc.) → Utiliser `==` / `!=`

### Documentation
- **TOUJOURS** documenter les méthodes publiques avec des XML comments (`///`)
- Documenter les propriétés publiques des modèles
- Utiliser `<summary>`, `<param>`, `<returns>`, `<exception>` selon le besoin
- Ajouter des commentaires inline pour la logique complexe

```csharp
/// <summary>
/// Charge les événements actifs pour une équipe donnée.
/// </summary>
/// <param name="teamId">Identifiant de l'équipe.</param>
/// <returns>Une liste d'événements actifs.</returns>
public async Task<List<EventViewModel>> LoadActiveEventsAsync(int teamId) { ... }
```

### Injection de dépendances
- **TOUJOURS** utiliser l'injection de dépendances pour les services
- Enregistrer les services dans les fichiers `Configurations/{Feature}Configuration.cs`
- Utiliser `services.AddScoped<T>()` pour les services avec état de requête
- Utiliser `services.AddSingleton<T>()` pour les services sans état

### Magic Numbers et Magic Strings

**PRINCIPE** : Éviter les valeurs littérales obscures dans la logique métier. Nommer les valeurs lorsque cela améliore la compréhension ou évite la duplication.

```csharp
// ❌ MAUVAIS - Valeur utilisée dans une condition sans contexte clair
if (daysSinceEvent > 4) { ... }

// ✅ BON - Valeur nommée pour clarifier l'intention
const int MaxDaysToUploadPhoto = 4;
if (daysSinceEvent > MaxDaysToUploadPhoto) { ... }
```

**NE PAS extraire SI** :
- Valeur par défaut d'une propriété (init/set)
- Message d'erreur ou de log unique
- Valeur évidente du contexte (0, 1, -1, "", etc.)

### Primary Constructors (C# 12)

**INTERDICTION TOTALE** : Ne jamais utiliser les Primary Constructors.

```csharp
// ❌ INTERDIT - Primary Constructor
public sealed class EventService(IEventRepository repository, ILogger<EventService> logger) { ... }

// ✅ BON - Constructeur classique avec injection
public sealed class EventService
{
    private readonly IEventRepository _repository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository repository, ILogger<EventService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}
```

### Logging
- Logger toutes les actions importantes (début, succès, erreur)
- Utiliser `ILogger<T>` injecté
- Niveaux : `LogInformation`, `LogWarning`, `LogError`, `LogDebug`

```csharp
_logger.LogInformation("Chargement des événements pour l'équipe {TeamId}", teamId);
_logger.LogError(ex, "Erreur lors de la création d'un événement");
```

---

## 🎨 Composants Blazor

### Composants autonomes
- Les composants doivent être **autonomes** et fonctionner uniquement avec des paramètres
- **AUCUN accès direct aux services** dans les composants enfants
- Toutes les données nécessaires doivent être passées via `[Parameter]`
- Communication parent-enfant via `EventCallback<T>`

```csharp
// ✅ BON - Composant autonome
[Parameter]
public EventViewModel Event { get; set; } = default!;

[Parameter]
public EventCallback<EventViewModel> OnDelete { get; set; }

// ❌ MAUVAIS - Accès direct à un service
[Inject]
private IEventService EventService { get; set; }
```

### Structure des composants
- `{Component}.razor` : markup uniquement
- `{Component}.razor.cs` : code-behind avec toute la logique
- `{Component}.razor.css` : styles scoped au composant

### Styles
- **TOUJOURS** utiliser des styles scoped (`.razor.css`)
- Ne pas utiliser de styles inline sauf exception justifiée
- Utiliser Bootstrap pour la structure, styles custom pour les détails

---

## 🎯 Conventions de nommage

### Suffixes obligatoires
- Services métier : `{Feature}Service` (ex: `EventService`)
- Services de vue : `{Feature}ViewService` (ex: `EventViewService`)
- Repositories : `{Entity}Repository` (ex: `EventRepository`)
- ViewModels : `{Feature}ViewModel` (ex: `EventViewModel`)
- Configurations DI : `{Feature}Configuration` (ex: `EventConfiguration`)

### Interfaces
- Préfixe `I` pour toutes les interfaces (ex: `IEventService`)
- Une interface par service pour faciliter les tests

### Fichiers et dossiers
- PascalCase pour tous les noms de fichiers
- Dossiers : `Components`, `Services`, `Models`, `Repository`, `Configurations`
- Pages : suffixe `Page` (ex: `EventPage.razor`)
- Composants : suffixe `Component` (ex: `EventCardComponent.razor`)

```csharp
// ✅ BON - Distinction claire
EventPage.razor          // Page complète avec accès aux services
EventCardComponent.razor // Composant autonome avec paramètres

// ❌ MAUVAIS
Event.razor              // Pas clair si c'est une page ou un composant
EventCard.razor          // Pas de suffixe
```

---

## 🗂️ Structuration des modèles

### Suffixes des modèles

| Suffixe | Rôle principal | Où l'utiliser | Quand l'utiliser |
|---------|----------------|---------------|------------------|
| **Db** | Données brutes de la base | Repository | Sortie de requête DB |
| **DTO** | Transfert de données | API, frontière système | Découpler domaine et exposition |
| **Command** | Action, modification | Application (CQRS) | Opération d'écriture |
| **Query** | Lecture, récupération | Application (CQRS) | Opération de lecture |
| **ViewModel** | Données pour la vue | Présentation | Adapter données à l'UI |
| **Form** | Saisie utilisateur | Présentation | Formulaire utilisateur |
| **Service** | Logique métier | Application/Domaine | Opérations métier |
| **Repository** | Accès données | Infrastructure | CRUD, persistance |
| **ViewService** | Préparer données vue | Présentation/Application | Composer/enrichir pour l'UI |

### Db (Database)

**Usage** : Objet venant de la base de données, de façon brute.

```csharp
public sealed record EventDb
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
}
```

### Command (CQRS)

**Principes de nommage** :
- Forme : `VerbeMétier + Objet + Command`
- Bannir les verbes techniques vagues (Save, Persist, Process)

```csharp
// ✅ BON
public sealed record CreateEventCommand { ... }
public sealed record AddRestaurantToEventCommand { ... }
public sealed record CloseEventVoteCommand { ... }

// ❌ MAUVAIS
public sealed record SaveEventCommand { ... }
public sealed record UpdateEventCommand { ... }
```

### Form

**Important** : Avoir des modèles différents entre création et mise à jour.

```csharp
// ✅ BON
public sealed record CreateEventForm
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime Date { get; set; }
}

public sealed record UpdateEventForm
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public string Name { get; set; }
}
```

---

## 🗄️ Standards SQL Server

### Mots-clés SQL
- **TOUJOURS** écrire les mots-clés SQL Server en MAJUSCULE
- Exemples : `SELECT`, `FROM`, `WHERE`, `INSERT`, `UPDATE`, `DELETE`, `JOIN`, `INNER JOIN`, `LEFT JOIN`, `CREATE`, `ALTER`, `DROP`, etc.

### Utilisation des crochets [ ]
- **NE PAS** abuser des crochets sur tous les champs
- **UNIQUEMENT** utiliser les crochets pour les noms qui sont des mots-clés SQL
- Exemples : `[Description]`, `[Role]`, `[User]`, `[Group]`, `[Order]`, `[Key]`, `[Type]`

### Valeurs par défaut (DEFAULT)

**INTERDICTION TOTALE** : Ne jamais utiliser de contraintes `DEFAULT` sur les colonnes.
Les valeurs par défaut doivent être gérées au niveau de l'application.

```sql
-- ❌ MAUVAIS
CREATE TABLE dbo.Event
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ...
);

-- ✅ BON - Valeurs gérées par l'application
CREATE TABLE dbo.Event
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    ...
);
```

### Schémas
- **TOUJOURS** préfixer les noms de tables avec le schéma : `dbo.`

### Convention de nommage des contraintes
- **Primary Key** : `PK_{NomTable}`
- **Foreign Key** : `FK_{NomTable}_{NomTableReferee}`
- **Unique** : `UK_{NomTable}_{NomColonne}`
- **Check** : `CK_{NomTable}_{NomColonne}`
- **Index** : `IX_{NomTable}_{NomColonne}`

```sql
-- ✅ BON
CREATE TABLE dbo.Event
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    TeamId INT NOT NULL,

    CONSTRAINT PK_Event
        PRIMARY KEY (Id),

    CONSTRAINT FK_Event_Team
        FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id)
);
```

### Indentation et lisibilité

```sql
-- ✅ BON - Conditions alignées
SELECT e.Id,
       e.Name,
       t.Name AS TeamName
FROM dbo.Event AS e
INNER JOIN dbo.Team AS t
    ON e.TeamId = t.Id
WHERE e.TeamId = @TeamId
  AND e.Date >= @DateStart
  AND e.IsActive = 1;

-- ❌ MAUVAIS - Tout sur une ligne
SELECT e.Id, e.Name FROM dbo.Event AS e WHERE e.TeamId = @TeamId AND e.IsActive = 1;
```

---

## 🚀 Bonnes pratiques spécifiques

### ViewModels
- Utiliser des `record` pour les ViewModels
- Immuables par défaut (utiliser `with` pour les modifications)
- Contiennent UNIQUEMENT l'état de la vue (pas de logique)

### Services de vue (ViewService)
- Gèrent l'état de la vue via un ViewModel
- Contiennent la logique de présentation (filtrage, formatage, etc.)
- Font le pont entre la page et les services métier
- Exposent le ViewModel via une propriété publique

### Async/Await
- **TOUJOURS** utiliser `async`/`await` pour les opérations IO
- Suffixe `Async` pour toutes les méthodes asynchrones
- Ne jamais bloquer avec `.Result` ou `.Wait()`
- **TOUJOURS** utiliser la version asynchrone d'une méthode si elle existe

```csharp
// ✅ BON - Versions async à privilégier
await _context.Events.ToListAsync();
await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
await _context.Events.AnyAsync(e => e.TeamId == teamId);
await _context.Events.CountAsync(e => e.IsActive);

// ❌ MAUVAIS
_context.Events.ToList();
_context.Events.FirstOrDefault(e => e.Id == id);
```

### Collections : IEnumerable vs List (Performance)

**Règle** : Matérialiser les collections avec `.ToList()` pour éviter les multiples énumérations.

```csharp
// ❌ MAUVAIS - Multiple énumérations
public IEnumerable<EventViewModel> GetActiveEvents()
{
    var events = _repository.GetEvents().Where(x => x.IsActive);
    if (events.Any())          // Première énumération
        return events.OrderBy(x => x.Date); // Sera ré-énuméré
    return Enumerable.Empty<EventViewModel>();
}

// ✅ BON - Une seule énumération
public List<EventViewModel> GetActiveEvents()
{
    List<EventViewModel> events = _repository.GetEvents()
                                             .Where(x => x.IsActive)
                                             .OrderBy(x => x.Date)
                                             .ToList();
    return events;
}
```

### Pattern matching
- Utiliser le pattern matching moderne de C#
- `switch` expressions pour la lisibilité

```csharp
// ✅ BON
var label = status switch
{
    EventStatus.Open   => "En cours",
    EventStatus.Closed => "Terminé",
    _                  => "Inconnu"
};
```

### Gestion des erreurs
- **TOUJOURS** utiliser des try-catch dans les méthodes publiques des services
- Logger les erreurs avant de les propager ou les transformer

```csharp
try
{
    await _service.CreateEventAsync(command);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erreur lors de la création de l'événement");
    throw;
}
```

### Sécurité et authentification
- Toujours vérifier l'authentification avec `AuthenticationStateProvider`
- Récupérer l'ID utilisateur via les claims
- Ne jamais faire confiance aux données côté client

```csharp
var authState = await _authStateProvider.GetAuthenticationStateAsync();
if (authState.User.Identity?.IsAuthenticated is not true)
    return null;

var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
```

---

## 📖 Lisibilité du code

### Indentation des conditions

```csharp
// ❌ MAUVAIS
if (event.Date < DateTime.Now || event.IsArchived || !event.Team.IsActive)

// ✅ BON
if (event.Date < DateTime.Now
    || event.IsArchived
    || !event.Team.IsActive)
```

**Règles** :
- Indenter verticalement les conditions multiples
- Mettre les opérateurs (`||`, `&&`) **en début de ligne**

### Indentation des requêtes LINQ

```csharp
// ❌ MAUVAIS
var restaurants = events.Where(e => e.TeamId == teamId).SelectMany(e => e.Restaurants).OrderBy(r => r.Name).ToList();

// ✅ BON
var restaurants = events.Where(e => e.TeamId == teamId)
                        .SelectMany(e => e.Restaurants)
                        .OrderBy(r => r.Name)
                        .ToList();
```

### KISS : Ne pas abuser du LINQ

Principe **KISS** : préférer des boucles simples plutôt que du LINQ complexe.

### Indentation des ternaires

```csharp
// ❌ MAUVAIS
var label = isActive ? "Actif" : "Inactif";

// ✅ BON (si expression longue)
var label = isActive
    ? "Actif"
    : "Inactif";
```

### Initialisation des variables au plus juste

```csharp
// ❌ MAUVAIS - Variable initialisée trop tôt
string message = string.Empty;
if (condition)
{
    message = "Succès";
    _logger.LogInformation(message);
}

// ✅ BON
if (condition)
{
    string message = "Succès";
    _logger.LogInformation(message);
}
```

### Organisation des méthodes dans une classe

**Ordre recommandé** :
1. Champs privés
2. Propriétés publiques
3. Constructeur(s)
4. Méthodes publiques
5. Méthodes protected (si applicable)
6. Méthodes private

### Limiter le nombre de paramètres des méthodes

Utiliser un objet Command/DTO plutôt que de multiples paramètres.

**Règle** : Plus de 3-4 paramètres dans une **méthode** → créer un objet Command/DTO

**Exception** : Les constructeurs pour l'injection de dépendances peuvent avoir plus de paramètres.

```csharp
// ❌ MAUVAIS
Task CreateEventAsync(string name, DateTime date, int teamId, string location, bool isPublic);

// ✅ BON
Task CreateEventAsync(CreateEventCommand command);
```

---

## 📌 Résumé : Les règles d'or

### Architecture et structure
1. ✅ **`sealed`** par défaut pour classes et records
2. ✅ **`is` / `is not`** pour les comparaisons (hors LINQ)
3. ✅ **Vertical Slice Architecture** - tout dans Features
4. ✅ **Pas de logique dans les vues** - tout dans le code-behind/services
5. ✅ **Composants autonomes** - paramètres uniquement, pas de services

### Documentation et qualité
6. ✅ **Documentation XML** - toutes les méthodes publiques
7. ✅ **EventCallback** - communication parent-enfant
8. ✅ **Logging** - toutes les actions importantes
9. ✅ **Try-catch** - gestion des erreurs partout
10. ✅ **Interfaces** - pour tous les services (testabilité)

### Standards de code
11. ✅ **Pas de Magic Numbers/Strings** - variables nommées
12. ✅ **INTERDICTION Primary Constructors** - constructeurs classiques uniquement
13. ✅ **Collections : ToList()** - matérialiser pour éviter multiples énumérations
14. ✅ **Async/Await** - toujours utiliser les versions async (ToListAsync, FirstOrDefaultAsync, etc.)

### Lisibilité et simplicité
15. ✅ **Indentation verticale** - conditions, LINQ, ternaires, méthodes chaînées
16. ✅ **KISS** - préférer la simplicité au LINQ complexe
17. ✅ **Extraire les traitements** - pas de logique dans les paramètres
18. ✅ **Initialisation au plus juste** - variables au plus proche de leur utilisation
19. ✅ **Organisation des méthodes** - public en haut, private en bas
20. ✅ **Command/DTO** - limiter le nombre de paramètres (max 3-4)

### Suffixes des modèles
21. ✅ **Db** - données brutes de la base (Repository)
22. ✅ **DTO** - transfert entre couches (API, frontières)
23. ✅ **Command** - actions qui modifient l'état (CQRS)
24. ✅ **Query** - lectures sans effet de bord (CQRS)
25. ✅ **ViewModel** - données formatées pour la vue
26. ✅ **Form** - saisie utilisateur avec validation
27. ✅ **Service** - logique métier
28. ✅ **Repository** - accès aux données
29. ✅ **ViewService** - préparation des données pour la vue
30. ✅ **Page** - page Blazor avec services
31. ✅ **Component** - composant Blazor autonome

### Standards SQL Server
32. ✅ **Mots-clés en MAJUSCULE** - SELECT, FROM, WHERE, etc.
33. ✅ **Crochets [ ] uniquement sur mots-clés SQL** - [Description], [Role]
34. ✅ **Schéma obligatoire** - dbo.TableName
35. ✅ **INTERDICTION DEFAULT** - pas de contraintes DEFAULT sur les colonnes
36. ✅ **Indentation claire** - aligner WHERE et AND
37. ✅ **Contraintes nommées** - PK_, FK_, UK_, CK_, IX_
38. ✅ **Alias significatifs** - courts mais compréhensibles

---

💡 **Note** : Ces règles garantissent la maintenabilité, la testabilité et la clarté du code. Si une règle doit être enfreinte pour une raison valable, documenter le pourquoi dans un commentaire explicatif.
