using Microsoft.AspNetCore.Components;
using RateMyResto.Features.Shared.Components.DrawerComponent;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Models;
using RateMyResto.Features.Team.Components;
using RateMyResto.Features.Team.Services;

namespace RateMyResto.Features.Team;

public partial class TeamPage : ComponentBase
{
    [Inject]
    private ITeamViewService _viewService { get; set; } = default!;

    [Inject]
    private IDrawerService DrawerService { get; set; } = default!;

    [Inject]
    private ISnackbarService SnackbarService { get; set; } = default!;

    // État de la modale
    private bool _showCreateModal = false;
    private string _newTeamName = string.Empty;
    private string? _newTeamDescription = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await _viewService.LoadViewModelAsync();
    }

    /// <summary>
    /// Ouvre la modale de création d'équipe.
    /// </summary>
    private void OpenCreateModal()
    {
        _showCreateModal = true;
        _newTeamName = string.Empty;
        _newTeamDescription = string.Empty;
    }

    /// <summary>
    /// Ferme la modale de création d'équipe.
    /// </summary>
    private void CloseCreateModal()
    {
        _showCreateModal = false;
        _newTeamName = string.Empty;
        _newTeamDescription = string.Empty;
    }

    /// <summary>
    /// Gère la création d'une nouvelle équipe.
    /// </summary>
    /// <returns></returns>
    private async Task HandleCreateTeam()
    {
        if (string.IsNullOrWhiteSpace(_newTeamName))
        {
            return;
        }

        await _viewService.CreateTeamAsync(_newTeamName, _newTeamDescription);
        CloseCreateModal();
        StateHasChanged();
    }

    /// <summary>
    /// Ouvre le drawer de gestion d'équipe.
    /// </summary>
    /// <param name="teamId">Identifiant de l'équipe</param>
    private void OpenTeamDrawer(Guid teamId)
    {
        _viewService.ViewModel.SelectedTeam = _viewService.ViewModel.OwnerEquipes.FirstOrDefault(e => e.Id == teamId);

        if (_viewService.ViewModel.SelectedTeam is null)
        {
            SnackbarService.ShowError("Équipe introuvable.");
            return;
        }

        // Créer le RenderFragment pour le contenu du drawer
        RenderFragment content = builder =>
        {
            builder.OpenComponent<TeamDrawerContent>(0);
            builder.AddAttribute(1, "Team", _viewService.ViewModel.SelectedTeam);
            builder.AddAttribute(2, "OnDeleteTeam", EventCallback.Factory.Create<Guid>(this, HandleDeleteTeam));
            builder.AddAttribute(3, "OnRemoveMember", EventCallback.Factory.Create<string>(this, HandleRemoveMember));
            builder.CloseComponent();
        };

        // Ouvrir le drawer avec le composant TeamDrawerContent
        DrawerService.Open("Gestion de l'équipe", "bi-gear", content);
    }

    /// <summary>
    /// Gère la suppression d'une équipe.
    /// </summary>
    private void HandleDeleteTeam(Guid teamId)
    {
        SnackbarService.ShowWarning("Fonctionnalité de suppression à venir...");
        // TODO: Implémenter la confirmation et la suppression
    }

    /// <summary>
    /// Gère le retrait d'un membre de l'équipe.
    /// </summary>
    /// <param name="userId">ID de l'utilisateur à retirer</param>
    private async Task HandleRemoveMember(string userId)
    {
        // Appeler le service pour retirer le membre
        await _viewService.RemoveMemberAsync(_viewService.ViewModel.SelectedTeam.Id, userId);

        // Recharger l'équipe dans le drawer
        _viewService.ViewModel.SelectedTeam = _viewService.ViewModel.OwnerEquipes
            .FirstOrDefault(e => e.Id == _viewService.ViewModel.SelectedTeam.Id);
        
        if (_viewService.ViewModel.SelectedTeam is not null)
        {
            // Rouvrir le drawer avec les données mises à jour
            OpenTeamDrawer(_viewService.ViewModel.SelectedTeam.Id);
        }
        else
        {
            DrawerService.Close();
        }
        
        StateHasChanged();
    }

    /// <summary>
    /// Gère le départ d'une équipe.
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private async Task HandleLeaveTeam(Guid teamId)
    {
        await _viewService.LeaveTeamAsync(teamId);
        StateHasChanged();
    }
}
