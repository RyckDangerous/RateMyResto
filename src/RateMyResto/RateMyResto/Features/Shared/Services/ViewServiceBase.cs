using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace RateMyResto.Features.Shared.Services;

public abstract class ViewServiceBase
{
    private readonly AuthenticationStateProvider _authStateProvider;

    /// <summary>
    /// Fonction pour rafraîchir l'interface utilisateur
    /// via le stateHasChanged du composant
    /// </summary>
    private Func<Task>? _refreshUi;


    protected ViewServiceBase(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Récupère l'ID de l'utilisateur actuellement authentifié.
    /// </summary>
    /// <returns></returns>
    protected async Task<string> GetCurrentUserIdAsync()
    {
        AuthenticationState authState = await _authStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return string.Empty;
        }

        string? userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if(string.IsNullOrEmpty(userId))
        {
            return string.Empty;
        }

        return userId;
    }

    /// <summary>
    /// Enregistre la fonction de rafraîchissement de l'interface utilisateur.
    /// </summary>
    /// <param name="refreshUi"></param>
    public void RegisterUiRefresh(Func<Task> refreshUi)
    {
        _refreshUi = refreshUi;
    }

    /// <summary>
    /// Notifie l'interface utilisateur de rafraîchir son état.
    /// </summary>
    /// <returns></returns>
    protected async Task RefreshUI()
    {
        if (_refreshUi is not null)
        {
            await _refreshUi();
        }
    }
}
