using PCBManufacturing.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PCBManufacturing.AvaloniaApp.Views;

public partial class PreferencesView : ReactiveUserControl<PreferencesViewModel>
{
    public PreferencesView()
    {
        InitializeComponent();
        this.WhenActivated((Action<IDisposable> register) =>
        {
            _ = register;
            _ = ViewTransitions.FadeInAsync(this);
        });
    }
}
