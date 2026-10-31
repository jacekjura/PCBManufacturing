using PCBManufacturing.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PCBManufacturing.AvaloniaApp.Views;

public partial class OrderView : ReactiveUserControl<OrderViewModel>
{
    public OrderView()
    {
        InitializeComponent();
        this.WhenActivated((Action<IDisposable> register) =>
        {
            _ = register;
            _ = ViewTransitions.FadeInAsync(this);
        });
    }
}
