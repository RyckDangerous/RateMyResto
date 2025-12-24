using RateMyResto.Features.Shared.Models;

namespace RateMyResto.Features.Shared.Components.SnackbarComponent;

/// <summary>
/// Service pour gérer l'affichage des notifications Snackbar
/// </summary>
public interface ISnackbarService
{
    /// <summary>
    /// Événement déclenché quand un nouveau message est ajouté
    /// </summary>
    event Action<SnackbarMessage>? OnMessageAdded;

    /// <summary>
    /// Événement déclenché quand un message doit être supprimé
    /// </summary>
    event Action<Guid>? OnMessageRemoved;

    /// <summary>
    /// Affiche un message de succès
    /// </summary>
    void ShowSuccess(string message, int duration = 5000);

    /// <summary>
    /// Affiche un message d'erreur
    /// </summary>
    void ShowError(string message, int duration = 5000);

    /// <summary>
    /// Affiche un message d'avertissement
    /// </summary>
    void ShowWarning(string message, int duration = 5000);

    /// <summary>
    /// Affiche un message d'information
    /// </summary>
    void ShowInfo(string message, int duration = 5000);

    /// <summary>
    /// Supprime un message spécifique
    /// </summary>
    void RemoveMessage(Guid messageId);
}

