using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace RateMyResto.Features.Shared.Services;

public abstract class ViewServiceBase
{
    private readonly AuthenticationStateProvider _authStateProvider;


    protected ViewServiceBase(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Récupère l'ID de l'utilisateur actuellement authentifié.
    /// </summary>
    /// <returns></returns>
    protected async Task<string?> GetCurrentUserIdAsync()
    {
        AuthenticationState authState = await _authStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return null;
        }

        return user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }

}
