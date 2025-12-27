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

    /// <inheritdoc />
    public EventDetailViewModel? ViewModel { get; private set; }


    /// <inheritdoc />
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
        await RefreshUI();
    }

    /// <inheritdoc />
    public async Task SubmitRatingAsync(Guid eventId, decimal rating, string? comment)
    {
        if (string.IsNullOrEmpty(_currentUserId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        if (rating < 0 || rating > 5)
        {
            _snackbarService.ShowError("La note doit être comprise entre 0 et 5.");
            return;
        }

        // TODO: Implémenter la méthode dans le repository pour sauvegarder la notation
        // ResultOf result = await _eventDetailRepository.SaveRatingAsync(eventId, _currentUserId, rating, comment);
        // 
        // if (result.HasError)
        // {
        //     _snackbarService.ShowError("Erreur lors de l'enregistrement de votre notation.");
        //     return;
        // }

        _snackbarService.ShowSuccess("Votre notation a été enregistrée avec succès !");
        
        // Recharger les données pour afficher la nouvelle notation
        await LoadEvent(eventId);
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
