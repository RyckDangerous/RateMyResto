namespace RateMyResto.Features.Mailing.Models;

/// <summary>
/// Paramètres applicatifs utilisés par la feature Mailing.
/// Lié à la section "Mailing" de la configuration (variable ENVRATE_Mailing__AppBaseUrl).
/// </summary>
public sealed class MailingAppSettings
{
    /// <summary>
    /// URL de base de l'application, utilisée pour construire les liens dans les emails.
    /// Exemple : https://rate-my-resto.ctrl-alt-suppr.net
    /// </summary>
    public string AppBaseUrl { get; set; } = string.Empty;
}
