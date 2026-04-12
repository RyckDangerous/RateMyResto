using RateMyResto.Features.HomeFeature.TeamsResto.Models;
using RateMyResto.Features.HomeFeature.TeamsResto.Repositories;

namespace RateMyResto.Features.HomeFeature.TeamsResto.ViewServices;

/// <summary>
/// Service de vue pour la page des sorties restaurant d'une équipe.
/// </summary>
public sealed class TeamsRestoViewService : ITeamsRestoViewService
{
    private readonly ITeamsRestoRepository _repository;
    private readonly ILogger<TeamsRestoViewService> _logger;

    public TeamsRestoViewService(ITeamsRestoRepository repository, ILogger<TeamsRestoViewService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public TeamsRestoViewModel? ViewModel { get; private set; }

    /// <inheritdoc />
    public async Task LoadAsync(Guid teamId)
    {
        _logger.LogInformation("Chargement des événements publics pour l'équipe {TeamId}", teamId);

        ResultOf<TeamEventsPublicDb> result = await _repository.GetTeamEventsAsync(teamId);

        if (result.HasError)
        {
            _logger.LogError("Erreur lors du chargement des événements pour l'équipe {TeamId} : {Error}",
                teamId, result.Error.Message);
            ViewModel = null;
            return;
        }

        TeamEventsPublicDb db = result.Value;

        ViewModel = new TeamsRestoViewModel
        {
            TeamId = db.TeamId,
            NomEquipe = db.NomEquipe,
            DescriptionEquipe = db.DescriptionEquipe,
            Evenements = db.Evenements ?? new List<EventPublicDb>()
        };

        _logger.LogInformation("{Count} événement(s) chargé(s) pour l'équipe {TeamId}",
            ViewModel.Evenements.Count, teamId);
    }
}
