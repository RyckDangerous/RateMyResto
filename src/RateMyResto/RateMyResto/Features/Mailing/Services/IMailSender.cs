namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Abstraction bas niveau pour l'envoi d'un email HTML.
/// </summary>
public interface IMailSender
{
    /// <summary>
    /// Envoie un email au format HTML à un destinataire unique.
    /// </summary>
    /// <param name="to">Adresse email du destinataire.</param>
    /// <param name="subject">Sujet du message.</param>
    /// <param name="htmlBody">Corps HTML du message.</param>
    Task SendHtmlAsync(string to, string subject, string htmlBody);
}
