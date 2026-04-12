using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Mailing.Components;

/// <summary>
/// Composant Blazor rendu en HTML par BlazorMail pour la notification de création d'un événement.
/// Autonome : aucun service injecté, uniquement des paramètres.
/// </summary>
public sealed partial class NewEventEmailComponent : ComponentBase
{
    /// <summary>
    /// Nom d'affichage du destinataire (DisplayName ou UserName en fallback).
    /// </summary>
    [Parameter]
    public required string DisplayName { get; set; }

    /// <summary>
    /// Nom d'affichage de l'organisateur de l'événement.
    /// </summary>
    [Parameter]
    public required string NomOrganisateur { get; set; }

    /// <summary>
    /// Nom de l'équipe organisatrice.
    /// </summary>
    [Parameter]
    public required string NomEquipe { get; set; }

    /// <summary>
    /// Nom du restaurant de l'événement.
    /// </summary>
    [Parameter]
    public required string NomRestaurant { get; set; }

    /// <summary>
    /// Adresse du restaurant. Affichée uniquement si non vide.
    /// </summary>
    [Parameter]
    public required string AdresseRestaurant { get; set; }

    /// <summary>
    /// Date de l'événement.
    /// </summary>
    [Parameter]
    public required DateOnly DateEvenement { get; set; }

    /// <summary>
    /// URL complète vers la page des événements de l'équipe.
    /// Exemple : https://rate-my-resto.ctrl-alt-suppr.net/events
    /// </summary>
    [Parameter]
    public required string TeamEventsUrl { get; set; }

    /// <summary>
    /// Date formatée en français pour affichage dans le mail.
    /// </summary>
    private string FormattedDate => DateEvenement.ToString("dddd d MMMM yyyy",
                                                           CultureInfo.GetCultureInfo("fr-FR"));
}
