namespace RateMyResto.Features.Account.Services;

public interface ICreateAdminService
{
    /// <summary>
    /// Vérifie si un compte administrateur existe déjà dans la base de données.
    /// Si c'est le cas, cela signifie que l'administrateur a déjà été créé 
    /// et que la création d'un compte administrateur n'est pas nécessaire.
    /// </summary>
    /// <returns></returns>
    Task<bool> IsAdminExist();

    /// <summary>
    /// Crée un compte administrateur avec le mot de passe spécifié.
    /// </summary>
    /// <returns></returns>
    Task CreateAdminAsync();
}
