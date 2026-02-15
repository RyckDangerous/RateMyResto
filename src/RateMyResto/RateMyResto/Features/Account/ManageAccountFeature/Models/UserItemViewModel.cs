namespace RateMyResto.Features.Account.ManageAccountFeature.Models;

/// <summary>
/// Représente un utilisateur affiché dans la liste de gestion des comptes.
/// </summary>
public sealed record UserItemViewModel
{
    /// <summary>
    /// Identifiant unique de l'utilisateur.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Nom d'utilisateur.
    /// </summary>
    public required string UserName { get; init; }
}
