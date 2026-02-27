using RateMyResto.Features.EventDetail.Models.InputModels;
using RateMyResto.Features.EventDetail.Models.ViewModels;

namespace RateMyResto.Features.EventDetail.Services;

public interface IEventDetailViewService
{
    /// <summary>
    /// Le ViewModel des détails de l'événement.
    /// </summary>
    EventDetailViewModel? ViewModel { get; }

    /// <summary>
    /// Les entrées de notation de l'événement par l'utilisateur.
    /// </summary>
    EventRatingInput RatingInput { get; }

    /// <summary>
    /// Indique si l'upload de photos est possible (dans les 4 jours suivant l'événement).
    /// </summary>
    bool CanUploadPhotos { get; }

    /// <summary>
    /// Nombre d'heures restantes pour uploader des photos (0 si dépassé).
    /// </summary>
    double HoursRemainingForUpload { get; }

    /// <summary>
    /// Charge les détails de l'événement.
    /// </summary>
    /// <param name="idEvent"></param>
    /// <returns></returns>
    Task LoadEvent(Guid idEvent);

    /// <summary>
    /// Soumet la notation d'un utilisateur pour l'événement.
    /// </summary>
    Task SubmitRatingAsync();

    /// <summary>
    /// Upload d'une photo pour l'événement.
    /// </summary>
    /// <param name="photo">Fichier photo à uploader</param>
    Task UploadPhotoAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile photo);
}