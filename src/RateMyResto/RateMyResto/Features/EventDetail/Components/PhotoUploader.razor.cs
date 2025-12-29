using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace RateMyResto.Features.EventDetail.Components;

public partial class PhotoUploader : ComponentBase
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 Mo
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    [Parameter]
    public EventCallback<IBrowserFile> OnPhotoSelected { get; set; }

    private List<string> _errorMessages = new();
    private bool _isUploading = false;

    /// <summary>
    /// Gère la sélection d'un fichier via l'InputFile.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        _errorMessages.Clear();
        
        var file = e.File;
        if (file == null)
            return;

        // Vérification de l'extension
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            _errorMessages.Add($"Format non accepté. Seules les images sont autorisées (JPG, PNG, GIF, WEBP).");
            return;
        }

        // Vérification de la taille
        if (file.Size > MaxFileSize)
        {
            _errorMessages.Add($"Fichier trop volumineux (max 5 Mo). Taille actuelle : {FormatFileSize(file.Size)}.");
            return;
        }

        // Upload immédiat
        await UploadPhoto(file);
    }

    /// <summary>
    /// Upload la photo sélectionnée.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task UploadPhoto(IBrowserFile file)
    {
        _isUploading = true;
        _errorMessages.Clear();
        
        try
        {
            // Envoyer le fichier au parent
            await OnPhotoSelected.InvokeAsync(file);
        }
        catch (Exception ex)
        {
            _errorMessages.Add($"Erreur lors de l'envoi : {ex.Message}");
        }
        finally
        {
            _isUploading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Formate la taille d'un fichier en une chaîne lisible.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "o", "Ko", "Mo", "Go" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}

