using RateMyResto.Features.Shared.Models;

namespace RateMyResto.Features.Shared.Converters;

public static class StatusConverters
{
    /// <summary>
    /// Convertit un short en ParticipationStatus
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static ParticipationStatus ToStatus(short status)
    {
        return status switch
        {
            1 => ParticipationStatus.Invite,
            2 => ParticipationStatus.Confirme,
            3 => ParticipationStatus.Decline,
            4 => ParticipationStatus.Absent
        };
    }
}
