namespace RateMyResto.Features.EventDetail.Models;

public sealed record RestaurantViewModel
{
    /// <summary>
    /// Nom du restaurant.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Adresse du restaurant.
    /// </summary>
    public required string Adresse { get; init; }

    /// <summary>
    /// Lien Google Maps du restaurant.
    /// </summary>
    public required string? LienGoogleMaps { get; init; }
}
