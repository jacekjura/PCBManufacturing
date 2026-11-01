using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCBManufacturing.Core.Services.Dialogs;
using PCBManufacturing.Core.Services.Theming;
using PCBManufacturing.PresentationLogic;
using PCBManufacturing.PresentationLogic.ViewModels;
using PCBManufacturing.Wpf.Services;

namespace PCBManufacturing.Wpf;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += (_, args) =>
        {
            MessageBox.Show(args.Exception.Message, "PCB Manufacturing", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        try
        {
            var builder = Host.CreateApplicationBuilder();
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCBManufacturing");
            builder.Services.AddPcbManufacturing(AppContext.BaseDirectory, appData);
            builder.Services.AddSingleton<IDialogService, WpfDialogService>();
            builder.Services.AddSingleton<IThemeApplier, WpfThemeApplier>();
            _host = builder.Build();

            var main = _host.Services.GetRequiredService<MainViewModel>();
            await main.InitializeAsync();
            var window = new MainWindow { DataContext = main };
            MainWindow = window;
            window.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.ToString(), "PCB Manufacturing", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
