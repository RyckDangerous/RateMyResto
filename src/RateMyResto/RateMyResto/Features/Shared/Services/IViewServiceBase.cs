namespace RateMyResto.Features.Shared.Services;

public interface IViewServiceBase
{
    /// <summary>
    /// Enregistre la fonction de rafraîchissement de l'interface utilisateur.
    /// </summary>
    /// <param name="refreshUi"></param>
    void RegisterUiRefresh(Func<Task> refreshUi);
    
}
