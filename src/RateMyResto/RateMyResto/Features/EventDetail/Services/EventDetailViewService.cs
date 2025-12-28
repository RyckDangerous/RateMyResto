using Microsoft.AspNetCore.Components.Authorization;
using RateMyResto.Features.EventDetail.Converters;
using RateMyResto.Features.EventDetail.Models.Commands;
using RateMyResto.Features.EventDetail.Models.DbModels;
using RateMyResto.Features.EventDetail.Models.InputModels;
using RateMyResto.Features.EventDetail.Models.ViewModels;
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
    private string _currentUserId = string.Empty;

    /// <summary>
    /// L'Id de l'événement courant.
    /// </summary>
    private Guid _idEvent;

    private readonly ISnackbarService _snackbarService;
    private readonly IEventDetailRepository _eventDetailRepository;


    public EventDetailViewService(AuthenticationStateProvider authStateProvider,
                                  ISnackbarService snackbarService,
                                  IEventDetailRepository eventDetailRepository)
        : base(authStateProvider)
    {
        _snackbarService = snackbarService;
        _eventDetailRepository = eventDetailRepository;

        RatingInput = new EventRatingInput();
    }

    /// <inheritdoc />
    public EventDetailViewModel? ViewModel { get; private set; }

    /// <inheritdoc />
    public EventRatingInput RatingInput { get; private set; }


    /// <inheritdoc />
    public async Task LoadEvent(Guid idEvent)
    {
        _idEvent = idEvent;
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
        await RefreshUI();
    }

    /// <inheritdoc />
    public async Task SubmitRatingAsync()
    {
        if (RatingInput.Rating < 0 || RatingInput.Rating > 5)
        {
            _snackbarService.ShowError("La note doit être comprise entre 0 et 5.");
            return;
        }

        RatingCommand ratingCommand = RatingInputConverters.ToCommand(RatingInput, _idEvent, _currentUserId);

        ResultOf result = await _eventDetailRepository.SaveRatingAsync(ratingCommand);
        if (result.HasError)
        {
            _snackbarService.ShowError("Erreur lors de l'enregistrement de votre notation.");
            return;
        }

        _snackbarService.ShowSuccess("Votre notation a été enregistrée avec succès !");
        
        // Recharger les données pour afficher la nouvelle notation
        await LoadEvent(_idEvent);
    }

    /// <summary>
    /// Remplit le ViewModel à partir du modèle de données.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
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

        // Masquer ou non les notes.
        // Masquer les notes tant que tout le monde n'a pas voté.
        bool haveOneNote = participantViewModels.Any(p => p.Note.HasValue);

        if (haveOneNote 
            && participantViewModels.Any(p => !p.Note.HasValue))
        {
            foreach (ParticipantViewModel participant in participantViewModels)
            {
                if(participant.Note.HasValue)
                {
                    participant.HideNote = true;
                }
            }
        }

        // TODO : Remplacer par la récupération réelle des photos
        // Récupération des photos de l'événement
        List<string> photos = new()
        {
            "https://woody.cloudly.space/app/uploads/bretagne-35/2020/06/thumbs/restaurant-jason-leung-unsplash-1920x960.jpg",
            "https://woody.cloudly.space/app/uploads/bretagne-35/2020/06/thumbs/restaurant-jason-leung-unsplash-1920x960.jpg",
            "https://woody.cloudly.space/app/uploads/bretagne-35/2020/06/thumbs/restaurant-jason-leung-unsplash-1920x960.jpg",
            "https://woody.cloudly.space/app/uploads/bretagne-35/2020/06/thumbs/restaurant-jason-leung-unsplash-1920x960.jpg",
            "https://woody.cloudly.space/app/uploads/bretagne-35/2020/06/thumbs/restaurant-jason-leung-unsplash-1920x960.jpg"
        };

        // Création du ViewModel final
        EventDetailViewModel eventDetailViewModel = new()
        {
            EquipeInfo = equipeViewModel,
            RestaurantInfo = restaurantViewModel,
            ParticipantsInfo = participantViewModels,
            DateEvenement = value.DateEvenement,
            NomIniateur = value.Initiateur,
            Photos = photos
        };

        return eventDetailViewModel;
    }
}
