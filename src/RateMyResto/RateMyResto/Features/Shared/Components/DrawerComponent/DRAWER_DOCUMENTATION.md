# 🎨 Composant Drawer - Documentation

## Vue d'ensemble

Le composant **Drawer** (aussi appelé Offcanvas) est un panneau latéral réutilisable qui glisse depuis les bords de l'écran pour afficher du contenu de manière contextuelle sans navigation.

## 📁 Architecture

```
Features/Shared/Components/DrawerComponent/
├── Drawer.razor              # Composant UI
├── Drawer.razor.css          # Styles avec animations
├── DrawerSettings.cs         # Modèle de configuration
├── DrawerService.cs          # Implémentation du service
├── IDrawerService.cs         # Interface du service
└── DRAWER_DOCUMENTATION.md   # Ce fichier
```

## 🎯 Caractéristiques

✅ **4 positions** : Droite, Gauche, Haut, Bas  
✅ **Dimensions personnalisables** : Largeur/Hauteur, Min/Max  
✅ **Animations fluides** : Slide-in depuis n'importe quel côté  
✅ **Backdrop** : Fond semi-transparent avec fermeture optionnelle  
✅ **Header configurable** : Titre et icône optionnels  
✅ **Contenu dynamique** : RenderFragment pour tout type de contenu  
✅ **Responsive** : S'adapte aux mobiles (85% largeur)  
✅ **Scroll automatique** : Body scrollable avec scrollbar personnalisée  

## 🚀 Utilisation

### 1. Injection du service

```csharp
@inject IDrawerService DrawerService

// Ou dans un service/code-behind
public class MyService
{
    private readonly IDrawerService _drawerService;
    
    public MyService(IDrawerService drawerService)
    {
        _drawerService = drawerService;
    }
}
```

### 2. Méthode simple - Titre et contenu

```csharp
private void OpenDrawer()
{
    DrawerService.Open("Mon Titre", @<div>
        <p>Contenu du drawer</p>
        <button class="btn btn-primary">Action</button>
    </div>);
}
```

### 3. Avec icône

```csharp
private void OpenDrawer()
{
    DrawerService.Open(
        "Gestion de l'équipe",
        "bi-gear",  // Icône Bootstrap Icons
        @<div>
            <p>Détails de l'équipe...</p>
        </div>
    );
}
```

### 4. Configuration complète

```csharp
private void OpenDrawer()
{
    var settings = new DrawerSettings
    {
        Title = "Configuration avancée",
        TitleIcon = "bi-sliders",
        Position = DrawerPosition.Right,
        Width = "30%",
        MinWidth = "500px",
        MaxWidth = "800px",
        CloseOnBackdropClick = true,
        ShowCloseButton = true,
        Content = @<div>
            <h4>Section 1</h4>
            <p>Contenu...</p>
            
            <h4>Section 2</h4>
            <p>Plus de contenu...</p>
        </div>
    };
    
    DrawerService.Open(settings);
}
```

### 5. Fermeture programmatique

```csharp
private void CloseDrawer()
{
    DrawerService.Close();
}
```

## 📋 Exemple complet : Gestion d'équipe

### TeamPage.razor.cs

```csharp
using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Shared.Components.DrawerComponent;

public partial class TeamPage : ComponentBase
{
    [Inject]
    private IDrawerService DrawerService { get; set; } = default!;
    
    [Inject]
    private ISnackbarService SnackbarService { get; set; } = default!;
    
    private Equipe? _selectedTeam;
    
    private void OpenTeamDrawer(Guid teamId)
    {
        _selectedTeam = _viewService.ViewModel.OwnerEquipes
            .FirstOrDefault(e => e.Id == teamId);
            
        if (_selectedTeam == null) return;
        
        DrawerService.Open(
            "Gestion de l'équipe",
            "bi-gear",
            BuildTeamDrawerContent()
        );
    }
    
    private RenderFragment BuildTeamDrawerContent() => builder =>
    {
        builder.OpenElement(0, "div");
        
        // En-tête de l'équipe
        builder.OpenElement(1, "div");
        builder.AddAttribute(2, "class", "drawer-section");
        
        builder.OpenElement(3, "h4");
        builder.AddContent(4, _selectedTeam!.Nom);
        builder.CloseElement();
        
        if (!string.IsNullOrEmpty(_selectedTeam.Description))
        {
            builder.OpenElement(5, "p");
            builder.AddAttribute(6, "class", "text-muted fst-italic");
            builder.AddContent(7, _selectedTeam.Description);
            builder.CloseElement();
        }
        
        builder.CloseElement(); // drawer-section
        
        // Section Membres
        builder.OpenElement(8, "div");
        builder.AddAttribute(9, "class", "drawer-section");
        
        builder.OpenElement(10, "h6");
        builder.AddAttribute(11, "class", "drawer-section-title");
        builder.AddContent(12, $"Membres ({_selectedTeam.Membres.Count})");
        builder.CloseElement();
        
        builder.OpenElement(13, "ul");
        builder.AddAttribute(14, "class", "list-group");
        
        foreach (var membre in _selectedTeam.Membres)
        {
            builder.OpenElement(15, "li");
            builder.AddAttribute(16, "class", "list-group-item");
            builder.OpenElement(17, "i");
            builder.AddAttribute(18, "class", "bi bi-person me-2");
            builder.CloseElement();
            builder.AddContent(19, membre.Nom);
            builder.CloseElement();
        }
        
        builder.CloseElement(); // ul
        builder.CloseElement(); // drawer-section
        
        // Section Actions
        builder.OpenElement(20, "div");
        builder.AddAttribute(21, "class", "drawer-section");
        
        builder.OpenElement(22, "h6");
        builder.AddAttribute(23, "class", "drawer-section-title");
        builder.AddContent(24, "Actions");
        builder.CloseElement();
        
        builder.OpenElement(25, "div");
        builder.AddAttribute(26, "class", "d-grid gap-2");
        
        // Bouton Inviter
        builder.OpenElement(27, "button");
        builder.AddAttribute(28, "class", "btn btn-primary");
        builder.AddAttribute(29, "onclick", EventCallback.Factory.Create(this, InviteMember));
        builder.OpenElement(30, "i");
        builder.AddAttribute(31, "class", "bi bi-person-plus me-1");
        builder.CloseElement();
        builder.AddContent(32, "Inviter un membre");
        builder.CloseElement();
        
        // Bouton Modifier
        builder.OpenElement(33, "button");
        builder.AddAttribute(34, "class", "btn btn-outline-secondary");
        builder.AddAttribute(35, "onclick", EventCallback.Factory.Create(this, EditTeam));
        builder.OpenElement(36, "i");
        builder.AddAttribute(37, "class", "bi bi-pencil me-1");
        builder.CloseElement();
        builder.AddContent(38, "Modifier l'équipe");
        builder.CloseElement();
        
        // Bouton Supprimer
        builder.OpenElement(39, "button");
        builder.AddAttribute(40, "class", "btn btn-outline-danger");
        builder.AddAttribute(41, "onclick", EventCallback.Factory.Create(this, DeleteTeam));
        builder.OpenElement(42, "i");
        builder.AddAttribute(43, "class", "bi bi-trash me-1");
        builder.CloseElement();
        builder.AddContent(44, "Supprimer l'équipe");
        builder.CloseElement();
        
        builder.CloseElement(); // d-grid
        builder.CloseElement(); // drawer-section
        
        builder.CloseElement(); // div principal
    };
    
    private void InviteMember()
    {
        DrawerService.Close();
        SnackbarService.ShowInfo("Fonctionnalité d'invitation à venir...");
    }
    
    private void EditTeam()
    {
        DrawerService.Close();
        SnackbarService.ShowInfo("Fonctionnalité de modification à venir...");
    }
    
    private void DeleteTeam()
    {
        DrawerService.Close();
        SnackbarService.ShowWarning("Êtes-vous sûr de vouloir supprimer cette équipe ?");
    }
}
```

### TeamPage.razor

```razor
<!-- Dans la boucle des équipes propriétaires -->
<button class="btn btn-outline-primary btn-sm w-100" 
        @onclick="() => OpenTeamDrawer(equipe.Id)">
    <i class="bi bi-gear me-1"></i>Gérer l'équipe
</button>
```

## 🎨 Positions disponibles

```csharp
public enum DrawerPosition
{
    Right,   // Depuis la droite (défaut) - 25% largeur
    Left,    // Depuis la gauche
    Top,     // Depuis le haut
    Bottom   // Depuis le bas
}
```

### Exemples de positions

```csharp
// Drawer à gauche pour navigation
DrawerService.Open(new DrawerSettings
{
    Position = DrawerPosition.Left,
    Width = "300px",
    Title = "Navigation",
    Content = /* ... */
});

// Drawer en haut pour notifications
DrawerService.Open(new DrawerSettings
{
    Position = DrawerPosition.Top,
    Width = "200px",  // Hauteur dans ce cas
    Title = "Notifications",
    Content = /* ... */
});
```

## 🎯 Paramètres DrawerSettings

| Propriété | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `Title` | string? | null | Titre affiché dans le header |
| `TitleIcon` | string? | null | Classe d'icône Bootstrap (ex: "bi-gear") |
| `Content` | RenderFragment? | null | Contenu du drawer |
| `Position` | DrawerPosition | Right | Position du drawer |
| `Width` | string | "25%" | Largeur (ou hauteur si Top/Bottom) |
| `MinWidth` | string | "400px" | Largeur minimale |
| `MaxWidth` | string | "600px" | Largeur maximale |
| `CloseOnBackdropClick` | bool | true | Fermer en cliquant sur le backdrop |
| `ShowCloseButton` | bool | true | Afficher le bouton de fermeture |

## 💡 Bonnes pratiques

### ✅ À faire

- Utiliser pour afficher des détails ou des actions contextuelles
- Garder le contenu organisé en sections
- Utiliser des icônes pour améliorer la lisibilité
- Fermer le drawer après les actions importantes
- Combiner avec Snackbar pour les confirmations

### ❌ À éviter

- Charger trop de contenu (utiliser une page séparée à la place)
- Ouvrir plusieurs drawers simultanément
- Mettre des formulaires complexes (préférer une modal ou page)
- Utiliser pour navigation principale

## 🎨 Classes CSS utiles

```css
.drawer-section {
    margin-bottom: 1.5rem;
}

.drawer-section-title {
    font-size: 1rem;
    font-weight: 600;
    margin-bottom: 0.75rem;
    padding-bottom: 0.5rem;
    border-bottom: 1px solid #e9ecef;
}
```

## 📱 Responsive

- **Desktop** : Largeur configurée (défaut 25% = 1/4 écran)
- **Mobile (< 768px)** : 85% de la largeur pour Right/Left
- **Mobile (< 768px)** : 70% de la hauteur pour Top/Bottom

## 🔧 État du service

```csharp
// Vérifier si le drawer est ouvert
if (DrawerService.IsOpen)
{
    // Drawer actuellement affiché
}
```

## 🐛 Dépannage

**Problème** : Le drawer n'apparaît pas
- ✅ Vérifier que `AddSharedServices()` est appelé dans `Program.cs`
- ✅ Vérifier que `<Drawer />` est dans `MainLayout.razor`
- ✅ Vérifier l'injection du service

**Problème** : Le contenu ne se met pas à jour
- ✅ Utiliser `InvokeAsync` si appelé depuis un thread différent
- ✅ Vérifier que le RenderFragment est bien construit

**Problème** : Animation saccadée
- ✅ Éviter les rendus lourds dans le contenu
- ✅ Optimiser les images/ressources

## 🎭 Exemples de cas d'usage

1. **Détails d'un élément** (votre cas - équipe)
2. **Panier d'achat** (e-commerce)
3. **Filtres avancés** (recherche)
4. **Paramètres utilisateur**
5. **Historique/Notifications**
6. **Aide contextuelle**

---

**Status** : ✅ Production-ready  
**Version** : 1.0  
**Dernière mise à jour** : Décembre 2024

