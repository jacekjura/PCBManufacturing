using System.ComponentModel;
using Avalonia.Controls;
using PCBManufacturing.PresentationLogic.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace PCBManufacturing.AvaloniaApp.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    private MainViewModel? _sections;

    public MainView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => AttachSections();
        this.WhenActivated((Action<IDisposable> register) =>
        {
            _ = register;
            _ = ViewTransitions.FadeInAsync(this);
        });
    }

    private void AttachSections()
    {
        if (_sections is not null)
            _sections.PropertyChanged -= OnSectionChanged;

        _sections = DataContext as MainViewModel;
        if (_sections is not null)
            _sections.PropertyChanged += OnSectionChanged;

        ApplySectionRows();
    }

    private void OnSectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.IsPreferencesExpanded)
            or nameof(MainViewModel.IsQuoteExpanded)
            or nameof(MainViewModel.IsOrdersExpanded)
            or null
            or "")
        {
            ApplySectionRows();
        }
    }

    private void ApplySectionRows()
    {
        if (_sections is null)
            return;

        var rows = Sections.RowDefinitions;
        var share = new GridLength(1, GridUnitType.Star);
        rows[0].MinHeight = 0;
        rows[0].Height = _sections.IsPreferencesExpanded ? share : GridLength.Auto;
        rows[1].Height = _sections.IsQuoteExpanded ? share : GridLength.Auto;
        rows[2].Height = _sections.IsOrdersExpanded ? share : GridLength.Auto;
    }
}
