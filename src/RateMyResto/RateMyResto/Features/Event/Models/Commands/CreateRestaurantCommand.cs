namespace RateMyResto.Features.Event.Models.Commands;

public sealed record CreateRestaurantCommand
{
    /// <summary>
    /// Nom du restaurant
    /// </summary>
    public required string Nom { get; init; }

    /// <summary>
    /// Adresse du restaurant
    /// </summary>
    public required string Adresse { get; init; }

    /// <summary>
    /// Url Google Maps du restaurant
    /// </summary>
    public string? UrlGoogleMaps { get; init; }
}
