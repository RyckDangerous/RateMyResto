# 📢 Composant Snackbar - Documentation

## Vue d'ensemble

Le composant **Snackbar** est un système de notification réutilisable pour afficher des messages utilisateur (succès, erreur, avertissement, information) de manière élégante et non intrusive.

## 📁 Architecture

```
Features/Shared/
├── Components/
│   ├── Snackbar.razor          # Composant UI
│   └── Snackbar.razor.css      # Styles
├── Models/
│   └── SnackbarMessage.cs      # Modèle de message
├── Services/
│   ├── ISnackbarService.cs     # Interface du service
│   └── SnackbarService.cs      # Implémentation
└── Configurations/
    └── SharedConfigurationService.cs  # Configuration DI
```

## 🎨 Types de messages

Le Snackbar supporte 4 types de messages :

| Type | Couleur | Icône | Usage |
|------|---------|-------|-------|
| **Success** | Vert | ✓ | Confirmation d'action réussie |
| **Error** | Rouge | ⚠ | Erreur ou échec |
| **Warning** | Jaune/Orange | ⚠ | Avertissement |
| **Info** | Bleu | ℹ | Information générale |

## 🚀 Utilisation

### 1. Injection du service

Dans votre service ou composant :

```csharp
public class MyService
{
    private readonly ISnackbarService _snackbarService;

    public MyService(ISnackbarService snackbarService)
    {
        _snackbarService = snackbarService;
    }

    public void SomeMethod()
    {
        // Afficher un message de succès
        _snackbarService.ShowSuccess("Opération réussie !");
        
        // Afficher un message d'erreur
        _snackbarService.ShowError("Une erreur est survenue.");
        
        // Afficher un avertissement
        _snackbarService.ShowWarning("Attention, cette action est irréversible.");
        
        // Afficher une information
        _snackbarService.ShowInfo("Nouvelle fonctionnalité disponible !");
    }
}
```

### 2. Paramètres

Toutes les méthodes acceptent une durée d'affichage personnalisée (en millisecondes) :

```csharp
// Afficher pendant 3 secondes (par défaut : 5000ms)
_snackbarService.ShowSuccess("Message", 3000);

// Afficher indéfiniment (durée = 0, l'utilisateur doit fermer manuellement)
_snackbarService.ShowError("Erreur critique", 0);
```

### 3. Méthodes disponibles

```csharp
void ShowSuccess(string message, int duration = 5000);
void ShowError(string message, int duration = 5000);
void ShowWarning(string message, int duration = 5000);
void ShowInfo(string message, int duration = 5000);
void RemoveMessage(Guid messageId);
```

## 💡 Exemples concrets

### Exemple 1 : Création d'équipe (TeamViewService)

```csharp
public async Task CreateTeamAsync(string nom, string? description)
{
    // ... code de création ...
    
    if (result.HasError)
    {
        _snackbarService.ShowError("Une erreur est survenue lors de la création de l'équipe.");
        return;
    }
    
    _snackbarService.ShowSuccess($"L'équipe '{nom}' a été créée avec succès !");
}
```

### Exemple 2 : Quitter une équipe avec validation

```csharp
public async Task LeaveTeamAsync(Guid teamId)
{
    // Validation : propriétaire ne peut pas quitter
    if (currentTeam.IdOwner == userId)
    {
        _snackbarService.ShowWarning(
            "Le propriétaire de l'équipe ne peut pas la quitter. " +
            "Veuillez supprimer l'équipe à la place."
        );
        return;
    }
    
    // ... code pour quitter ...
    
    _snackbarService.ShowSuccess("Vous avez quitté l'équipe avec succès.");
}
```

## 🎯 Bonnes pratiques

### ✅ À faire

- Utiliser des messages clairs et concis
- Choisir le bon type de message selon le contexte
- Privilégier des durées courtes (3-5 secondes) pour les messages informatifs
- Utiliser une durée infinie (0) uniquement pour les erreurs critiques
- Ajouter des détails dans les messages de succès (ex: nom de l'élément créé)

### ❌ À éviter

- Messages trop longs (> 100 caractères)
- Afficher plusieurs snackbars simultanément pour la même action
- Utiliser Success pour des avertissements
- Messages techniques incompréhensibles pour l'utilisateur

## 🎨 Personnalisation du style

Le composant utilise Bootstrap Icons. Les couleurs sont définies dans `Snackbar.razor.css` :

```css
.snackbar-success { background-color: #d4edda; border-left: 4px solid #28a745; }
.snackbar-error   { background-color: #f8d7da; border-left: 4px solid #dc3545; }
.snackbar-warning { background-color: #fff3cd; border-left: 4px solid #ffc107; }
.snackbar-info    { background-color: #d1ecf1; border-left: 4px solid #17a2b8; }
```

## 📱 Responsive

Le Snackbar est entièrement responsive :
- **Desktop** : Apparaît en haut à droite, glisse horizontalement
- **Mobile** : Apparaît en haut, pleine largeur, glisse verticalement

## 🔧 Installation & Configuration

Le composant est automatiquement configuré si vous avez appelé dans `Program.cs` :

```csharp
builder.Services.AddSharedServices();
```

Et ajouté dans `MainLayout.razor` :

```razor
<Snackbar />
```

## ⚡ Fonctionnalités avancées

### Fermeture automatique

Les messages disparaissent automatiquement après la durée spécifiée.

### Fermeture manuelle

L'utilisateur peut fermer un message à tout moment en cliquant sur le bouton ✕.

### Empilage de messages

Plusieurs messages peuvent être affichés simultanément, empilés verticalement.

### Animations fluides

- Animation d'entrée : slide-in avec fade
- Animation de sortie : slide-out avec fade
- Transitions douces sur le hover du bouton de fermeture

## 🐛 Dépannage

**Problème** : Les messages n'apparaissent pas
- ✅ Vérifier que `AddSharedServices()` est appelé dans `Program.cs`
- ✅ Vérifier que `<Snackbar />` est dans `MainLayout.razor`
- ✅ Vérifier que le service est bien injecté

**Problème** : Les icônes ne s'affichent pas
- ✅ Vérifier que Bootstrap Icons est inclus dans `App.razor`

**Problème** : Les styles sont cassés
- ✅ Vérifier que le fichier CSS n'a pas été modifié
- ✅ Faire un clean & rebuild du projet

## 📊 Métriques UX

- **Durée par défaut** : 5 secondes (optimal pour la lecture)
- **Animations** : 300ms (fluide sans être trop lent)
- **Position** : Haut-droite (zone peu intrusive)
- **Z-index** : 9999 (toujours au-dessus)

---

**Status** : ✅ Production-ready  
**Version** : 1.0  
**Dernière mise à jour** : Décembre 2024

