using System.Windows;
using PCBManufacturing.Core.Services.Dialogs;
using PCBManufacturing.Core.Models.Dialogs;
using PCBManufacturing.PresentationLogic.ViewModels;
using PCBManufacturing.Wpf.Views;

namespace PCBManufacturing.Wpf.Services;

public sealed class WpfDialogService : IDialogService
{
    public Task<bool> ConfirmAsync(DialogContent content, CancellationToken cancellationToken = default) =>
        ShowAsync(content);

    public async Task AlertAsync(DialogContent content, CancellationToken cancellationToken = default) =>
        await ShowAsync(content);

    private static Task<bool> ShowAsync(DialogContent content)
    {
        var dispatcher = Application.Current?.Dispatcher ?? throw new InvalidOperationException("The WPF application is not running.");
        if (dispatcher.CheckAccess())
            return Task.FromResult(Show(content));

        return dispatcher.InvokeAsync(() => Show(content)).Task;
    }

    private static bool Show(DialogContent content)
    {
        var viewModel = new DialogViewModel(content);
        var window = new MessageDialogWindow(viewModel)
        {
            Owner = Application.Current?.MainWindow
        };
        return window.ShowDialog() == true;
    }
}
