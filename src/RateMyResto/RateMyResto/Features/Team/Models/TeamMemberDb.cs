namespace RateMyResto.Features.Team.Models;

public sealed record TeamMemberDb
{
    /// <summary>
    /// Identifiant unique du membre de l'équipe
    /// </summary>
    public required string IdUser { get; set; }

    /// <summary>
    /// Nom d'utilisateur du membre de l'équipe
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Nom d'affichage du membre de l'équipe
    /// </summary>
    public required string? DisplayName { get; set; }

    /// <summary>
    /// Date de fin de présence dans l'équipe.
    /// Null = membre actif. Renseignée = membre inactif depuis cette date.
    /// </summary>
    public required DateOnly? DateFinPresence { get; set; }
}
