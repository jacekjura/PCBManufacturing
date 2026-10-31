using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCBManufacturing.Core.Dialogs;
using PCBManufacturing.Core.Formatting;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Ordering;

namespace PCBManufacturing.ViewModels;

public sealed partial class OrderViewModel : ObservableObject
{
    private readonly PreferencesViewModel _preferences;
    private readonly QuoteViewModel _quote;
    private readonly IOrderService _orders;
    private readonly IOrderHistoryStore _history;
    private readonly IDialogService _dialogs;
    private readonly ILocalizationService _text;
    private readonly SynchronizationContext? _ui = UiThread.Capture();

    public OrderViewModel(
        PreferencesViewModel preferences,
        QuoteViewModel quote,
        IOrderService orders,
        IOrderHistoryStore history,
        IDialogService dialogs,
        ILocalizationService text)
    {
        _preferences = preferences;
        _quote = quote;
        _orders = orders;
        _history = history;
        _dialogs = dialogs;
        _text = text;
        _preferences.PropertyChanged += (_, _) => RefreshValidation();
        _quote.Changed += (_, _) => RefreshValidation();
        _text.PropertyChanged += (_, _) => RefreshText();
        History.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasOrders));
        RefreshValidation();
    }

    public ILocalizationService Text => _text;
    public PreferencesViewModel Preferences => _preferences;
    public QuoteViewModel Quote => _quote;
    public ObservableCollection<OrderHistoryItemViewModel> History { get; } = [];
    public ObservableCollection<string> ValidationMessages { get; } = [];

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasValidationErrors;
    [ObservableProperty] private string statusMessage = "";
    [ObservableProperty] private string placeOrderText = "";

    public bool HasOrders => History.Count > 0;

    public void LoadHistory(IReadOnlyList<OrderConfirmation> orders)
    {
        History.Clear();
        foreach (var order in orders)
            History.Add(ToItem(order));
        RefreshText();
    }

    public void RefreshText()
    {
        PlaceOrderText = IsBusy ? _text["Action.Submitting"] : _text["Action.PlaceOrder"];
        foreach (var item in History)
            ApplyItemText(item);
        RefreshValidation();
    }

    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    private async Task PlaceOrderAsync(CancellationToken cancellationToken)
    {
        var errors = CollectErrors();
        if (errors.Count > 0)
        {
            UiThread.Run(_ui, () => Replace(ValidationMessages, errors));
            await _dialogs.AlertAsync(new DialogContent
            {
                Title = _text["Order.ValidationTitle"],
                Message = _text["Order.ValidationMessage"],
                ConfirmText = _text["Action.Close"],
                Rows = errors.Select(error => new DialogRow { Label = _text["Order.Heading"], Value = error }).ToList()
            }, cancellationToken).ConfigureAwait(false);
            return;
        }

        var confirmed = await _dialogs.ConfirmAsync(BuildConfirmationDialog(), cancellationToken).ConfigureAwait(false);
        if (!confirmed || _quote.Result is null)
            return;

        UiThread.Run(_ui, () => IsBusy = true);
        try
        {
            var confirmation = await _orders.SubmitAsync(BuildDraft(), cancellationToken).ConfigureAwait(false);
            List<OrderConfirmation> snapshot = [];
            UiThread.Run(_ui, () =>
            {
                History.Insert(0, ToItem(confirmation));
                StatusMessage = _text.Format("Order.Confirmed", confirmation.Number);
                snapshot = History.Select(item => item.Model).ToList();
            });
            await _history.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            UiThread.Run(_ui, () => StatusMessage = exception.Message);
            await _dialogs.AlertAsync(new DialogContent
            {
                Title = _text["Order.FailureTitle"],
                Message = exception.Message,
                ConfirmText = _text["Action.Close"]
            }, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            UiThread.Run(_ui, () => IsBusy = false);
        }
    }

    private bool CanPlaceOrder() => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        PlaceOrderText = value ? _text["Action.Submitting"] : _text["Action.PlaceOrder"];
        PlaceOrderCommand.NotifyCanExecuteChanged();
    }

    private void RefreshValidation()
    {
        var errors = CollectErrors();
        Replace(ValidationMessages, errors);
        HasValidationErrors = errors.Count > 0;
    }

    private List<string> CollectErrors()
    {
        var errors = new List<string>();
        if (!_preferences.IsProjectValid)
            errors.Add(_text["Order.Error.Project"]);
        if (!_preferences.IsPostcodeValid)
            errors.Add(_text["Order.Error.Postcode"]);
        if (_preferences.Material is null)
            errors.Add(_text["Order.Error.Material"]);
        if (_preferences.SolderMask is null)
            errors.Add(_text["Order.Error.Mask"]);
        if (_preferences.Thickness is null)
            errors.Add(_text["Order.Error.Thickness"]);
        if (_preferences.Quantity is null)
            errors.Add(_text["Order.Error.Quantity"]);
        if (_quote.SelectedSize is null)
            errors.Add(_text["Order.Error.Size"]);
        if (_quote.SelectedLayers is null)
            errors.Add(_text["Order.Error.Layers"]);
        if (_quote.SelectedFinish is null)
            errors.Add(_text["Order.Error.Finish"]);
        if (_quote.Result is null)
            errors.Add(_text["Quote.Incomplete"]);
        return errors;
    }

    private DialogContent BuildConfirmationDialog()
    {
        var result = _quote.Result!;
        var rows = new List<DialogRow>
        {
            Row("Summary.Project", _preferences.ProjectName),
            Row("Summary.Postcode", _preferences.Postcode),
            Row("Summary.Material", _preferences.SummaryMaterial),
            Row("Summary.Mask", _preferences.SummaryMask),
            Row("Summary.Thickness", _preferences.SummaryThickness),
            Row("Quote.Legend.Size", _quote.LegendSize),
            Row("Quote.Legend.Layers", _quote.LegendLayers),
            Row("Quote.Legend.Finish", _quote.LegendFinish),
            Row("Summary.Quantity", _preferences.SummaryQuantity),
            Row("Quote.Total", Money.Format(result.Total, result.Currency, _text.CultureInfo)),
            Row("Quote.Lead", _text.Format("Summary.Days", result.ProductionDays))
        };
        if (!string.IsNullOrWhiteSpace(_preferences.Notes))
            rows.Add(Row("Order.Notes", _preferences.Notes.Trim()));

        return new DialogContent
        {
            Title = _text["Order.DialogTitle"],
            Message = _text["Order.DialogMessage"],
            ConfirmText = _text["Action.Confirm"],
            CancelText = _text["Action.Cancel"],
            Rows = rows
        };
    }

    private OrderDraft BuildDraft()
    {
        var result = _quote.Result ?? throw new InvalidOperationException("Quote is not ready.");
        return new OrderDraft
        {
            ProjectName = _preferences.ProjectName.Trim(),
            Postcode = _preferences.Postcode.Trim(),
            Notes = _preferences.Notes.Trim(),
            MaterialKey = _preferences.Material?.NameKey ?? "",
            MaskKey = _preferences.SolderMask?.NameKey ?? "",
            MaskHex = _preferences.MaskHex,
            ThicknessKey = _preferences.Thickness?.NameKey ?? "",
            SizeKey = _quote.SelectedSize?.NameKey ?? "",
            FinishKey = _quote.SelectedFinish?.NameKey ?? "",
            Layers = _quote.SelectedLayers?.LayerCount ?? 0,
            Quantity = result.Quantity,
            Total = result.Total,
            Currency = result.Currency,
            ProductionDays = result.ProductionDays
        };
    }

    private OrderHistoryItemViewModel ToItem(OrderConfirmation confirmation)
    {
        var item = new OrderHistoryItemViewModel { Model = confirmation };
        ApplyItemText(item);
        return item;
    }

    private void ApplyItemText(OrderHistoryItemViewModel item)
    {
        var model = item.Model;
        item.Title = model.Number;
        item.MaskHex = model.MaskHex;
        item.When = model.CreatedAt.ToLocalTime().ToString("g", _text.CultureInfo);
        item.Detail = string.Join(" · ", new[]
        {
            model.ProjectName,
            _text[model.MaterialKey],
            _text[model.SizeKey],
            _text[model.FinishKey],
            _text.Format("Summary.Days", model.ProductionDays),
            Money.Format(model.Total, model.Currency, _text.CultureInfo)
        });
    }

    private DialogRow Row(string labelKey, string value) =>
        new() { Label = _text[labelKey], Value = value };

    private static void Replace(ObservableCollection<string> target, IReadOnlyList<string> values)
    {
        if (target.SequenceEqual(values))
            return;
        target.Clear();
        foreach (var value in values)
            target.Add(value);
    }
}
