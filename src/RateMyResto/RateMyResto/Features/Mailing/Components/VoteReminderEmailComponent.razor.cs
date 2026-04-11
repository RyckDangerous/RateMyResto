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
    /// Nom d'affichage du destinataire.
    /// </summary>
    [Parameter]
    public required string DisplayName { get; set; }

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
    /// Date formatée en français pour affichage dans le mail.
    /// </summary>
    private string FormattedDate => DateEvenement.ToString("dddd d MMMM yyyy",
                                                           CultureInfo.GetCultureInfo("fr-FR"));
}
