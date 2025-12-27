using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.Event.Converters;
using RateMyResto.Features.Event.Models;
using RateMyResto.Features.Event.Models.Commands;
using RateMyResto.Features.Event.Models.Dbs;
using RateMyResto.Features.Event.Models.Queries;
using RateMyResto.Features.Event.Models.ViewModels;
using RateMyResto.Features.Event.Repositories;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.Event.Services;

public sealed class EventViewService : ViewServiceBase, IEventViewService
{
    private readonly ISnackbarService _snackbarService;

    private readonly IEventRepository _eventRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// L'Id de l'équipe sélectionnée pour la création d'un événement.
    /// </summary>
    private Guid? _currentTeamIdSelected;

    /// <inheritdoc />
    public EventsViewModel ViewModel { get; private set; }

    /// <inheritdoc />
    public NewEventInput? EventInput { get; private set; }

    // === Gestion de la modale de création d'événement ===

    /// <inheritdoc />
    public bool ShowCreateEventModal { get; private set; } = false;

    /// <inheritdoc />
    public Guid? SelectedTeamId { get; set; }

    /// <inheritdoc />
    private string _currentUserId = string.Empty;

    /// <inheritdoc />
    public List<EquipeViewModel> AvailableTeams { get; private set; } = new();


    public EventViewService(AuthenticationStateProvider authStateProvider,
                            IEventRepository eventRepository,
                            IRestaurantRepository restaurantRepository,
                            ITeamRepository teamRepository,
                            ISnackbarService snackbarService,
                            NavigationManager navigationManager)
        : base(authStateProvider)
    {
        _eventRepository = eventRepository;
        _restaurantRepository = restaurantRepository;
        _teamRepository = teamRepository;
        _snackbarService = snackbarService;
        _navigationManager = navigationManager;

        ViewModel = new()
        {
            EventsByTeam = Enumerable.Empty<EventByTeamViewModel>()
        };
    }

    /// <inheritdoc />
    public async Task LoadEventsAsync()
    {
        _currentUserId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(_currentUserId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        ResultOf<List<EventByUserDb>> eventsResult = await _eventRepository.GetEventsAsync(_currentUserId);

        if (eventsResult.HasError)
        {
            if (eventsResult.Error is not NotFoundError)
            {
                _snackbarService.ShowError("Erreur sur la récupération des évènements.");
            }

            ViewModel = new()
            {
                EventsByTeam = Enumerable.Empty<EventByTeamViewModel>()
            };

            return;
        }

        ViewModel = FillViewModel(eventsResult.Value);
        await RefreshUI();
    }

    /// <inheritdoc />
    public async Task OpenCreateEventModalAsync()
    {
        ResultOf<List<EquipeDb>> availableTeamsResult = await _teamRepository.GetTeamsByUserIdAsync(_currentUserId);

        if (availableTeamsResult.HasError)
        {
            _snackbarService.ShowError("Erreur lors de la récupération des équipes.");
            return;
        }

        AvailableTeams = availableTeamsResult.Value
            .Select(equipe => new EquipeViewModel
            {
                Id = equipe.IdEquipe,
                Nom = equipe.NomEquipe
            })
            .ToList();

        if (AvailableTeams.Count == 1)
        {
            // Sélectionner la première équipe par défaut
            SelectedTeamId = AvailableTeams.FirstOrDefault()?.Id;
        }
        else
        {
            // TODO : gérer le cas où l'utilisateur est dans plusieurs équipes
        }

        // Initialiser EventInput avec des valeurs par défaut
        EventInput = new NewEventInput
        {
            IdRestaurant = null,
            NomDuRestaurant = string.Empty,
            Adresse = string.Empty,
            UrlGoogleMaps = null,
            DateEvenement = DateOnly.FromDateTime(DateTime.Today)
        };

        ShowCreateEventModal = true;
        await RefreshUI();
    }

    /// <inheritdoc />
    public async Task CloseCreateEventModalAsync()
    {
        ShowCreateEventModal = false;
        await RefreshUI();
    }

    /// <inheritdoc />
    public async Task HandleCreateEventAsync()
    {
        if (EventInput is null)
        {
            _snackbarService.ShowError("Erreur : les données de l'événement sont introuvables.");
            return;
        }

        // Validation des champs
        if (string.IsNullOrWhiteSpace(EventInput.NomDuRestaurant))
        {
            _snackbarService.ShowWarning("Le nom du restaurant est obligatoire.");
            return;
        }

        if (string.IsNullOrWhiteSpace(EventInput.Adresse))
        {
            _snackbarService.ShowWarning("L'adresse du restaurant est obligatoire.");
            return;
        }

        if (!SelectedTeamId.HasValue)
        {
            _snackbarService.ShowWarning("Vous devez sélectionner une équipe.");
            return;
        }

        if (EventInput.DateEvenement < DateOnly.FromDateTime(DateTime.Today))
        {
            _snackbarService.ShowWarning("La date de l'événement ne peut pas être dans le passé.");
            return;
        }

        // Définir l'équipe sélectionnée pour la création
        _currentTeamIdSelected = SelectedTeamId;

        // Appeler la méthode de création
        await CreateEventAsync();

        // Fermer la modale
        await CloseCreateEventModalAsync();

        // Recharger les événements
        await LoadEventsAsync();
    }

    /// <inheritdoc />
    public async Task ConfirmParticipationAsync(Guid eventId)
    {
        UpdateStatusCommand command = new()
        {
            UserId = _currentUserId,
            EventId = eventId,
            Status = (byte)ParticipationStatus.Confirme
        };

        ResultOf updateResult = await _eventRepository.UpdateParticipationStatusAsync(command);

        if (updateResult.HasError)
        {
            _snackbarService.ShowError("Erreur lors de la confirmation de la participation.");
            return;
        }

        _snackbarService.ShowSuccess("Participation confirmée !");
        await LoadEventsAsync();
    }

    /// <inheritdoc />
    public async Task DeclineParticipationAsync(Guid eventId)
    {
        UpdateStatusCommand command = new()
        {
            UserId = _currentUserId,
            EventId = eventId,
            Status = (byte)ParticipationStatus.Decline
        };

        ResultOf updateResult = await _eventRepository.UpdateParticipationStatusAsync(command);

        if (updateResult.HasError)
        {
            _snackbarService.ShowError("Erreur lors de la confirmation de la participation.");
            return;
        }

        _snackbarService.ShowInfo("Participation déclinée.");

        // Recharger les événements pour mettre à jour l'affichage
        await LoadEventsAsync();
    }

    /// <inheritdoc />
    public async Task CreateEventAsync()
    {
        if (!_currentTeamIdSelected.HasValue)
        {
            _snackbarService.ShowError("Vous devez sélectionner une équipe pour créer un événement.");
            return;
        }

        int? idRestaurant = await RestoManageAsync();

        if (idRestaurant is null)
        {
            // Il y a eu une erreur dans la gestion du restaurant
            _snackbarService.ShowError("Il y a eu une erreur lors de la gestion du restaurant.");
            return;
        }

        UserQuery query = new()
        {
            UserId = _currentUserId,
            TeamId = _currentTeamIdSelected.Value
        };
        ResultOf<int> idUserResult = await _teamRepository.GetUserTeamsIdAsync(query);

        if (idUserResult.HasError)
        {
            _snackbarService.ShowError("Erreur lors de la récupération de l'utilisateur.");
            return;
        }

        if (EventInput is null)
        {
            _snackbarService.ShowError("Les données de l'événement sont manquantes.");
            return;
        }

        NewEventCommand command = new()
        {
            IdTeam = _currentTeamIdSelected.Value,
            IdInitiateur = idUserResult.Value,
            IdRestaurant = idRestaurant.Value,
            DateEvent = EventInput.DateEvenement
        };

        ResultOf createResult = await _eventRepository.CreateEventAsync(command);
        if (createResult.HasError)
        {
            _snackbarService.ShowError("Erreur lors de la création de l'événement.");
        }
    }

    /// <inheritdoc />
    public async Task OpenDetailPageAsync(Guid eventId)
    {
        _navigationManager.NavigateTo($"/event/detail/{eventId}");
    }

    #region Private methods

    /// <summary>
    /// Gère la création ou la mise à jour d'un restaurant
    /// </summary>
    /// <returns>true : on peut continuer / false : il y a eu une erreur</returns>
    private async Task<int?> RestoManageAsync()
    {
        if (EventInput is null)
        {
            _snackbarService.ShowError("Les données de l'événement sont manquantes.");
            return null;
        }

        // Si l'Id est à null, c'est un nouveau restaurant
        if (EventInput.IdRestaurant is null)
        {
            // Vérifier si le restaurant existe déjà
            ResultOf<bool> isRestoExistResult = await _restaurantRepository.IsRestaurantExistAsync(EventInput.NomDuRestaurant);

            if (isRestoExistResult.HasError)
            {
                // Gérer l'erreur de vérification d'existence du restaurant
                _snackbarService.ShowError("Erreur lors de la vérification de l'existence du restaurant.");
                return null;
            }

            if (isRestoExistResult.Value)
            {
                // Le restaurant existe déjà, afficher un message d'erreur
                _snackbarService.ShowError("Le restaurant existe déjà.");
                return null;
            }

            // Création du nouveau restaurant
            CreateRestaurantCommand commandCreate = new()
            {
                Nom = EventInput.NomDuRestaurant,
                Adresse = EventInput.Adresse,
                UrlGoogleMaps = EventInput.UrlGoogleMaps
            };

            ResultOf<int> createRestoResult = await _restaurantRepository.CreateRestaurantAsync(commandCreate);

            if (createRestoResult.HasError)
            {
                // Gérer l'erreur de création du restaurant
                _snackbarService.ShowError("Erreur lors de la création du restaurant.");
                return null;
            }

            return createRestoResult.Value;
        }
        else
        {
            // sinon l'utilisateur à sélectionné un restaurant existant,
            // on n'a rien à faire
            return EventInput.IdRestaurant.Value;
        }
    }

    /// <summary>
    /// Va créer le ViewModel.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private EventsViewModel FillViewModel(List<EventByUserDb> value)
    {
        List<EventByTeamViewModel> eventByTeam = new();

        foreach (EventByUserDb item in value)
        {
            ParticipationStatus status = StatusConverters.ToStatus(item.ParticipationStatus);

            EventCardViewModel cardViewModel = new()
            {
                IdEvent = item.IdEvent,
                DateEvent = item.DateEvent,
                NomDuRestaurant = item.RestaurantName,
                Status = status
            };

            EventByTeamViewModel? currentEvent = eventByTeam.FirstOrDefault(x => x.IdEquipe == item.IdEquipe);

            // S'il existe déjà l'équipe dans le retour du viewModel
            if (currentEvent is not null)
            {
                currentEvent.Events.Add(cardViewModel);
            }
            else
            {
                List<EventCardViewModel> eventsOnTeams = new();
                eventsOnTeams.Add(cardViewModel);

                // Sinon créer "l'équipe" dans le ViewModel
                EventByTeamViewModel evt = new()
                {
                    IdEquipe = item.IdEquipe,
                    TeamName = item.EquipeName,
                    Events = eventsOnTeams
                };

                eventByTeam.Add(evt);
            }
        }

        return new EventsViewModel()
        {
            EventsByTeam = eventByTeam
        };
    }

    #endregion

}
