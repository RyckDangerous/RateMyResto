using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using RateMyResto.Features.EventDetail.Converters;
using RateMyResto.Features.EventDetail.Models.Commands;
using RateMyResto.Features.EventDetail.Models.DbModels;
using RateMyResto.Features.EventDetail.Models.Errors;
using RateMyResto.Features.EventDetail.Models.InputModels;
using RateMyResto.Features.EventDetail.Models.ViewModels;
using RateMyResto.Features.EventDetail.Repositories;
using RateMyResto.Features.Shared.Components.SnackbarComponent;
using RateMyResto.Features.Shared.Converters;
using RateMyResto.Features.Shared.Services;

namespace RateMyResto.Features.EventDetail.Services;

public sealed class EventDetailViewService : ViewServiceBase, IEventDetailViewService
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 Mo

    /// <summary>
    /// Le chemin physique des images.
    /// </summary>
    private readonly string _pathImages;

    /// <summary>
    /// Le chemin de la requête pour accéder aux images.
    /// </summary>
    private string _pathRequest = "/img";

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
        _pathImages = Path.Combine(Directory.GetCurrentDirectory(), "img");

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

        ViewModel = await FillViewModel(resultDb.Value);
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

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task UploadPhotoAsync(IBrowserFile photo)
    {
        if (photo is null)
        {
            _snackbarService.ShowWarning("Aucune photo sélectionnée.");
            return;
        }

        if (string.IsNullOrEmpty(_currentUserId))
        {
            _snackbarService.ShowError("Utilisateur non authentifié.");
            return;
        }

        try
        {
            // Générer un nom de fichier sécurisé (bonne pratique Microsoft)
            string trustedFileName = Path.GetRandomFileName();
            string fileExtension = Path.GetExtension(photo.Name);
            trustedFileName = Path.ChangeExtension(trustedFileName, fileExtension);

            // Créer la commande avec le stream direct (pas de copie en mémoire)
            UploadPhotoCommand photoCommand = new()
            {
                EventId = _idEvent,
                FileName = trustedFileName,
                ImageStream = photo.OpenReadStream(MaxFileSize)
            };

            // Sauvegarder la photo
            ResultOf result = await SaveImageAsync(photoCommand);
            if (result.HasError)
            {
                _snackbarService.ShowError($"Erreur lors de l'upload de la photo.");
                return;
            }

            _snackbarService.ShowSuccess("Photo uploadée avec succès !");
            
            // Recharger l'événement pour afficher la nouvelle photo
            await LoadEvent(_idEvent);
        }
        catch (Exception ex)
        {
            _snackbarService.ShowError($"Erreur lors de l'upload de la photo : {ex.Message}");
        }
    }

    /// <summary>
    /// Remplit le ViewModel à partir du modèle de données.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private async Task<EventDetailViewModel> FillViewModel(EventDetailDb value)
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

        // Si une note globale n'est pas présente
        // tout le monde n'a pas encore voté
        if (!value.NoteGlobale.HasValue)
        {
            // Masquer ou non les notes.
            // Masquer les notes tant que tout le monde n'a pas voté.
            bool haveOneNote = participantViewModels.Any(p => p.Note.HasValue);

            if (haveOneNote
                && participantViewModels.Any(p => !p.Note.HasValue))
            {
                foreach (ParticipantViewModel participant in participantViewModels)
                {
                    if (participant.Note.HasValue)
                    {
                        participant.HideNote = true;
                    }
                }
            }
            else
            {
                // Tous les participants ont noté
                bool allHaveNote = participantViewModels.All(p => p.Note.HasValue);
                if (allHaveNote)
                {
                    // Calcul de la note globale
                    decimal totalNotes = participantViewModels.Sum(p => p.Note ?? 0);
                    value.NoteGlobale = Math.Round(totalNotes / participantViewModels.Count, 2);

                    // sauvegarde de la note globale
                    GlobalRatingCommand globalRatingCommand = new()
                    {
                        EventId = _idEvent,
                        Rating = value.NoteGlobale.Value
                    };

                    ResultOf globalRatingResult = await _eventDetailRepository.UpdateGlobalRatingAsync(globalRatingCommand);

                    if (globalRatingResult.HasError)
                    {
                        _snackbarService.ShowError("Erreur lors de la mise à jour de la note globale.");
                    }
                }
            }
        }

        // Récupération des photos de l'événement
        IEnumerable<string> photos = GetImagesByEvent(value.Id);

        // Création du ViewModel final
        EventDetailViewModel eventDetailViewModel = new()
        {
            EquipeInfo = equipeViewModel,
            RestaurantInfo = restaurantViewModel,
            ParticipantsInfo = participantViewModels,
            DateEvenement = value.DateEvenement,
            NomIniateur = value.Initiateur,
            Photos = photos,
            NoteGlobale = value.NoteGlobale
        };

        return eventDetailViewModel;
    }



    private IEnumerable<string> GetImagesByEvent(Guid idEvent)
    {
        string eventImagePath = Path.Combine(_pathImages, idEvent.ToString());

        List<string> imageUrls = new();

        if (Directory.Exists(eventImagePath))
        {
            string[] imageFiles = Directory.GetFiles(eventImagePath);

            foreach (string imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                string imageUrl = $"{_pathRequest}/{idEvent}/{fileName}";

                imageUrls.Add(imageUrl);
            }
        }

        return imageUrls;
    }

    private async Task<ResultOf> SaveImageAsync(UploadPhotoCommand command)
    {
        string eventImagePath = Path.Combine(_pathImages, command.EventId.ToString());

        if (!Directory.Exists(eventImagePath))
        {
            Directory.CreateDirectory(eventImagePath);
        }

        string filePath = Path.Combine(eventImagePath, command.FileName);

        try
        {
            // Copier le stream DIRECTEMENT vers le fichier sans passer par la mémoire
            // (Bonne pratique Microsoft pour éviter les problèmes de performance)
            await using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
            await command.ImageStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();

            return ResultOf.Success();
        }
        catch (Exception ex)
        {
            FileError error = new("Erreur de sauvegarde d'une photo.", ex);

            // Si erreur il faut supprimer le fichier "fantome".
            if (File.Exists(filePath))
                File.Delete(filePath);

            return ResultOf.Failure(error);
        }

    }

}
