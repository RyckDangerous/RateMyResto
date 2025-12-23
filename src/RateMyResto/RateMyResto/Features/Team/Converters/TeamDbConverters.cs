using RateMyResto.Features.Shared.Models;
using RateMyResto.Features.Team.Models;

namespace RateMyResto.Features.Team.Converters;

public static class TeamDbConverters
{
    /// <summary>
    /// Convertit un TeamDb en Equipe.
    /// </summary>
    /// <param name="teamDb"></param>
    /// <returns></returns>
    public static Equipe ToEquipe(TeamDb teamDb)
    {
        return new Equipe
        {
            Id = teamDb.Id,
            Nom = teamDb.Nom,
            Description = teamDb.Description,
            IdOwner = teamDb.OwnerId
        };
    }
}
