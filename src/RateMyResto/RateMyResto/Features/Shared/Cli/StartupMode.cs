namespace RateMyResto.Features.Shared.Cli;

/// <summary>
/// Mode de démarrage de l'application.
/// </summary>
public enum StartupMode
{
    /// <summary>
    /// Mode par défaut : lancement du serveur web Blazor.
    /// </summary>
    Web,

    /// <summary>
    /// Mode ligne de commande : exécution d'une sous-commande puis arrêt.
    /// </summary>
    Cli
}
