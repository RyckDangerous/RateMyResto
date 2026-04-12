using RateMyResto.Features.HomeFeature.RestoDetail.Models;
using RateMyResto.Features.HomeFeature.RestoDetail.Repositories;

namespace RateMyResto.Features.HomeFeature.RestoDetail.ViewServices;

/// <summary>
/// Service de vue pour la page de détail public d'un événement.
/// Charge le détail depuis la base et enrichit le ViewModel avec les photos du filesystem.
/// </summary>
public sealed class RestoDetailViewService : IRestoDetailViewService
{
    private const string PathRequest = "/img";

    private readonly IRestoDetailRepository _repository;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<RestoDetailViewService> _logger;

    public RestoDetailViewService(IRestoDetailRepository repository,
                                  IWebHostEnvironment env,
                                  ILogger<RestoDetailViewService> logger)
    {
        _repository = repository;
        _env = env;
        _logger = logger;
    }

    /// <inheritdoc />
    public RestoDetailViewModel? ViewModel { get; private set; }

    /// <inheritdoc />
    public async Task LoadAsync(Guid eventId)
    {
        _logger.LogInformation("Chargement du détail public de l'événement {EventId}", eventId);

        ResultOf<PublicEventDetailDb> result = await _repository.GetEventDetailAsync(eventId);

        if (result.HasError)
        {
            _logger.LogError("Erreur lors du chargement de l'événement {EventId} : {Error}",
                eventId, result.Error.Message);
            ViewModel = null;
            return;
        }

        PublicEventDetailDb db = result.Value;

        IReadOnlyList<string> photos = GetImagesByEvent(db.Id);

        ViewModel = new RestoDetailViewModel
        {
            Id = db.Id,
            DateEvenement = db.DateEvenement,
            NoteGlobale = db.NoteGlobale,
            NomRestaurant = db.NomRestaurant,
            Adresse = db.Adresse,
            LienGoogleMaps = db.LienGoogleMaps,
            NomEquipe = db.NomEquipe,
            TeamId = db.TeamId,
            Avis = db.Avis ?? new List<AvisPublicDb>(),
            Photos = photos
        };

        _logger.LogInformation("Détail chargé pour l'événement {EventId} : {PhotoCount} photo(s), {AvisCount} avis",
            eventId, photos.Count, ViewModel.Avis.Count);
    }

    /// <summary>
    /// Retourne les URLs des photos stockées dans le répertoire de l'événement.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Liste des URLs relatives /img/{eventId}/{filename}.</returns>
    private IReadOnlyList<string> GetImagesByEvent(Guid eventId)
    {
        string eventImagePath = Path.Combine(Directory.GetCurrentDirectory(), "img", eventId.ToString());

        List<string> imageUrls = new();

        if (!Directory.Exists(eventImagePath))
        {
            return imageUrls;
        }

        string[] imageFiles = Directory.GetFiles(eventImagePath);

        foreach (string imageFile in imageFiles)
        {
            string fileName = Path.GetFileName(imageFile);
            string imageUrl = $"{PathRequest}/{eventId}/{fileName}";
            imageUrls.Add(imageUrl);
        }

        return imageUrls;
    }
}
