using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.Shared.Services;
using RateMyResto.Features.Team.Converters;
using RateMyResto.Features.Team.Models;
using RateMyResto.Features.Team.Repositories;

namespace RateMyResto.Features.Team.Services;

public sealed class TeamViewService : ViewServiceBase, ITeamViewService
{

    private readonly ITeamRepository _teamRepository;
    private readonly ILogger<TeamViewService> _logger;


    /// <inheritdoc />
    public TeamViewModel ViewModel { get; private set; } 


    public TeamViewService(AuthenticationStateProvider authenticationStateProvider,
                            ITeamRepository teamRepository, 
                            ILogger<TeamViewService> logger)
        : base(authenticationStateProvider)
    {
        _teamRepository = teamRepository;
        _logger = logger;

        ViewModel = new();
    }

    /// <inheritdoc />
    public async Task LoadViewModelAsync()
    {
        ViewModel.IsLoading = true;

        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            ViewModel.ErrorMessage = "Utilisateur non authentifié.";
            return;
        }

        // Charge les équipes dont l'utilisateur est propriétaire
        ResultOf<List<TeamDb>> ownerTeamsResult = await _teamRepository.GetTeamByOwnerAsync(userId);

        if (ownerTeamsResult.HasError)
        {
            ViewModel.ErrorMessage = "Une erreur est survenue lors du chargement des équipes.";
            return;
        }

        ViewModel.OwnerEquipes = ownerTeamsResult.Value.Select(TeamDbConverters.ToEquipe).ToList();

        // Charge les équipes dont l'utilisateur est membre
        ResultOf<List<TeamDb>> memberTeamsResult = await _teamRepository.GetTeamsByMemberAsync(userId);

        if (memberTeamsResult.HasError)
        {
            ViewModel.ErrorMessage = "Une erreur est survenue lors du chargement des équipes.";
            return;
        }

        ViewModel.MemberEquipes = memberTeamsResult.Value.Select(TeamDbConverters.ToEquipe).ToList();
        ViewModel.IsLoading = false;
    }

    /// <inheritdoc />
    public async Task JoinTeamAsync(Guid teamId)
    {
        ViewModel.IsLoading = true;

        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            ViewModel.ErrorMessage = "Utilisateur non authentifié.";
            return;
        }

        ResultOf result = await _teamRepository.JoinTeamAsync(teamId, userId);

        if (result.HasError)
        {
            ViewModel.ErrorMessage = "Une erreur est survenue lors de la tentative de rejoindre l'équipe.";
            return;
        }

        ViewModel.IsLoading = false;
    }

    /// <inheritdoc />
    public async Task LeaveTeamAsync(Guid teamId)
    {
        ViewModel.IsLoading = true;

        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            ViewModel.ErrorMessage = "Utilisateur non authentifié.";
            return;
        }

        ResultOf result = await _teamRepository.DeleteTeamMemberAsync(teamId, userId);

        if (result.HasError)
        {
            ViewModel.ErrorMessage = "Une erreur est survenue lors de la tentative de quitter l'équipe.";
            return;
        }

        ViewModel.IsLoading = false;
    }
}
