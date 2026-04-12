using RateMyResto.Features.HomeFeature.Home.Models;
using RateMyResto.Features.HomeFeature.Home.Repositories;

namespace RateMyResto.Features.HomeFeature.Home.ViewServices;

/// <summary>
/// Service de vue pour la page d'accueil publique.
/// Charge et expose la liste des équipes.
/// </summary>
public sealed class HomeViewService : IHomeViewService
{
    private readonly IHomeRepository _repository;
    private readonly ILogger<HomeViewService> _logger;

    public HomeViewService(IHomeRepository repository, ILogger<HomeViewService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public List<TeamPublicViewModel> Teams { get; private set; } = new();

    /// <inheritdoc />
    public async Task LoadTeamsAsync()
    {
        _logger.LogInformation("Chargement des équipes pour la vitrine publique");

        ResultOf<List<TeamPublicDb>> result = await _repository.GetAllTeamsAsync();

        if (result.HasError)
        {
            _logger.LogError("Erreur lors du chargement des équipes publiques : {Error}", result.Error.Message);
            Teams = new List<TeamPublicViewModel>();
            return;
        }

        Teams = result.Value
                      .Select(db => new TeamPublicViewModel
                      {
                          Id = db.Id,
                          Nom = db.Nom,
                          Description = db.Description,
                          NombreMembres = db.NombreMembres,
                          NombreEvenements = db.NombreEvenements
                      })
                      .ToList();

        _logger.LogInformation("{Count} équipe(s) chargée(s) pour la vitrine", Teams.Count);
    }
}
