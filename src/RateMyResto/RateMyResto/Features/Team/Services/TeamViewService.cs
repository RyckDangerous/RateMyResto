using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.Shared.Components.DrawerComponent;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Models;
using RateMyResto.Features.Shared.Services;
using RateMyResto.Features.Team.Components;
using RateMyResto.Features.Team.Converters;
using RateMyResto.Features.Team.Models;
using RateMyResto.Features.Team.Repositories;

namespace RateMyResto.Features.Team.Services;

public sealed class TeamViewService : ViewServiceBase, ITeamViewService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ISnackbarService _snackbarService;
    private readonly IDrawerService _drawerService;

    /// <inheritdoc />
    public TeamViewModel ViewModel { get; private set; }

    /// <inheritdoc />
    public bool ShowCreateModal { get; set; } = false;

    /// <inheritdoc />
    public string NewTeamName { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? NewTeamDescription { get; set; } = string.Empty;

    /// <inheritdoc />
    public bool ShowJoinModal { get; private set; } = false;

    /// <inheritdoc />
    public string TeamCodeToJoin { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? JoinErrorMessage { get; private set; } = null;


    public TeamViewService(AuthenticationStateProvider authenticationStateProvider,
                            ITeamRepository teamRepository,
                            ISnackbarService snackbarService,
                            IDrawerService drawerService)
        : base(authenticationStateProvider)
    {
        _teamRepository = teamRepository;
        _snackbarService = snackbarService;
        _drawerService = drawerService;
        ViewModel = new();
    }

    /// <summary>
    /// Enregistre une fonction de rafraîchissement de l'UI.
    /// </summary>
    /// <param name="refreshUi"></param>
    void IViewServiceBase.RegisterUiRefresh(Func<Task> refreshUi)
    {
        RegisterUiRefresh(refreshUi);
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
    public async Task RemoveMemberAsync(Guid teamId, string userId)
    {
        if (ViewModel.SelectedTeam is null)
            return;

        string? currentUserId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(currentUserId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            ViewModel.IsLoading = false;
            return;
        }

        // Vérifier que ce n'est pas le propriétaire
        if (ViewModel.SelectedTeam.IdOwner == userId)
        {
            _snackbarService.ShowWarning("Le propriétaire ne peut pas être retiré de l'équipe.");
            return;
        }

        ViewModel.IsLoading = true;

        // Appeler le repository pour retirer le membre
        ResultOf result = await _teamRepository.DeleteTeamMemberAsync(teamId, userId);

        if (result.HasError)
        {
            _snackbarService.ShowError("Une erreur est survenue lors du retrait du membre.");
            ViewModel.IsLoading = false;
            return;
        }

        _snackbarService.ShowSuccess("Le membre a été retiré de l'équipe avec succès.");
        
        // Recharge les équipes pour mettre à jour la liste des membres
        await LoadViewModelAsync();
    }

    /// <inheritdoc />
    public void OpenCreateModal()
    {
        ShowCreateModal = true;
        NewTeamName = string.Empty;
        NewTeamDescription = string.Empty;
    }

    /// <inheritdoc />
    public void CloseCreateModal()
    {
        ShowCreateModal = false;
        NewTeamName = string.Empty;
        NewTeamDescription = string.Empty;
    }

    /// <inheritdoc />
    public async Task HandleCreateTeam()
    {
        if (string.IsNullOrWhiteSpace(NewTeamName))
        {
            return;
        }

        await CreateTeamAsync(NewTeamName, NewTeamDescription);

        CloseCreateModal();
        await NotifyUiAsync();
    }

    /// <inheritdoc />
    public void OpenJoinModal()
    {
        ShowJoinModal = true;
        TeamCodeToJoin = string.Empty;
        JoinErrorMessage = null;
    }

    /// <inheritdoc />
    public void OpenTeamDrawer(Guid teamId)
    {
        ViewModel.SelectedTeam = ViewModel.OwnerEquipes.FirstOrDefault(e => e.Id == teamId);

        if (ViewModel.SelectedTeam is null)
        {
            _snackbarService.ShowError("Équipe introuvable.");
            return;
        }

        // Créer le RenderFragment pour le contenu du drawer
        RenderFragment content = builder =>
        {
            builder.OpenComponent<TeamDrawerContent>(0);
            builder.AddAttribute(1, "Team", ViewModel.SelectedTeam);
            builder.AddAttribute(2, "OnDeleteTeam", EventCallback.Factory.Create<Guid>(this, HandleDeleteTeam));
            builder.AddAttribute(3, "OnRemoveMember", EventCallback.Factory.Create<string>(this, HandleRemoveMember));
            builder.CloseComponent();
        };

        // Ouvrir le drawer avec le composant TeamDrawerContent
        _drawerService.Open("Gestion de l'équipe", "bi-gear", content);
    }

    /// <inheritdoc />
    public async Task HandleJoinTeam()
    {
        JoinErrorMessage = null;

        // Valider que le code n'est pas vide
        if (string.IsNullOrWhiteSpace(TeamCodeToJoin))
        {
            JoinErrorMessage = "Veuillez saisir un code d'équipe.";
            return;
        }

        // Valider que c'est un GUID valide
        if (!Guid.TryParse(TeamCodeToJoin, out Guid teamId))
        {
            JoinErrorMessage = "Le code d'équipe n'est pas valide. Il doit être un GUID.";
            return;
        }

        // Appeler le service pour rejoindre l'équipe
        await JoinTeamAsync(teamId);

        // Fermer la modale et rafraîchir
        CloseJoinModal();

        // Recharge les équipes après avoir quitté
        await LoadViewModelAsync();
        await NotifyUiAsync();
    }

    /// <inheritdoc />
    public void CloseJoinModal()
    {
        ShowJoinModal = false;
        TeamCodeToJoin = string.Empty;
        JoinErrorMessage = null;
    }

    /// <summary>
    /// Gère le départ d'une équipe.
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    public async Task HandleLeaveTeam(Guid teamId)
    {
        await LeaveTeamAsync(teamId);
        await NotifyUiAsync();
    }

    #region Private methods

    /// <summary>
    /// Gère la suppression d'une équipe.
    /// </summary>
    private void HandleDeleteTeam(Guid teamId)
    {
        _snackbarService.ShowWarning("Fonctionnalité de suppression à venir...");
        // TODO: Implémenter la confirmation et la suppression
    }

    /// <summary>
    /// Gère le retrait d'un membre de l'équipe.
    /// </summary>
    /// <param name="userId">ID de l'utilisateur à retirer</param>
    private async Task HandleRemoveMember(string userId)
    {
        if (ViewModel.SelectedTeam is null) 
            return;

        // Appeler le service pour retirer le membre
        await RemoveMemberAsync(ViewModel.SelectedTeam.Id, userId);

        // Recharger l'équipe dans le drawer
        ViewModel.SelectedTeam = ViewModel.OwnerEquipes
            .FirstOrDefault(e => e.Id == ViewModel.SelectedTeam.Id);

        if (ViewModel.SelectedTeam is not null)
        {
            // Rouvrir le drawer avec les données mises à jour
            OpenTeamDrawer(ViewModel.SelectedTeam.Id);
        }
        else
        {
            _drawerService.Close();
        }

        await NotifyUiAsync();
    }

    /// <summary>
    /// Crée une nouvelle équipe.
    /// </summary>
    /// <param name="nom"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    private async Task CreateTeamAsync(string nom, string? description)
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

    /// <summary>
    /// Quitte une équipe.
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private async Task LeaveTeamAsync(Guid teamId)
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

    /// <summary>
    /// Rejoint une équipe.
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private async Task JoinTeamAsync(Guid teamId)
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

    #endregion
}
