using RateMyResto.Core.Models;
using RateMyResto.Features.Account.ManageAccountFeature.Models;
using RateMyResto.Features.Account.ManageAccountFeature.Repositories;
using RateMyResto.Features.Account.Shared.Constantes;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.Account.ManageAccountFeature.Services;

/// <summary>
/// ViewService pour la page de gestion des comptes.
/// </summary>
public sealed class ManageAccountViewService : ViewServiceBase, IManageAccountViewService
{
    private readonly IManageAccountRepository _manageAccountRepository;
    private readonly ISnackbarService _snackbarService;

    /// <inheritdoc />
    public ManageAccountViewModel ViewModel { get; } = new();

    /// <inheritdoc />
    public bool ShowResetPasswordModal { get; private set; }

    /// <inheritdoc />
    public string? SelectedUserId { get; private set; }

    /// <inheritdoc />
    public string? SelectedUserName { get; private set; }

    /// <inheritdoc />
    public string NewPassword { get; set; } = string.Empty;

    /// <inheritdoc />
    public bool IsNewPasswordValid =>
        NewPassword.Length >= 6
        && NewPassword.Any(char.IsDigit)
        && NewPassword.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

    public ManageAccountViewService(
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authenticationStateProvider,
        IManageAccountRepository manageAccountRepository,
        ISnackbarService snackbarService)
        : base(authenticationStateProvider)
    {
        _manageAccountRepository = manageAccountRepository;
        _snackbarService = snackbarService;
    }

    /// <inheritdoc />
    void IViewServiceBase.RegisterUiRefresh(Func<Task> refreshUi)
    {
        RegisterUiRefresh(refreshUi);
    }

    /// <inheritdoc />
    public async Task LoadUsersAsync()
    {
        ViewModel.IsLoading = true;
        ViewModel.IsAdmin = false;

        string? currentUserName = await GetCurrentUserNameAsync();
        if (string.IsNullOrEmpty(currentUserName) 
            || !currentUserName.Equals(UserConstantes.AdminUserName, StringComparison.OrdinalIgnoreCase))
        {
            ViewModel.IsAdmin = false;
            ViewModel.Users = [];
            ViewModel.IsLoading = false;
            await RefreshUI();
            return;
        }

        ViewModel.IsAdmin = true;

        ResultOf<List<UserItemViewModel>> result = await _manageAccountRepository.GetUsersExceptAdminAsync();

        if (result.HasError)
        {
            _snackbarService.ShowError("Erreur lors du chargement des utilisateurs.");
            ViewModel.Users = [];
        }
        else
        {
            ViewModel.Users = result.Value ?? [];
        }

        ViewModel.IsLoading = false;
        await RefreshUI();
    }

    /// <inheritdoc />
    public void OpenResetPasswordModal(string userId, string userName)
    {
        SelectedUserId = userId;
        SelectedUserName = userName;
        NewPassword = string.Empty;
        ShowResetPasswordModal = true;
    }

    /// <inheritdoc />
    public void CloseResetPasswordModal()
    {
        ShowResetPasswordModal = false;
        SelectedUserId = null;
        SelectedUserName = null;
        NewPassword = string.Empty;
    }

    /// <inheritdoc />
    public async Task HandleResetPasswordAsync()
    {
        if (string.IsNullOrEmpty(SelectedUserId))
        {
            _snackbarService.ShowError("Aucun utilisateur sélectionné.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            _snackbarService.ShowWarning("Le mot de passe ne peut pas être vide.");
            return;
        }

        if (!IsNewPasswordValid)
        {
            _snackbarService.ShowWarning("Le mot de passe doit contenir au moins 6 caractères, un chiffre et un caractère spécial.");
            return;
        }

        ResultOf result = await _manageAccountRepository.ResetPasswordAsync(SelectedUserId, NewPassword);

        if (result.HasError)
        {
            _snackbarService.ShowError(result.Error?.Message ?? "Erreur lors de la réinitialisation du mot de passe.");
            return;
        }

        _snackbarService.ShowSuccess($"Mot de passe réinitialisé avec succès pour {SelectedUserName}.");
        CloseResetPasswordModal();
        await RefreshUI();
    }
}