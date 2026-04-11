using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Mailing.Components;

/// <summary>
/// Composant Blazor rendu en HTML par BlazorMail pour le rappel de vote.
/// Autonome : aucun service injecté, uniquement des paramètres.
/// </summary>
public sealed partial class VoteReminderEmailComponent : ComponentBase
{
    /// <summary>
    /// Nom d'affichage du destinataire (DisplayName ou UserName en fallback).
    /// </summary>
    [Parameter]
    public required string DisplayName { get; set; }

    /// <summary>
    /// Nom de l'équipe organisatrice de l'évènement.
    /// </summary>
    [Parameter]
    public required string NomEquipe { get; set; }

    /// <summary>
    /// Nom du restaurant de l'évènement à noter.
    /// </summary>
    [Parameter]
    public required string NomRestaurant { get; set; }

    /// <summary>
    /// Date de l'évènement à noter.
    /// </summary>
    [Parameter]
    public required DateOnly DateEvenement { get; set; }

    /// <summary>
    /// URL complète vers la page de détail de l'évènement.
    /// Exemple : https://rate-my-resto.ctrl-alt-suppr.net/event/detail/{guid}
    /// </summary>
    [Parameter]
    public required string EventDetailUrl { get; set; }

    /// <summary>
    /// Date formatée en français pour affichage dans le mail.
    /// </summary>
    private string FormattedDate => DateEvenement.ToString("dddd d MMMM yyyy",
                                                           CultureInfo.GetCultureInfo("fr-FR"));
}
