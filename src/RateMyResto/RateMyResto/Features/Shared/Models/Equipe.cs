namespace RateMyResto.Features.Shared.Models;

public sealed record Equipe
{
    /// <summary>
    /// Identifiant unique de l'équipe
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Nom de l'équipe
    /// </summary>
    public required string Nom { get; set; }

    /// <summary>
    /// Description de l'équipe (optionnelle)
    /// </summary>
    public required string? Description { get; set; }

    /// <summary>
    /// Identifiant unique du propriétaire de l'équipe
    /// </summary>
    public required string IdOwner { get; set; }

    /// <summary>
    /// Nom du propriétaire de l'équipe
    /// </summary>
    public required string OwnerName { get; set; }

    /// <summary>
    /// Liste des membres de l'équipe
    /// </summary>
    public required List<Membre> Membres { get; set; }
}
