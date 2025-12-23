using RateMyResto.Features.Shared.Models;

namespace RateMyResto.Features.Team.Models;

public sealed record TeamViewModel
{
    /// <summary>
    /// Indique si les données sont en cours de chargement
    /// </summary>
    public bool IsLoading { get; set; }

    /// <summary>
    /// Message d'erreur à afficher
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Liste des équipes ou l'utilisateur est Owner
    /// </summary>
    public List<Equipe> OwnerEquipes { get; set; } = new List<Equipe>();

    /// <summary>
    /// Liste des équipes ou l'utilisateur est membre
    /// </summary>
    public List<Equipe> MemberEquipes { get; set; } = new List<Equipe>();
}
