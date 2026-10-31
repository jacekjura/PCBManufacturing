using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PCBManufacturing.AvaloniaApp.Views;
using PCBManufacturing.Core.Dialogs;
using PCBManufacturing.Core.Models;
using PCBManufacturing.ViewModels;

namespace PCBManufacturing.AvaloniaApp.Services;

public sealed class AvaloniaDialogService : IDialogService
{
    public Task<bool> ConfirmAsync(DialogContent content, CancellationToken cancellationToken = default) =>
        ShowAsync(content);

    public async Task AlertAsync(DialogContent content, CancellationToken cancellationToken = default) =>
        await ShowAsync(content);

    private static async Task<bool> ShowAsync(DialogContent content)
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
            ?? throw new InvalidOperationException("The Avalonia main window is not available.");
        var viewModel = new DialogViewModel(content);
        var dialog = new MessageDialogWindow { DataContext = viewModel };
        viewModel.CloseRequested += (_, result) => dialog.Close(result);
        return await dialog.ShowDialog<bool>(owner);
    }
}
