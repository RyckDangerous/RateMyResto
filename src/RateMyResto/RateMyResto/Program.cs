using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RateMyResto.Features;
using RateMyResto.Features.Account;
using RateMyResto.Features.Data;
using RateMyResto.Features.DbMigration.Configurations;
using RateMyResto.Features.DbMigration.Services;
using RateMyResto.Features.Event.Configurations;
using RateMyResto.Features.EventDetail.Configurations;
using RateMyResto.Features.Shared.Configurations;
using RateMyResto.Features.Team.Configurations;


ILogger? logger = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
}).CreateLogger<Program>();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

    builder.Configuration.Sources.Clear();
    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables(prefix: "ENVRATE_")
                        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

    builder.Services.AddLogging();

    string connectionString = builder.Services.ConfigureSettings(builder.Configuration);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false; // La confirmation de l'email n'est pas requise
            options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            options.User.RequireUniqueEmail = false; // L'email n'est pas requis
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

    builder.Services.AddSharedServices();
    builder.Services.AddDbMigrationServices();
    builder.Services.AddTeamFeatures();
    builder.Services.AddEventFeatures();
    builder.Services.AddEventDetailFeatures();

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Migration de la base de données
    using (IServiceScope scope = app.Services.CreateScope())
    {
        // Appliquer les migrations EF Core en attente
        ApplicationDbContext? db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        // Appliquer les migrations DbUp
        IDbMigrationService dbMigrationService = scope.ServiceProvider.GetRequiredService<IDbMigrationService>();

        if (dbMigrationService.UpgradeDatabase())
        {
            logger?.LogInformation("Migration de la base de données réussie");
        }
        else
        {
            throw new Exception("Erreur lors de la migration de la base de données");
        }
    }

    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    // Répertoire personnalisé pour les fichiers statiques (images)
    string pathImages = Path.Combine(Directory.GetCurrentDirectory(), "img");
    if (!Directory.Exists(pathImages))
    {
        Directory.CreateDirectory(pathImages);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(pathImages),
        RequestPath = "/img"
    });

    // Add additional endpoints required by the Identity /Account Razor components.
    app.MapAdditionalIdentityEndpoints();

    await app.RunAsync();
}
catch (Exception ex)
{
    logger?.LogCritical(ex, "MAIN Exception - Stopped program because of exception");
}