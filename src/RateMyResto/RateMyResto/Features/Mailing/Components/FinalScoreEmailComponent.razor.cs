using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Mailing.Components;

/// <summary>
/// Composant Blazor rendu en HTML par BlazorMail pour la notification de note finale.
/// Autonome : aucun service injecté, uniquement des paramètres.
/// </summary>
public sealed partial class FinalScoreEmailComponent : ComponentBase
{
    /// <summary>
    /// Nom d'affichage du destinataire (DisplayName ou UserName en fallback).
    /// </summary>
    [Parameter]
    public required string DisplayName { get; set; }

    /// <summary>
    /// Nom du restaurant de l'événement.
    /// </summary>
    [Parameter]
    public required string NomRestaurant { get; set; }

    /// <summary>
    /// Nom de l'équipe organisatrice.
    /// </summary>
    [Parameter]
    public required string NomEquipe { get; set; }

    /// <summary>
    /// Date de l'événement.
    /// </summary>
    [Parameter]
    public required DateOnly DateEvenement { get; set; }

    /// <summary>
    /// Note globale officielle (moyenne des participants confirmés).
    /// </summary>
    [Parameter]
    public required decimal NoteGlobale { get; set; }

    /// <summary>
    /// URL complète vers la page de détail de l'événement.
    /// Exemple : https://rate-my-resto.ctrl-alt-suppr.net/event/detail/guid
    /// </summary>
    [Parameter]
    public required string EventDetailUrl { get; set; }

    /// <summary>
    /// Date formatée en français pour affichage dans le mail.
    /// </summary>
    private string FormattedDate => DateEvenement.ToString("dddd d MMMM yyyy",
                                                           CultureInfo.GetCultureInfo("fr-FR"));

    /// <summary>
    /// Couleur hexadécimale du badge de score selon la tranche de note.
    /// </summary>
    private string ScoreColor => NoteGlobale >= 4m ? "#16a34a"
                               : NoteGlobale >= 3m ? "#0d9488"
                               : NoteGlobale >= 2m ? "#f97316"
                               : "#dc2626";

    /// <summary>
    /// Libellé de verdict selon la tranche de note.
    /// </summary>
    private string ScoreLabel => NoteGlobale >= 4m ? "Excellent !"
                               : NoteGlobale >= 3m ? "Très bien !"
                               : NoteGlobale >= 2m ? "Peut mieux faire"
                               : "Décevant...";

    /// <summary>
    /// Couleur hexadécimale des étoiles pleines.
    /// </summary>
    private const string StarFilledColor = "#f59e0b";

    /// <summary>
    /// Couleur hexadécimale des étoiles vides.
    /// </summary>
    private const string StarEmptyColor = "#d1d5db";

    /// <summary>
    /// Nombre d'étoiles pleines (arrondi à l'entier le plus proche).
    /// </summary>
    private int FilledStars => (int)Math.Round(NoteGlobale);

    /// <summary>
    /// Nombre d'étoiles vides.
    /// </summary>
    private int EmptyStars => 5 - FilledStars;
}
