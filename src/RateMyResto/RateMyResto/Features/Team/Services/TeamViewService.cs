using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Models;
using RateMyResto.Features.Shared.Services;
using RateMyResto.Features.Team.Converters;
using RateMyResto.Features.Team.Models;
using RateMyResto.Features.Team.Repositories;

namespace RateMyResto.Features.Team.Services;

public sealed class TeamViewService : ViewServiceBase, ITeamViewService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ISnackbarService _snackbarService;

    /// <inheritdoc />
    public TeamViewModel ViewModel { get; private set; } 


    public TeamViewService(AuthenticationStateProvider authenticationStateProvider,
                            ITeamRepository teamRepository,
                            ISnackbarService snackbarService)
        : base(authenticationStateProvider)
    {
        _teamRepository = teamRepository;
        _snackbarService = snackbarService;
        ViewModel = new();
    }

    /// <inheritdoc />
    public async Task LoadViewModelAsync()
    {
        ViewModel.IsLoading = true;

        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            ViewModel.IsLoading = false;
            return;
        }

        // Charge les équipes dont l'utilisateur est propriétaire
        ResultOf<List<TeamDb>> ownerTeamsResult = await _teamRepository.GetTeamByOwnerAsync(userId);

        if (ownerTeamsResult.HasError)
        {
            // Si l'erreur est une NotFoundError, cela signifie que l'utilisateur n'est propriétaire d'aucune équipe
            if (ownerTeamsResult.Error is NotFoundError)
            {
                ViewModel.OwnerEquipes = new List<Equipe>();
            }
            else
            {
                _snackbarService.ShowError("Une erreur est survenue lors du chargement des équipes.");
            }
        }
        else
        {
            ViewModel.OwnerEquipes = ownerTeamsResult.Value.Select(TeamDbConverters.ToEquipe).ToList();
        }

        // Charge les équipes dont l'utilisateur est membre
        ResultOf<List<TeamDb>> memberTeamsResult = await _teamRepository.GetTeamsByMemberAsync(userId);

        if (memberTeamsResult.HasError)
        {
            if(memberTeamsResult.Error is NotFoundError)
            {
                ViewModel.MemberEquipes = new List<Equipe>();
            }
            else
            {
                _snackbarService.ShowError("Une erreur est survenue lors du chargement des équipes.");
            }
        }
        else
        {
            ViewModel.MemberEquipes = memberTeamsResult.Value.Select(TeamDbConverters.ToEquipe).ToList();
        }

        ViewModel.IsLoading = false;
    }

    /// <inheritdoc />
    public async Task JoinTeamAsync(Guid teamId)
    {
        ViewModel.IsLoading = true;

        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            ViewModel.IsLoading = false;
            return;
        }

        ResultOf result = await _teamRepository.JoinTeamAsync(teamId, userId);

        if (result.HasError)
        {
            _snackbarService.ShowError("Une erreur est survenue lors de la tentative de rejoindre l'équipe.");
        }
        else
        {
            _snackbarService.ShowSuccess("Vous avez rejoint l'équipe avec succès !");
        }

        ViewModel.IsLoading = false;
    }

    /// <inheritdoc />
    public async Task LeaveTeamAsync(Guid teamId)
    {
        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        // Vérifie si l'utilisateur est le propriétaire de l'équipe
        Equipe? currentTeam = ViewModel.MemberEquipes.Where(e => e.Id == teamId).FirstOrDefault();
        if (currentTeam is null)
        {
            _snackbarService.ShowError("Équipe introuvable.");
            return;
        }

        if (currentTeam.IdOwner == userId)
        {
            _snackbarService.ShowWarning("Le propriétaire de l'équipe ne peut pas la quitter. Veuillez supprimer l'équipe à la place.");
            return;
        }

        ViewModel.IsLoading = true;

        ResultOf result = await _teamRepository.DeleteTeamMemberAsync(teamId, userId);

        if (result.HasError)
        {
            _snackbarService.ShowError("Une erreur est survenue lors de la tentative de quitter l'équipe.");
            ViewModel.IsLoading = false;
        }
        else
        {
            _snackbarService.ShowSuccess("Vous avez quitté l'équipe avec succès.");
            // Recharge les équipes après avoir quitté
            await LoadViewModelAsync();
        }
    }

    /// <inheritdoc />
    public async Task CreateTeamAsync(string nom, string? description)
    {
        ViewModel.IsLoading = true;
        ViewModel.ErrorMessage = null;

        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            ViewModel.IsLoading = false;
            return;
        }

        TeamCommand command = new()
        {
            IdTeam = Guid.NewGuid(),
            Nom = nom,
            Description = description,
            Owner = Guid.Parse(userId)
        };

        ResultOf result = await _teamRepository.CreateTeamAsync(command);

        if (result.HasError)
        {
            _snackbarService.ShowError("Une erreur est survenue lors de la création de l'équipe.");
            ViewModel.IsLoading = false;
            return;
        }

        _snackbarService.ShowSuccess($"L'équipe '{nom}' a été créée avec succès !");
        // Recharge les équipes après création
        await LoadViewModelAsync();
    }
}
