using RateMyResto.Features.Event.Attributs;
using System.ComponentModel.DataAnnotations;

namespace RateMyResto.Features.Event.Models;

public sealed record NewEventInput
{
    /// <summary>
    /// Identifiant du restaurant
    /// </summary>
    public required int? IdRestaurant { get; set; }

    /// <summary>
    /// Nom du restaurant
    /// </summary>
    [Required]
    [MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
    public required string NomDuRestaurant { get; set; }

    /// <summary>
    /// Adresse du restaurant
    /// </summary>
    [Required]
    [MaxLength(255, ErrorMessage = "Maximum 255 caractères")]
    public required string Adresse { get; set; }

    /// <summary>
    /// Url Google Maps du restaurant
    /// </summary>
    public required string? UrlGoogleMaps { get; set; }

    /// <summary>
    /// Date de l'évènement
    /// </summary>
    [Required]
    [DataType(DataType.Date)]
    [DateNotInPast]
    public required DateOnly DateEvenement { get; set; } = DateOnly.FromDateTime(DateTime.Today);

}
