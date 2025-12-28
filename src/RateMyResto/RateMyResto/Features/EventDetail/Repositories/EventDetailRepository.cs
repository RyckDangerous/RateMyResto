using Microsoft.Data.SqlClient;
using RateMyResto.Core.Data;
using RateMyResto.Features.EventDetail.Models.Commands;
using RateMyResto.Features.EventDetail.Models.DbModels;
using RateMyResto.Features.Shared.Configurations;

namespace RateMyResto.Features.EventDetail.Repositories;

public sealed class EventDetailRepository : RepositoryBase<EventDetailRepository>, IEventDetailRepository
{

    public EventDetailRepository(IApplicationSettings config, ILogger<EventDetailRepository> logger)
        : base(config, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ResultOf<EventDetailDb>> GetDetailEventAsync(Guid idEvent)
    {
        SqlParameter[] sqlParameters =
        {
            GetSqlParameterUniqueIdentifier("@IdEvent", idEvent)
        };

        return await ExecuteStoredProcedureWithJsonResultAsync<EventDetailDb>(procName: "sp_GetEventById",
                                                                            parameters: sqlParameters);
    }

    /// <inheritdoc />
    public async Task<ResultOf> SaveRatingAsync(RatingCommand ratingCommand)
    {
        SqlParameter[] sqlParameters =
        {
            GetSqlParameterUniqueIdentifier("@EventId", ratingCommand.EventId),
            GetSqlParameterNVarchar("@UserId", ratingCommand.UserId),
            GetSqlParameterDecimal("@Note", ratingCommand.Rating),
            GetSqlParameterDate("@DateReview", DateOnly.FromDateTime(DateTime.Now)),
            GetSqlParameterNVarchar("@Commentaire", ratingCommand.Comment, 1000)
        };

        return await ExecuteStoredProcedureAsync(procName: "sp_SaveParticipantReview",
                                                parameters: sqlParameters);
    }

    /// <inheritdoc />
    public async Task<ResultOf> UpdateGlobalRatingAsync(GlobalRatingCommand globalRatingCommand)
    {
        SqlParameter[] sqlParameters =
        {
            GetSqlParameterUniqueIdentifier("@EventId", globalRatingCommand.EventId),
            GetSqlParameterDecimal("@NoteGlobale", globalRatingCommand.Rating)
        };

        return await ExecuteStoredProcedureAsync(procName: "sp_UpdateEventGlobalRating",
                                                parameters: sqlParameters);
    }
}
