using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Shared.Components.DrawerComponent;

public partial class Drawer : ComponentBase
{
    private bool _isOpen = false;
    private DrawerSettings? _currentSettings = null;

    protected override void OnInitialized()
    {
        DrawerService.OnDrawerOpened += HandleDrawerOpened;
        DrawerService.OnDrawerClosed += HandleDrawerClosed;
    }

    /// <summary>
    /// Gère l'ouverture du tiroir avec les paramètres spécifiés.
    /// </summary>
    /// <param name="settings"></param>
    private void HandleDrawerOpened(DrawerSettings settings)
    {
        InvokeAsync(() =>
        {
            _currentSettings = settings;
            _isOpen = true;
            StateHasChanged();
        });
    }

    /// <summary>
    /// Gère la fermeture du tiroir.
    /// </summary>
    private void HandleDrawerClosed()
    {
        InvokeAsync(() =>
        {
            _isOpen = false;
            StateHasChanged();

            // Nettoyer après l'animation
            Task.Delay(300).ContinueWith(_ =>
            {
                InvokeAsync(() =>
                {
                    _currentSettings = null;
                    StateHasChanged();
                });
            });
        });
    }

    /// <summary>
    /// Ferme le tiroir.
    /// </summary>
    private void CloseDrawer()
    {
        DrawerService.Close();
    }

    /// <summary>
    /// Gère le clic sur le fond pour fermer le tiroir si l'option est activée.
    /// </summary>
    private void HandleBackdropClick()
    {
        if (_currentSettings?.CloseOnBackdropClick == true)
        {
            CloseDrawer();
        }
    }

    /// <summary>
    /// Obtient la classe CSS correspondant à la position du tiroir.
    /// </summary>
    /// <returns></returns>
    private string GetPositionClass() => _currentSettings?.Position switch
    {
        DrawerPosition.Right => "right",
        DrawerPosition.Left => "left",
        DrawerPosition.Top => "top",
        DrawerPosition.Bottom => "bottom",
        _ => "right"
    };

    /// <summary>
    /// Obtient le style CSS pour la taille du tiroir.
    /// </summary>
    /// <returns></returns>
    private string GetDrawerStyle()
    {
        if (_currentSettings == null) return string.Empty;

        var position = _currentSettings.Position;

        if (position == DrawerPosition.Right || position == DrawerPosition.Left)
        {
            return $"width: {_currentSettings.Width}; min-width: {_currentSettings.MinWidth}; max-width: {_currentSettings.MaxWidth};";
        }
        else
        {
            return $"height: {_currentSettings.Width}; min-height: {_currentSettings.MinWidth}; max-height: {_currentSettings.MaxWidth};";
        }
    }

    /// <summary>
    /// Nettoie les abonnements aux événements lors de la disposition du composant.
    /// </summary>
    public void Dispose()
    {
        DrawerService.OnDrawerOpened -= HandleDrawerOpened;
        DrawerService.OnDrawerClosed -= HandleDrawerClosed;
    }
}
