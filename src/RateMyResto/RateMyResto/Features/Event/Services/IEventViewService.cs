using RateMyResto.Features.Event.Models;
using RateMyResto.Features.Event.Models.ViewModels;

namespace RateMyResto.Features.Event.Services;

/// <summary>
/// Service de présentation pour la gestion des événements (chargement, création, participation).
/// Conçu pour être consommé depuis des composants Blazor.
/// </summary>
public interface IEventViewService
{
    /// <summary>
    /// Obtient la liste des équipes disponibles pour la création d'un événement.
    /// </summary>
    List<EquipeViewModel> AvailableTeams { get; }

    /// <summary>
    /// Obtient le modèle de saisie pour la création d'un nouvel événement.
    /// <br/>Peut être <see langword="null" /> si le modal de création n'est pas ouvert.
    /// </summary>
    NewEventInput? EventInput { get; }

    /// <summary>
    /// Obtient ou définit l'identifiant de l'équipe sélectionnée pour la création d'un événement.
    /// <br/>Peut être <see langword="null" /> si aucune équipe n'est sélectionnée.
    /// </summary>
    Guid? SelectedTeamId { get; set; }

    /// <summary>
    /// Indique si la fenêtre modale de création d'événement est affichée.
    /// </summary>
    bool ShowCreateEventModal { get; }

    /// <summary>
    /// Obtient le modèle de vue courant des événements.
    /// </summary>
    EventsViewModel ViewModel { get; }

    /// <summary>
    /// Ferme la fenêtre modale de création d'événement et réinitialise l'état si nécessaire.
    /// </summary>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task CloseCreateEventModalAsync();

    /// <summary>
    /// Confirme la participation de l'utilisateur à l'événement spécifié.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task ConfirmParticipationAsync(Guid eventId);

    /// <summary>
    /// Crée un nouvel événement à partir des données saisies dans <see cref="EventInput" /> et de <see cref="SelectedTeamId" />.
    /// </summary>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task CreateEventAsync();

    /// <summary>
    /// Décline la participation de l'utilisateur à l'événement spécifié.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task DeclineParticipationAsync(Guid eventId);

    /// <summary>
    /// Gère le flux de création d'événement (validation des données puis création).
    /// </summary>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task HandleCreateEventAsync();

    /// <summary>
    /// Charge ou actualise la liste des événements dans le <see cref="ViewModel" />.
    /// </summary>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task LoadEventsAsync();

    /// <summary>
    /// Ouvre la fenêtre modale de création d'événement et prépare le modèle de saisie.
    /// </summary>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task OpenCreateEventModalAsync();

    /// <summary>
    /// Ouvre la page de détail de l'événement spécifié.
    /// </summary>
    /// <param name="eventId">Identifiant de l'événement.</param>
    /// <returns>Tâche asynchrone représentant l'opération.</returns>
    Task OpenDetailPageAsync(Guid eventId);
}   