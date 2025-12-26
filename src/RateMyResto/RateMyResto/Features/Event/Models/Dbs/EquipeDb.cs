namespace RateMyResto.Features.Event.Models.Dbs;

public sealed record EquipeDb
{
    /// <summary>
    /// Identifiant de l'équipe.
    /// </summary>
    public required Guid IdEquipe { get; init; }

    /// <summary>
    /// Nom de l'équipe.
    /// </summary>
    public required string NomEquipe { get; init; }
}
