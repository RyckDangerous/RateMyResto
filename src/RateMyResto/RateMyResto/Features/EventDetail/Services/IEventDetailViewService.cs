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
    /// Charge les détails de l'événement.
    /// </summary>
    /// <param name="idEvent"></param>
    /// <returns></returns>
    Task LoadEvent(Guid idEvent);

    /// <summary>
    /// Soumet la notation d'un utilisateur pour l'événement.
    /// </summary>
    Task SubmitRatingAsync();
}