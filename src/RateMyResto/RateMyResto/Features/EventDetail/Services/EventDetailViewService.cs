using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.EventDetail.Models;
using RateMyResto.Features.EventDetail.Repositories;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Converters;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.EventDetail.Services;

public sealed class EventDetailViewService : ViewServiceBase, IEventDetailViewService
{
    /// <summary>
    /// L'Id de l'utilisateur courant.
    /// </summary>
    private string? _currentUserId = string.Empty;

    private readonly ISnackbarService _snackbarService;
    private readonly IEventDetailRepository _eventDetailRepository;


    public EventDetailViewService(AuthenticationStateProvider authStateProvider,
                                  ISnackbarService snackbarService,
                                  IEventDetailRepository eventDetailRepository)
        : base(authStateProvider)
    {
        _snackbarService = snackbarService;
        _eventDetailRepository = eventDetailRepository;
    }


    public EventDetailViewModel ViewModel { get; private set; }





    public async Task LoadEvent(Guid idEvent)
    {
        _currentUserId = await GetCurrentUserIdAsync();

        if (string.IsNullOrEmpty(_currentUserId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        ResultOf<EventDetailDb> resultDb = await _eventDetailRepository.GetDetailEventAsync(idEvent);

        if (resultDb.HasError)
        {
            _snackbarService.ShowError("Erreur lors du chargement de l'événement");
            return;
        }

        ViewModel = FillViewModel(resultDb.Value);
    }


    /// <summary>
    /// Remplit le ViewModel à partir du modèle de données.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private EventDetailViewModel FillViewModel(EventDetailDb value)
    {
        // Infos équipe
        EquipeViewModel equipeViewModel = new()
        {
            TeamName = value.NomEquipe
        };

        // Infos restaurant
        RestaurantViewModel restaurantViewModel = new()
        {
            Name = value.NomRestaurant,
            Adresse = value.Adresse,
            LienGoogleMaps = value.LienGoogleMaps
        };

        // Infos participants
        List<ParticipantViewModel> participantViewModels = value.InfoParticipants
            .Select(participantDb => new ParticipantViewModel
            {
                ParticipantName = participantDb.UserName,
                Status = StatusConverters.ToStatus(participantDb.StatusId),
                Note = participantDb.Note,
                Commentaire = participantDb.Commentaire,
                DateReview = participantDb.DateReview
            })
            .ToList();

        // Création du ViewModel final
        EventDetailViewModel eventDetailViewModel = new()
        {
            EquipeInfo = equipeViewModel,
            RestaurantInfo = restaurantViewModel,
            ParticipantsInfo = participantViewModels,
            DateEvenement = value.DateEvenement,
            NomIniateur = value.Initiateur
        };

        return eventDetailViewModel;
    }
}
