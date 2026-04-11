namespace RateMyResto.Features.Mailing.Models;

/// <summary>
/// Paramètres de connexion au serveur SMTP.
/// Lié à la section "Smtp" de la configuration (variables ENVRATE_Smtp__*).
/// </summary>
public sealed class SmtpSettings
{
    /// <summary>
    /// Nom d'hôte du serveur SMTP (ex: mail_postfix).
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Port du serveur SMTP (par défaut 587).
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Adresse d'expéditeur utilisée dans le champ From des messages.
    /// </summary>
    public string From { get; set; } = string.Empty;
}
