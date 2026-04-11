using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RateMyResto.Features.Mailing.Models;

namespace RateMyResto.Features.Mailing.Services;

/// <summary>
/// Implémentation MailKit de <see cref="IMailSender"/>.
/// Cible le serveur SMTP interne Docker (pas d'auth, pas de TLS).
/// </summary>
public sealed class MailSender : IMailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<MailSender> _logger;

    public MailSender(IOptions<SmtpSettings> settings, ILogger<MailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendHtmlAsync(string to, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            throw new InvalidOperationException(
                "SMTP non configuré : Smtp:Host est vide. Vérifier les variables ENVRATE_Smtp__*.");
        }

        if (string.IsNullOrWhiteSpace(_settings.From))
        {
            throw new InvalidOperationException(
                "SMTP non configuré : Smtp:From est vide. Vérifier ENVRATE_Smtp__From.");
        }

        MimeMessage message = new();
        message.From.Add(MailboxAddress.Parse(_settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        _logger.LogInformation("Envoi SMTP vers {To} via {Host}:{Port}",
            to, _settings.Host, _settings.Port);

        using SmtpClient client = new();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None);
        await client.SendAsync(message);
        await client.DisconnectAsync(quit: true);

        _logger.LogInformation("Email envoyé à {To}", to);
    }
}
