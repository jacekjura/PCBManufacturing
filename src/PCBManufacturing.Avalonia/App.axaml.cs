using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCBManufacturing.Core.Services.Dialogs;
using PCBManufacturing.Core.Services.Theming;
using PCBManufacturing.PresentationLogic;
using PCBManufacturing.PresentationLogic.ViewModels;
using PCBManufacturing.AvaloniaApp.Services;

namespace PCBManufacturing.AvaloniaApp;

public partial class App : Application
{
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                var builder = Host.CreateApplicationBuilder();
                var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCBManufacturing");
                builder.Services.AddPcbManufacturing(AppContext.BaseDirectory, appData);
                builder.Services.AddSingleton<IDialogService, AvaloniaDialogService>();
                builder.Services.AddSingleton<IThemeApplier, AvaloniaThemeApplier>();
                _host = builder.Build();

                var main = _host.Services.GetRequiredService<MainViewModel>();
                await main.InitializeAsync();
                desktop.MainWindow = new MainWindow { DataContext = main };
                desktop.ShutdownRequested += (_, _) => _host.Dispose();
            }
            catch (Exception exception)
            {
                desktop.MainWindow = new Window
                {
                    Title = "PCB Manufacturing",
                    Width = 640,
                    Height = 360,
                    Content = new TextBlock
                    {
                        Text = exception.ToString(),
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(24)
                    }
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
            && lifetime.MainWindow is { IsVisible: false } window)
        {
            window.Show();
        }
    }
}
