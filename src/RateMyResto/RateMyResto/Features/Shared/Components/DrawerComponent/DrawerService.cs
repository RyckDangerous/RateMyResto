using Microsoft.AspNetCore.Components;

namespace RateMyResto.Features.Shared.Components.DrawerComponent;

/// <summary>
/// Implémentation du service de gestion du Drawer
/// </summary>
public sealed class DrawerService : IDrawerService
{
    /// <inheritdoc />
    public event Action<DrawerSettings>? OnDrawerOpened;

    /// <inheritdoc />
    public event Action? OnDrawerClosed;

    /// <inheritdoc />
    public bool IsOpen { get; private set; }

    /// <inheritdoc />
    public void Open(DrawerSettings settings)
    {
        IsOpen = true;
        OnDrawerOpened?.Invoke(settings);
    }

    /// <inheritdoc />
    public void Open(string title, RenderFragment content)
    {
        Open(new DrawerSettings
        {
            Title = title,
            Content = content
        });
    }

    /// <inheritdoc />
    public void Open(string title, string titleIcon, RenderFragment content)
    {
        Open(new DrawerSettings
        {
            Title = title,
            TitleIcon = titleIcon,
            Content = content
        });
    }

    /// <inheritdoc />
    public void Close()
    {
        IsOpen = false;
        OnDrawerClosed?.Invoke();
    }
}

