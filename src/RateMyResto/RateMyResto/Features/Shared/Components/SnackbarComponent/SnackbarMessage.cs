namespace RateMyResto.Features.Shared.Components.SnackbarComponent;

/// <summary>
/// Représente un message à afficher dans le Snackbar
/// </summary>
public sealed class SnackbarMessage
{
    /// <summary>
    /// Identifiant unique du message
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Contenu du message
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Type de message (Success, Error, Warning, Info)
    /// </summary>
    public required SnackbarType Type { get; set; }

    /// <summary>
    /// Durée d'affichage en millisecondes (0 = infini)
    /// </summary>
    public int Duration { get; set; } = 5000;
}

/// <summary>
/// Type de message Snackbar
/// </summary>
public enum SnackbarType
{
    Success,
    Error,
    Warning,
    Info
}

