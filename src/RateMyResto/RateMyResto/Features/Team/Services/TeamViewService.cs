using RateMyResto.Features.Team.Converters;
using RateMyResto.Features.Team.Models;
using RateMyResto.Features.Team.Repositories;

namespace RateMyResto.Features.Team.Services;

public sealed class TeamViewService : ITeamViewService
{

    private readonly ITeamRepository _teamRepository;
    private ILogger<TeamViewService> _logger;


    /// <inheritdoc />
    public TeamViewModel ViewModel { get; private set; } 


    public TeamViewService(ITeamRepository teamRepository, ILogger<TeamViewService> logger)
    {
        _teamRepository = teamRepository;
        _logger = logger;

        ViewModel = new();
    }

    /// <inheritdoc />
    public async Task LoadViewModelAsync()
    {
        ViewModel.IsLoading = true;

        try
        {
            // Charger les équipes dont l'utilisateur est propriétaire
            ResultOf<List<TeamDb>> ownerTeamsResult = await _teamRepository.GetTeamByOwner("");

            if(ownerTeamsResult.HasError)
            {
                ViewModel.ErrorMessage = "Une erreur est survenue lors du chargement des équipes.";
                return;
            }

            // Charger les équipes dont l'utilisateur est membre
            ResultOf<List<TeamDb>> memberTeamsResult = await _teamRepository.GetTeamsByMember("");

            if(memberTeamsResult.HasError)
            {
                ViewModel.ErrorMessage = "Une erreur est survenue lors du chargement des équipes.";
                return;
            }

            ViewModel.OwnerEquipes = ownerTeamsResult.Value.Select(TeamDbConverters.ToEquipe).ToList();
            ViewModel.MemberEquipes = memberTeamsResult.Value.Select(TeamDbConverters.ToEquipe).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des équipes.");
        }

        ViewModel.IsLoading = false;
    }

}
