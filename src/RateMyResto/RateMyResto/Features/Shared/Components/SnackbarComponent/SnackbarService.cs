
namespace RateMyResto.Features.Shared.Components.SnackbarComponent;

/// <summary>
/// Implémentation du service de gestion des notifications Snackbar
/// </summary>
public sealed class SnackbarService : ISnackbarService
{
    /// <inheritdoc />
    public event Action<SnackbarMessage>? OnMessageAdded;

    /// <inheritdoc />
    public event Action<Guid>? OnMessageRemoved;

    /// <inheritdoc />
    public void ShowSuccess(string message, int duration = 5000)
    {
        ShowMessage(message, SnackbarType.Success, duration);
    }

    /// <inheritdoc />
    public void ShowError(string message, int duration = 5000)
    {
        ShowMessage(message, SnackbarType.Error, duration);
    }

    /// <inheritdoc />
    public void ShowWarning(string message, int duration = 5000)
    {
        ShowMessage(message, SnackbarType.Warning, duration);
    }

    /// <inheritdoc />
    public void ShowInfo(string message, int duration = 5000)
    {
        ShowMessage(message, SnackbarType.Info, duration);
    }

    /// <inheritdoc />
    public void RemoveMessage(Guid messageId)
    {
        OnMessageRemoved?.Invoke(messageId);
    }

    private void ShowMessage(string message, SnackbarType type, int duration)
    {
        SnackbarMessage snackbarMessage = new()
        {
            Message = message,
            Type = type,
            Duration = duration
        };

        OnMessageAdded?.Invoke(snackbarMessage);
    }
}

