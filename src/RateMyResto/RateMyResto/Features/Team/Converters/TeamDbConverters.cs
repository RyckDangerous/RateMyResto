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
            IdOwner = teamDb.OwnerId,
            OwnerName = teamDb.OwnerName,
            Membres = teamDb.Members.Select(ToMembre).ToList()
        };
    }

    /// <summary>
    /// Convertit un TeamMemberDb en Membre.
    /// </summary>
    /// <param name="memberDb"></param>
    /// <returns></returns>
    public static Membre ToMembre(TeamMemberDb memberDb)
    {
        return new Membre()
        {
            Id = memberDb.IdUser,
            Nom = memberDb.UserName
        };
    }

}
