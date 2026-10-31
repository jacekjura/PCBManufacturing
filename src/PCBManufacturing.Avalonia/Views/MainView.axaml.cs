using PCBManufacturing.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PCBManufacturing.AvaloniaApp.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
        this.WhenActivated((Action<IDisposable> register) =>
        {
            _ = register;
            _ = ViewTransitions.FadeInAsync(this);
        });
    }
}
