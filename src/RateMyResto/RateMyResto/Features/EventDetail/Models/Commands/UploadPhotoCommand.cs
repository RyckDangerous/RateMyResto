namespace RateMyResto.Features.EventDetail.Models.Commands;

public sealed record UploadPhotoCommand
{
    /// <summary>
    /// L'Id de l'événement.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Nom de l'image.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Le flux de l'image (stream direct sans copie en mémoire).
    /// </summary>
    public required Stream ImageStream { get; init; }



}
