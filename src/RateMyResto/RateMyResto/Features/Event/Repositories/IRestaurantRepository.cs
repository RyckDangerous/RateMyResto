
using RateMyResto.Features.Event.Models.Commands;

namespace RateMyResto.Features.Event.Repositories;

public interface IRestaurantRepository
{
    /// <summary>
    /// Crée un restaurant
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<ResultOf<int>> CreateRestaurantAsync(CreateRestaurantCommand command);

    /// <summary>
    /// Vérifie si un restaurant existe déjà
    /// </summary>
    /// <param name="nom"></param>
    /// <returns></returns>
    Task<ResultOf<bool>> IsRestaurantExistAsync(string nom);
}