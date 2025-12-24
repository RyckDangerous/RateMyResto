namespace RateMyResto.Features.Shared.Models;

public sealed record Membre
{
    /// <summary>
    /// Identifiant unique du membre
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Nom du membre
    /// </summary>
    public required string Nom { get; set; }
}
