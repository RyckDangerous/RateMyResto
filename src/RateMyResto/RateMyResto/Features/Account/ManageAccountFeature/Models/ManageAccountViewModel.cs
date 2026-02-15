namespace RateMyResto.Features.Account.ManageAccountFeature.Models;

/// <summary>
/// ViewModel pour la page de gestion des comptes utilisateurs.
/// </summary>
public sealed class ManageAccountViewModel
{
    /// <summary>
    /// Indique si les données sont en cours de chargement.
    /// </summary>
    public bool IsLoading { get; set; }

    /// <summary>
    /// Indique si l'utilisateur connecté est administrateur.
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Liste des utilisateurs à afficher (exclut le compte admin).
    /// </summary>
    public List<UserItemViewModel> Users { get; set; } = [];
}
