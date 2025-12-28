using RateMyResto.Features.EventDetail.Models.Commands;
using RateMyResto.Features.EventDetail.Models.InputModels;

namespace RateMyResto.Features.EventDetail.Converters;

public static class RatingInputConverters
{
    /// <summary>
    /// Convertit un EventRatingInput en RatingCommand
    /// </summary>
    /// <param name="input"></param>
    /// <param name="idEvent"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static RatingCommand ToCommand(EventRatingInput input, Guid idEvent, string userId)
    {
        return new RatingCommand
        {
            EventId = idEvent,
            UserId = userId,
            Rating = input.Rating ?? 0,
            Comment = input.Comment
        };
    }
}
