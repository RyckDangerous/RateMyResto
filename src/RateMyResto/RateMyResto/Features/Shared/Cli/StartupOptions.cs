namespace RateMyResto.Features.Shared.Cli;

/// <summary>
/// Options de démarrage de l'application déduites de la ligne de commande.
/// Le mode par défaut (aucun argument) est <see cref="StartupMode.Web"/>.
/// </summary>
public sealed record StartupOptions
{
    private const string ModeArgumentPrefix = "--mode=";
    private const string ReminderArgument = "--reminder";
    private const string ModeValueWeb = "web";
    private const string ModeValueCli = "cli";

    /// <summary>
    /// Mode de démarrage de l'application.
    /// </summary>
    public required StartupMode Mode { get; init; }

    /// <summary>
    /// Sous-commande à exécuter quand <see cref="Mode"/> vaut <see cref="StartupMode.Cli"/>.
    /// </summary>
    public required CliCommand Command { get; init; }

    /// <summary>
    /// Message d'erreur si le parsing a échoué. Null en cas de succès.
    /// </summary>
    public string? ParsingError { get; init; }

    /// <summary>
    /// Indique si les options parsées sont valides.
    /// </summary>
    public bool IsValid => ParsingError is null;

    /// <summary>
    /// Parse les arguments de la ligne de commande.
    /// Aucun argument => mode Web (défaut).
    /// Formats reconnus : --mode=web | --mode=cli [--reminder]
    /// </summary>
    /// <param name="args">Arguments fournis par le point d'entrée.</param>
    /// <returns>Options de démarrage. Vérifier <see cref="IsValid"/>.</returns>
    public static StartupOptions Parse(string[] args)
    {
        if (args is null || args.Length is 0)
        {
            return new StartupOptions
            {
                Mode = StartupMode.Web,
                Command = CliCommand.None
            };
        }

        StartupMode? parsedMode = null;
        CliCommand parsedCommand = CliCommand.None;

        foreach (string arg in args)
        {
            if (arg.StartsWith(ModeArgumentPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string value = arg[ModeArgumentPrefix.Length..];

                if (string.Equals(value, ModeValueWeb, StringComparison.OrdinalIgnoreCase))
                {
                    parsedMode = StartupMode.Web;
                }
                else if (string.Equals(value, ModeValueCli, StringComparison.OrdinalIgnoreCase))
                {
                    parsedMode = StartupMode.Cli;
                }
                else
                {
                    return Invalid($"Valeur de --mode inconnue : '{value}'. Valeurs attendues : web | cli.");
                }

                continue;
            }

            if (string.Equals(arg, ReminderArgument, StringComparison.OrdinalIgnoreCase))
            {
                parsedCommand = CliCommand.Reminder;
                continue;
            }

            return Invalid($"Argument inconnu : '{arg}'.");
        }

        StartupMode effectiveMode = parsedMode ?? StartupMode.Web;

        if (effectiveMode is StartupMode.Web)
        {
            return new StartupOptions
            {
                Mode = StartupMode.Web,
                Command = CliCommand.None
            };
        }

        if (parsedCommand is CliCommand.None)
        {
            return Invalid("Le mode CLI nécessite une sous-commande (ex: --reminder).");
        }

        return new StartupOptions
        {
            Mode = StartupMode.Cli,
            Command = parsedCommand
        };
    }

    private static StartupOptions Invalid(string error)
    {
        return new StartupOptions
        {
            Mode = StartupMode.Web,
            Command = CliCommand.None,
            ParsingError = error
        };
    }
}
