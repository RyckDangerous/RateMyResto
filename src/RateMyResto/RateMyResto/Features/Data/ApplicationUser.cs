using Microsoft.AspNetCore.Identity;

namespace RateMyResto.Features.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nom de l'utilisateur à afficher dans l'application. 
    /// Peut être différent du nom d'utilisateur utilisé pour la connexion.
    /// </summary>
    public string? DisplayName { get; set; }
}

