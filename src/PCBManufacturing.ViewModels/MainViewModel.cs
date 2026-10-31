using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCBManufacturing.Core.Catalog;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Ordering;
using PCBManufacturing.Core.Preferences;
using PCBManufacturing.Core.Theming;

namespace PCBManufacturing.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IPreferencesStore _store;
    private readonly IOrderHistoryStore _history;
    private readonly ILocalizationService _text;
    private readonly IThemeApplier _themeApplier;
    private readonly ManufacturingCatalog _catalog;
    private readonly SynchronizationContext? _ui = UiThread.Capture();
    private SavedPreferences _lastSaved = new();

    public MainViewModel(
        PreferencesViewModel preferences,
        QuoteViewModel quote,
        OrderViewModel order,
        IPreferencesStore store,
        IOrderHistoryStore history,
        ILocalizationService text,
        IThemeApplier themeApplier,
        ICatalogService catalogs)
    {
        Preferences = preferences;
        Quote = quote;
        Order = order;
        _store = store;
        _history = history;
        _text = text;
        _themeApplier = themeApplier;
        _catalog = catalogs.Load();
        Localization = text;
        Preferences.DiscardRequested += (_, _) => Discard();
        Preferences.ContinueRequested += async (_, _) => await ContinueAsync();
        Preferences.PropertyChanged += (_, _) => UpdateStatus();
        Quote.Changed += (_, _) => UpdateStatus();
        _text.PropertyChanged += (_, _) => UpdateStatus();
    }

    public PreferencesViewModel Preferences { get; }
    public QuoteViewModel Quote { get; }
    public OrderViewModel Order { get; }
    public ILocalizationService Localization { get; }
    public IAsyncRelayCommand PlaceOrderCommand => Order.PlaceOrderCommand;

    [ObservableProperty] private int selectedTabIndex;
    [ObservableProperty] private AppTheme theme = AppTheme.Light;
    [ObservableProperty] private string culture = "en";
    [ObservableProperty] private string statusText = "";
    [ObservableProperty] private bool isDirty;

    public bool IsEnglish
    {
        get => Culture == "en";
        set
        {
            if (value)
                ChangeCulture("en");
        }
    }

    public bool IsRussian
    {
        get => Culture == "ru";
        set
        {
            if (value)
                ChangeCulture("ru");
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var saved = await _store.LoadAsync(cancellationToken).ConfigureAwait(true);
        var history = await _history.LoadAsync(cancellationToken).ConfigureAwait(true);
        var snapshot = saved ?? SavedPreferences.CreateDefault(_catalog);
        Apply(snapshot);
        Order.LoadHistory(history);
        _lastSaved = Capture();
        _themeApplier.Apply(Theme);
        UpdateStatus();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var snapshot = Capture();
        await _store.SaveAsync(snapshot).ConfigureAwait(false);
        UiThread.Run(_ui, () =>
        {
            _lastSaved = snapshot;
            UpdateStatus();
        });
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        await SaveAsync().ConfigureAwait(true);
        UiThread.Run(_ui, () => SelectedTabIndex = 1);
    }

    [RelayCommand]
    private void Discard()
    {
        var defaults = SavedPreferences.CreateDefault(_catalog) with
        {
            Theme = Theme,
            Culture = Culture
        };
        Preferences.Apply(defaults);
        Quote.Select(defaults.SizeId, defaults.LayersId, defaults.FinishId);
        RefreshText();
        UpdateStatus();
    }

    [RelayCommand]
    private void ToggleTheme() =>
        Theme = Theme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;

    public bool IsDark => Theme == AppTheme.Dark;

    partial void OnThemeChanged(AppTheme value)
    {
        OnPropertyChanged(nameof(IsDark));
        _themeApplier.Apply(value);
        UpdateStatus();
    }

    partial void OnCultureChanged(string value)
    {
        OnPropertyChanged(nameof(IsEnglish));
        OnPropertyChanged(nameof(IsRussian));
    }

    private void ChangeCulture(string culture)
    {
        _text.SetCulture(culture);
        Culture = _text.Culture;
        RefreshText();
        UpdateStatus();
    }

    private void Apply(SavedPreferences saved)
    {
        _text.SetCulture(saved.Culture);
        Culture = _text.Culture;
        Theme = saved.Theme;
        Preferences.Apply(saved);
        Quote.Select(saved.SizeId, saved.LayersId, saved.FinishId);
        RefreshText();
    }

    private void RefreshText()
    {
        Preferences.RefreshText();
        Quote.RefreshText();
        Order.RefreshText();
    }

    private SavedPreferences Capture() => new()
    {
        ProjectName = Preferences.ProjectName,
        Postcode = Preferences.Postcode,
        Notes = Preferences.Notes,
        MaterialId = Preferences.SelectedMaterial?.Id,
        SolderMaskColorId = Preferences.SelectedSolderMask?.Id,
        ThicknessId = Preferences.SelectedThickness?.Id,
        QuantityId = Preferences.SelectedQuantity?.Id,
        SizeId = Quote.SelectedSize?.Id,
        LayersId = Quote.SelectedLayers?.Id,
        FinishId = Quote.SelectedFinish?.Id,
        Theme = Theme,
        Culture = Culture
    };

    private void UpdateStatus()
    {
        IsDirty = !Capture().Equals(_lastSaved);
        StatusText = IsDirty ? _text["Status.Dirty"] : _text["Status.Saved"];
    }
}
