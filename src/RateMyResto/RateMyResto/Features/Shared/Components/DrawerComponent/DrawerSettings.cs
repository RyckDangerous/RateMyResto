using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Shared.Components.DrawerComponent;

/// <summary>
/// Configuration et contenu du Drawer
/// </summary>
public sealed record DrawerSettings
{
    /// <summary>
    /// Identifiant unique du drawer
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Titre affiché dans le header du drawer
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Icône optionnelle pour le titre (classe Bootstrap Icons)
    /// </summary>
    public string? TitleIcon { get; set; }

    /// <summary>
    /// Contenu du drawer (RenderFragment sera passé dynamiquement)
    /// </summary>
    public RenderFragment? Content { get; set; }

    /// <summary>
    /// Position du drawer
    /// </summary>
    public DrawerPosition Position { get; set; } = DrawerPosition.Right;

    /// <summary>
    /// Largeur du drawer (en pourcentage ou pixels)
    /// </summary>
    public string Width { get; set; } = "25%";

    /// <summary>
    /// Largeur minimale
    /// </summary>
    public string MinWidth { get; set; } = "400px";

    /// <summary>
    /// Largeur maximale
    /// </summary>
    public string MaxWidth { get; set; } = "600px";

    /// <summary>
    /// Autoriser la fermeture en cliquant sur le backdrop
    /// </summary>
    public bool CloseOnBackdropClick { get; set; } = true;

    /// <summary>
    /// Afficher le bouton de fermeture
    /// </summary>
    public bool ShowCloseButton { get; set; } = true;
}
