# Règles de développement Front-end — Blazor

> Ces règles s'appliquent à tous les composants `.razor` et `.razor.cs` du dossier `Features/`.
> Elles complètent le `CLAUDE.md` racine sans le répéter.

---

## 🗂️ Page vs Component : distinction fondamentale

| | **Page** | **Component** |
|---|---|---|
| **Suffixe** | `Page` (ex: `EventPage.razor`) | `Component` (ex: `EventCardComponent.razor`) |
| **Directive** | `@page "/route"` | Aucune |
| **Services** | `@inject` autorisé | **INTERDIT** |
| **Données** | Chargées depuis les services | Reçues via `[Parameter]` uniquement |
| **Logique** | Dans le code-behind `.razor.cs` | Dans le code-behind `.razor.cs` |

```csharp
// ✅ BON - Page avec injection de service
// EventPage.razor
@page "/event/{Id:int}"
@inject IEventViewService EventViewService

// ✅ BON - Composant autonome, pas de @inject
// EventCardComponent.razor
<div class="card">@Event.Name</div>

// ❌ MAUVAIS - Service injecté dans un composant enfant
// EventCardComponent.razor
@inject IEventService EventService
```

---

## 📄 Structure des fichiers

Chaque composant ou page doit être découpé en **3 fichiers** :

```
EventPage.razor         ← markup uniquement, aucune logique
EventPage.razor.cs      ← toute la logique C#
EventPage.razor.css     ← styles scoped à ce composant
```

### Règles du fichier `.razor`

- **Uniquement du markup** : HTML, directives Razor (`@if`, `@foreach`, `@bind-Value`), appels de handlers
- **AUCUN bloc `@code { }`** sauf pour les composants simples sans `.razor.cs`
- **AUCUN calcul** dans le markup — extraire dans une propriété du code-behind

```razor
@* ✅ BON - Markup délègue tout au code-behind *@
<button @onclick="HandleSubmit" disabled="@IsLoading">
    @(IsLoading ? "Chargement..." : "Valider")
</button>

@foreach (var item in ViewModel.Events)
{
    <EventCardComponent Event="item" OnDelete="HandleDeleteEvent" />
}

@* ❌ MAUVAIS - Logique dans le markup *@
<button @onclick="async () => { IsLoading = true; await Service.SaveAsync(); IsLoading = false; }">
    Valider
</button>

@foreach (var item in events.Where(e => e.IsActive).OrderBy(e => e.Date))
```

### Règles du fichier `.razor.cs`

- La classe doit être `partial` et `sealed`
- Hériter de `ComponentBase` (explicitement)
- `[Parameter]` et `[CascadingParameter]` en haut, après les injections
- Méthodes publiques avant les méthodes privées

```csharp
// ✅ BON - Structure du code-behind
public sealed partial class EventPage : ComponentBase
{
    // 1. Injections (pages uniquement)
    [Inject]
    private IEventViewService EventViewService { get; set; } = default!;

    // 2. Paramètres
    [Parameter]
    public int Id { get; set; }

    // 3. État interne
    private EventViewModel ViewModel { get; set; } = new();
    private bool IsLoading { get; set; }

    // 4. Cycle de vie
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    // 5. Handlers publics (appelés depuis le markup)
    public async Task HandleSubmit() { ... }

    // 6. Méthodes privées
    private async Task LoadDataAsync() { ... }
}
```

---

## 🔗 Communication parent ↔ enfant

### Données parent → enfant : `[Parameter]`

```csharp
// ✅ BON
[Parameter]
public required EventViewModel Event { get; set; }

[Parameter]
public bool IsOwner { get; set; } = false;
```

### Actions enfant → parent : `EventCallback<T>`

- **TOUJOURS** utiliser `EventCallback<T>` (jamais `Action<T>` ou `Func<T, Task>`)
- Vérifier `HasDelegate` avant d'invoquer si le callback est optionnel
- Utiliser `InvokeAsync()` et non `Invoke()`

```csharp
// ✅ BON - EventCallback optionnel avec HasDelegate
[Parameter]
public EventCallback<int> OnDelete { get; set; }

private async Task HandleDelete()
{
    if (OnDelete.HasDelegate)
        await OnDelete.InvokeAsync(Event.Id);
}

// ✅ BON - EventCallback requis
[Parameter]
public EventCallback<IBrowserFile> OnPhotoSelected { get; set; }

private async Task HandleFileChange(InputFileChangeEventArgs e)
{
    await OnPhotoSelected.InvokeAsync(e.File);
}

// ❌ MAUVAIS - Action au lieu de EventCallback
[Parameter]
public Action<int> OnDelete { get; set; }
```

### Passer des arguments à un handler dans le markup

```razor
@* ✅ BON - Lambda pour passer des arguments *@
<button @onclick="() => HandleRemoveMember(member.Id)">Retirer</button>

@* ✅ BON - Lambda async *@
<button @onclick="async () => await HandleDeleteEvent(event.Id)">Supprimer</button>
```

---

## 🔄 Cycle de vie des composants

Utiliser les méthodes de cycle de vie dans cet ordre de préférence :

| Méthode | Quand l'utiliser |
|---------|-----------------|
| `OnInitializedAsync` | Chargement initial des données (le plus courant) |
| `OnParametersSetAsync` | Réaction à un changement de `[Parameter]` |
| `OnAfterRenderAsync` | Interop JS, actions nécessitant le DOM |
| `IDisposable.Dispose` | Désabonnement d'événements, nettoyage |

```csharp
// ✅ BON - Chargement initial
protected override async Task OnInitializedAsync()
{
    await LoadDataAsync();
}

// ✅ BON - Réaction au changement de paramètre
protected override async Task OnParametersSetAsync()
{
    if (Id != _previousId)
    {
        _previousId = Id;
        await LoadDataAsync();
    }
}
```

### Composants avec abonnement à des événements de service

```csharp
// ✅ BON - Pattern IDisposable complet
public sealed partial class SnackbarComponent : ComponentBase, IDisposable
{
    [Inject]
    private ISnackbarService SnackbarService { get; set; } = default!;

    protected override void OnInitialized()
    {
        SnackbarService.OnShow += HandleShow;
    }

    private async void HandleShow(string message)
    {
        // InvokeAsync garantit la thread-safety avec Blazor Server
        await InvokeAsync(() =>
        {
            _message = message;
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        SnackbarService.OnShow -= HandleShow;
    }
}
```

---

## 📋 Gestion des formulaires

### Structure d'un formulaire

```razor
@* ✅ BON *@
<EditForm Model="Form" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="mb-3">
        <label>Nom de l'événement</label>
        <InputText @bind-Value="Form.Name" class="form-control" />
        <ValidationMessage For="() => Form.Name" />
    </div>

    <button type="submit" disabled="@IsLoading">Créer</button>
</EditForm>
```

### Modèle de formulaire dédié

- **TOUJOURS** créer un objet `Form` dédié (suffixe `Form`)
- Ne jamais binder directement sur un ViewModel ou une entité DB

```csharp
// ✅ BON - Modèle dédié au formulaire dans Models/
public sealed record CreateEventForm
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date est obligatoire")]
    public DateTime? Date { get; set; }
}

// Dans le code-behind
private CreateEventForm Form { get; set; } = new();

private async Task HandleSubmit()
{
    IsLoading = true;
    try
    {
        CreateEventCommand command = new()
        {
            Name = Form.Name,
            Date = Form.Date!.Value
        };
        await EventService.CreateAsync(command);
    }
    finally
    {
        IsLoading = false;
    }
}
```

---

## 🔃 Mise à jour de l'UI

### `StateHasChanged` : quand et comment

- **Ne pas appeler** dans les handlers `@onclick`, `@onchange` — Blazor le fait automatiquement
- **Appeler** uniquement quand la mise à jour vient d'un événement externe (service, timer, etc.)
- **Toujours** via `InvokeAsync(StateHasChanged)` depuis un thread non-Blazor

```csharp
// ✅ BON - Mise à jour depuis un événement de service (thread externe)
private async void HandleExternalUpdate()
{
    await InvokeAsync(StateHasChanged);
}

// ❌ INUTILE - Blazor le fait déjà après un handler @onclick
private async Task HandleButtonClick()
{
    _count++;
    StateHasChanged(); // ← inutile ici
}
```

---

## 🎨 Styles

- **TOUJOURS** utiliser des styles scoped dans `.razor.css`
- **JAMAIS** de styles inline sauf exception justifiée et commentée
- Bootstrap pour la structure, `.razor.css` pour les personnalisations

```razor
@* ❌ MAUVAIS *@
<div style="color: red; margin-top: 10px;">...</div>

@* ✅ BON *@
<div class="event-card">...</div>
```

```css
/* EventCardComponent.razor.css */
.event-card {
    color: var(--bs-danger);
    margin-top: 0.625rem;
}
```

---

## 🖥️ Render modes

- Utiliser `@rendermode InteractiveServer` **uniquement** quand de l'interactivité est nécessaire (événements, état, websocket)
- Les pages statiques (rendu serveur sans interactivité) n'ont pas besoin de `@rendermode`

```razor
@* ✅ BON - Composant interactif nécessitant des mises à jour en temps réel *@
@rendermode InteractiveServer

@* Pas de @rendermode pour les pages statiques *@
```

---

## 🔒 Autorisation dans les vues

```razor
@* ✅ BON - Utiliser AuthorizeView pour conditionner l'affichage *@
<AuthorizeView>
    <Authorized>
        <button @onclick="HandleDelete">Supprimer</button>
    </Authorized>
</AuthorizeView>

@* ✅ BON - Protéger une page entière *@
@attribute [Authorize]
```

---

## 📌 Résumé : Les règles d'or du front

1. ✅ **Markup = affichage uniquement** — aucune logique, aucun calcul dans `.razor`
2. ✅ **Pages** : `@inject` OK, charge les données depuis les services
3. ✅ **Components** : zéro `@inject`, tout via `[Parameter]`
4. ✅ **`EventCallback<T>`** — communication enfant → parent, toujours avec `InvokeAsync`
5. ✅ **Formulaires** : objet `Form` dédié, jamais binder sur ViewModel/entité DB
6. ✅ **`sealed partial`** sur toutes les classes code-behind
7. ✅ **`IDisposable`** — désabonner tous les événements de service dans `Dispose()`
8. ✅ **`InvokeAsync(StateHasChanged)`** — pour les mises à jour depuis des threads externes
9. ✅ **Styles scoped** — `.razor.css`, jamais de styles inline
10. ✅ **`HasDelegate`** — vérifier avant d'invoquer un `EventCallback` optionnel

---

💡 **Note** : Si une règle doit être enfreinte pour une raison valable, documenter le pourquoi dans un commentaire explicatif.
