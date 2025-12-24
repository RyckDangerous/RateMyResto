# 🎯 Guide d'utilisation du Drawer - Approche propre avec composant Blazor

## ✅ Bonne pratique : Créer un composant Blazor pour le contenu

Au lieu de construire le HTML en C# avec RenderFragment Builder, **créez toujours un composant Blazor séparé** !

## 📋 Exemple complet : TeamDrawerContent

### 1. Créer le composant de contenu

**`Features/Team/Components/TeamDrawerContent.razor`**

```razor
@using RateMyResto.Features.Shared.Models
@using RateMyResto.Features.Shared.Components.DrawerComponent
@inject IDrawerService DrawerService

<!-- Votre contenu HTML propre et lisible -->
<div class="drawer-section">
    <h4>@Team.Nom</h4>
    <p class="text-muted">@Team.Description</p>
</div>

<div class="drawer-section">
    <h6 class="drawer-section-title">Membres</h6>
    <ul class="list-group">
        @foreach (var membre in Team.Membres)
        {
            <li class="list-group-item">@membre.Nom</li>
        }
    </ul>
</div>

<div class="drawer-section">
    <button class="btn btn-primary" @onclick="HandleAction">
        Action
    </button>
</div>

@code {
    [Parameter]
    public required Equipe Team { get; set; }
    
    [Parameter]
    public EventCallback<Guid> OnAction { get; set; }
    
    private async Task HandleAction()
    {
        DrawerService.Close();
        await OnAction.InvokeAsync(Team.Id);
    }
}
```

### 2. Utiliser le composant dans votre page

**`TeamPage.razor.cs`**

```csharp
[Inject]
private IDrawerService DrawerService { get; set; } = default!;

private Equipe? _selectedTeam;

private void OpenTeamDrawer(Guid teamId)
{
    _selectedTeam = _viewService.ViewModel.OwnerEquipes
        .FirstOrDefault(e => e.Id == teamId);
    
    if (_selectedTeam == null) return;
    
    // Créer le RenderFragment avec le composant
    RenderFragment content = builder =>
    {
        builder.OpenComponent<TeamDrawerContent>(0);
        builder.AddAttribute(1, "Team", _selectedTeam);
        builder.AddAttribute(2, "OnAction", EventCallback.Factory.Create<Guid>(this, HandleAction));
        builder.CloseComponent();
    };
    
    DrawerService.Open("Titre", "bi-gear", content);
}

private void HandleAction(Guid teamId)
{
    // Votre logique
}
```

## 🎨 Avantages de cette approche

✅ **Séparation des préoccupations** : HTML dans `.razor`, logique dans `.cs`
✅ **Lisibilité** : Code HTML propre et maintenable
✅ **IntelliSense** : Support complet de l'éditeur
✅ **Réutilisable** : Le composant peut être testé indépendamment
✅ **Pas de RenderFragment Builder manuel** : Plus de code complexe !

## 🚫 À éviter

❌ **NE PAS faire** : Construire le HTML en C# avec RenderFragment Builder (complexe et illisible)
❌ **NE PAS mettre** du HTML dans le ViewService
❌ **NE PAS utiliser** `@<Component />` dans un fichier `.cs` (erreur de compilation)

## ✅ Résumé de l'implémentation

1. **Créer** un composant Blazor pour le contenu du Drawer
2. **Passer** les données via `[Parameter]`
3. **Utiliser** des `EventCallback` pour les actions
4. **Construire** le RenderFragment avec `builder.OpenComponent<>()`
5. **Appeler** `DrawerService.Open()` avec le RenderFragment

C'est propre, maintenable et suit les meilleures pratiques Blazor ! 🎉

