namespace RateMyResto.Features.Event.Models.Dbs;

public sealed record EventByUserDb
{
    /// <summary>
    /// Identifiant de l'événement
    /// </summary>
    public required Guid IdEvent { get; init; }

    /// <summary>
    /// Date de l'événement
    /// </summary>
    public required DateOnly DateEvent { get; init; }

    /// <summary>
    /// Identifiant du restaurant
    /// </summary>
    public required int IdRestaurant { get; init; }

    /// <summary>
    /// Nom du restaurant
    /// </summary>
    public required string RestaurantName { get; init; }

    /// <summary>
    /// Statut de participation de l'utilisateur à l'événement
    /// </summary>
    public required short ParticipationStatus { get; init; }

    /// <summary>
    /// Identifiant de l'équipe
    /// </summary>
    public required Guid IdEquipe { get; init; }

    /// <summary>
    /// Nom de l'équipe
    /// </summary>
    public required string EquipeName { get; init; }
}
