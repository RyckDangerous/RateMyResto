using RateMyResto.Features.EventDetail.Models;

namespace RateMyResto.Features.EventDetail.Services;

public interface IEventDetailViewService
{
    EventDetailViewModel ViewModel { get; }

    Task LoadEvent(Guid idEvent);
}