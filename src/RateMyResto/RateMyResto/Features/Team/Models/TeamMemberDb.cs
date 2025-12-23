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
}
