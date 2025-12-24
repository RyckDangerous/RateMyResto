using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Shared.Components.DrawerComponent;

/// <summary>
/// Service pour gérer l'affichage et la fermeture du Drawer
/// </summary>
public interface IDrawerService
{
    /// <summary>
    /// Événement déclenché quand le drawer doit être ouvert
    /// </summary>
    event Action<DrawerSettings>? OnDrawerOpened;

    /// <summary>
    /// Événement déclenché quand le drawer doit être fermé
    /// </summary>
    event Action? OnDrawerClosed;

    /// <summary>
    /// Ouvre le drawer avec les paramètres spécifiés
    /// </summary>
    void Open(DrawerSettings settings);

    /// <summary>
    /// Ouvre le drawer avec un titre et un contenu
    /// </summary>
    void Open(string title, RenderFragment content);

    /// <summary>
    /// Ouvre le drawer avec titre, icône et contenu
    /// </summary>
    void Open(string title, string titleIcon, RenderFragment content);

    /// <summary>
    /// Ferme le drawer
    /// </summary>
    void Close();

    /// <summary>
    /// Indique si le drawer est actuellement ouvert
    /// </summary>
    bool IsOpen { get; }
}

