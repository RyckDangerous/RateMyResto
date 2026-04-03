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

    /// <summary>
    /// Indique si le membre est actif dans l'équipe (DateFinPresence IS NULL).
    /// Un membre inactif n'est plus invité aux nouveaux événements mais son historique est conservé.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
