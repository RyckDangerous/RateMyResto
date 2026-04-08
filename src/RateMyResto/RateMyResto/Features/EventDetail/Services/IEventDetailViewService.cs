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
    /// Indique si l'upload de photos est possible (dans les 3 semaines suivant l'événement).
    /// </summary>
    bool CanUploadPhotos { get; }

    /// <summary>
    /// Nombre de jours restants pour uploader des photos (0 si dépassé).
    /// </summary>
    int DaysRemainingForUpload { get; }

    /// <summary>
    /// Indique si la notation est possible (dans les 3 semaines suivant l'événement).
    /// </summary>
    bool CanRate { get; }

    /// <summary>
    /// Nombre de jours restants pour noter l'événement (0 si dépassé).
    /// </summary>
    int DaysRemainingForRating { get; }

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

    /// <summary>
    /// Modifie le statut d'un participant par l'initiateur ou le responsable d'équipe.
    /// Autorisé uniquement si aucun vote n'a eu lieu, ou si l'événement est terminé et que le participant n'a pas voté.
    /// </summary>
    /// <param name="participantUserId">Identifiant ASP.NET Identity du participant</param>
    /// <param name="newStatus">Nouveau statut (valeur de <see cref="RateMyResto.Features.Shared.Models.ParticipationStatus"/>)</param>
    Task ChangeParticipantStatusAsync(string participantUserId, short newStatus);
}