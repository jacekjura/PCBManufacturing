using System.ComponentModel;
using PCBManufacturing.PresentationLogic.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PCBManufacturing.AvaloniaApp.Views;

public partial class QuoteView : ReactiveUserControl<QuoteViewModel>
{
    public QuoteView()
    {
        InitializeComponent();
        this.WhenActivated((Action<IDisposable> register) =>
        {
            _ = ViewTransitions.FadeInAsync(this);
            if (ViewModel is null)
                return;

            PropertyChangedEventHandler handler = (_, args) =>
            {
                if (args.PropertyName == nameof(QuoteViewModel.PreviewRevision))
                    _ = ViewTransitions.PulseAsync(PreviewHost);
            };

            ViewModel.PropertyChanged += handler;
            register(new Subscription(() =>
            {
                if (ViewModel is not null)
                    ViewModel.PropertyChanged -= handler;
            }));
        });
    }

    private sealed class Subscription(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}
