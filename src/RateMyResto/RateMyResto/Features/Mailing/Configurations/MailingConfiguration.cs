using BlazorMail.Extensions;
using RateMyResto.Features.Mailing.Models;
using RateMyResto.Features.Mailing.Repositories;
using RateMyResto.Features.Mailing.Services;

namespace RateMyResto.Features.Mailing.Configurations;

/// <summary>
/// Enregistrement des dépendances de la feature Mailing.
/// </summary>
public static class MailingConfiguration
{
    /// <summary>
    /// Ajoute les services de la feature Mailing au conteneur DI :
    /// binding SmtpSettings et MailingAppSettings, rendu BlazorMail,
    /// envoi SMTP MailKit et service de rappel.
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration de l'application.</param>
    public static IServiceCollection AddMailingFeatures(this IServiceCollection services,
                                                         IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<MailingAppSettings>(configuration.GetSection("Mailing"));

        services.AddBlazorMail();

        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddTransient<IMailSender, MailSender>();
        services.AddScoped<IReminderService, ReminderService>();

        return services;
    }
}
