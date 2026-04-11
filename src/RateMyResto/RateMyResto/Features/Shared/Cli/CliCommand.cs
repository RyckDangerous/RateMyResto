namespace RateMyResto.Features.Shared.Cli;

/// <summary>
/// Sous-commandes disponibles en mode CLI.
/// </summary>
public enum CliCommand
{
    /// <summary>
    /// Aucune sous-commande fournie.
    /// </summary>
    None,

    /// <summary>
    /// Envoi des rappels de vote aux participants confirmés
    /// d'événements terminés qui n'ont pas encore voté.
    /// </summary>
    Reminder
}
