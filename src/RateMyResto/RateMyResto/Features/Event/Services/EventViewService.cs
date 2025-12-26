using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.Event.Models;
using RateMyResto.Features.Event.Models.Commands;
using RateMyResto.Features.Event.Models.Dbs;
using RateMyResto.Features.Event.Models.Queries;
using RateMyResto.Features.Event.Models.ViewModels;
using RateMyResto.Features.Event.Repositories;
using RateMyResto.Features.Shared.Components.DrawerComponent;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.Event.Services;

public sealed class EventViewService : ViewServiceBase
{
    private readonly ISnackbarService _snackbarService;
    private readonly IDrawerService _drawerService;

    private readonly IEventRepository _eventRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ITeamRepository _teamRepository;

    /// <summary>
    /// ViewModel pour les événements
    /// </summary>
    public EventsViewModel ViewModel { get; private set; }

    /// <summary>
    /// Entrée pour la création d'un nouvel événement
    /// </summary>
    public NewEventInput? EventInput { get; private set; }

    /// <summary>
    /// Identifiant de l'équipe actuellement sélectionnée
    /// dans le cas ou l'utilisateur appartient à plusieurs équipes
    /// </summary>
    private Guid? _currentTeamIdSelected;


    public EventViewService(AuthenticationStateProvider authStateProvider,
                            IEventRepository eventRepository,
                            IRestaurantRepository restaurantRepository,
                            ITeamRepository teamRepository,
                            ISnackbarService snackbarService,
                            IDrawerService drawerService)
        : base(authStateProvider)
    {
        _eventRepository = eventRepository;
        _restaurantRepository = restaurantRepository;
        _teamRepository = teamRepository;
        _snackbarService = snackbarService;
        _drawerService = drawerService;

        ViewModel = new()
        {
            EventsByTeam = Enumerable.Empty<EventByTeamViewModel>()
        };
    }

    /// <inheritdoc />
    public async Task LoadEventsAsync()
    {
        string? userId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        ResultOf<List<EventByUserDb>> eventsResult = await _eventRepository.GetEventsAsync(userId);

        if (eventsResult.HasError)
        {
            if(eventsResult.Error is not NotFoundError)
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

        // récupérer l'Id de l'utilisateur courant
        string? currentUserId = await GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(currentUserId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        UserQuery query = new()
        {
            UserId = currentUserId,
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
            NomRestaurant = EventInput.NomDuRestaurant,
            Adresse = EventInput.Adresse,
            LienGoogleMaps = EventInput.UrlGoogleMaps,
            DateEvent = EventInput.DateEvenement
        };

        var createResult = await _eventRepository.CreateEventAsync(command);
        //if (createResult.HasError)
        //{
        //    // Gérer l'erreur de création d'événement
        //    throw new Exception("Erreur lors de la création de l'événement.", createResult.Error);
        //}
    }


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
            EventCardViewModel cardViewModel = new()
            {
                IdEvent = item.IdEvent,
                DateEvent = item.DateEvent,
                NomDuRestaurant = item.RestaurantName
            };

            EventByTeamViewModel? currentEvent = eventByTeam.FirstOrDefault(x => x.IdEquipe == item.IdEquipe);

            // S'il existe déjà l'équipe dans le retour du viewModel
            if(currentEvent is not null)
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
}
