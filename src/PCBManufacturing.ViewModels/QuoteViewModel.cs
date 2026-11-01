using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PCBManufacturing.Core.Formatting;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Pricing;

namespace PCBManufacturing.ViewModels;

public sealed partial class QuoteViewModel : ObservableObject
{
    private readonly PreferencesViewModel _preferences;
    private readonly IQuoteCalculator _calculator;
    private readonly IPcbArtworkFactory _artwork;
    private readonly ILocalizationService _text;
    private QuoteResult? _result;

    public QuoteViewModel(
        PreferencesViewModel preferences,
        IQuoteCalculator calculator,
        IPcbArtworkFactory artwork,
        ILocalizationService text)
    {
        _preferences = preferences;
        _calculator = calculator;
        _artwork = artwork;
        _text = text;

        foreach (var kind in new[] { ParameterKind.Size, ParameterKind.Layers, ParameterKind.Finish })
        {
            var parameters = preferences.Catalog.Parameters.Where(parameter => parameter.Kind == kind).ToList();
            var group = new ParameterGroupViewModel(kind, parameters[0].GroupKey, parameters);
            group.SelectionChanged += (_, _) => Recalculate();
            Groups.Add(group);
        }

        _preferences.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(PreferencesViewModel.SelectedMaterial)
                or nameof(PreferencesViewModel.SelectedSolderMask)
                or nameof(PreferencesViewModel.SelectedThickness)
                or nameof(PreferencesViewModel.SelectedQuantity)
                or null)
            {
                Recalculate();
            }
        };

        _text.PropertyChanged += (_, _) => RefreshText();
        Recalculate();
    }

    public ILocalizationService Text => _text;
    public ObservableCollection<ParameterGroupViewModel> Groups { get; } = [];
    public ObservableCollection<QuoteLineViewModel> Lines { get; } = [];
    public ObservableCollection<VectorShape> BoardShapes { get; } = [];
    public ObservableCollection<VectorShape> LayerShapes { get; } = [];

    public double BoardCanvasWidth => PcbArtworkFactory.BoardWidth;
    public double BoardCanvasHeight => PcbArtworkFactory.BoardHeight;
    public double StackCanvasWidth => PcbArtworkFactory.StackWidth;
    public double StackCanvasHeight => PcbArtworkFactory.StackHeight;

    [ObservableProperty] private string totalText = "";
    [ObservableProperty] private string unitText = "";
    [ObservableProperty] private string leadText = "";
    [ObservableProperty] private string legendSize = "";
    [ObservableProperty] private string legendLayers = "";
    [ObservableProperty] private string legendFinish = "";
    [ObservableProperty] private string legendMask = "";
    [ObservableProperty] private string previewMaskHex = "#1B7A3A";
    [ObservableProperty] private string incompleteText = "";
    [ObservableProperty] private bool hasQuote;
    [ObservableProperty] private int previewRevision;

    public QuoteResult? Result => _result;
    public ParameterDefinition? SelectedSize => Selected(ParameterKind.Size);
    public ParameterDefinition? SelectedLayers => Selected(ParameterKind.Layers);
    public ParameterDefinition? SelectedFinish => Selected(ParameterKind.Finish);

    public event EventHandler? Changed;

    public void Select(string? sizeId, string? layersId, string? finishId)
    {
        Choose(ParameterKind.Size, sizeId);
        Choose(ParameterKind.Layers, layersId);
        Choose(ParameterKind.Finish, finishId);
        Recalculate();
    }

    public void RefreshText()
    {
        foreach (var group in Groups)
        {
            group.Title = _text[group.GroupKey];
            group.ColumnParameter = _text[Quote.Column.Parameter];
            group.ColumnValue = _text[Quote.Column.Value];
            group.ColumnTime = _text[Quote.Column.Time];
            group.ColumnCost = _text[Quote.Column.Cost];
            foreach (var row in group.Rows)
            {
                row.Name = _text[row.Definition.NameKey];
                row.Value = _text[row.Definition.ValueKey];
                row.TimeImpact = row.Definition.ProductionDaysExtra == 0
                    ? _text[Impact.None]
                    : _text.Format(Impact.Days, row.Definition.ProductionDaysExtra);
                row.CostImpact = row.Definition.PriceModifier == 1m
                    ? _text[Impact.Included]
                    : _text.Format(Impact.Modifier, row.Definition.PriceModifier);
            }
        }

        FormatResult();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void Recalculate()
    {
        var material = _preferences.Material;
        var thickness = _preferences.Thickness;
        var quantity = _preferences.Quantity;
        var size = SelectedSize;
        var layers = SelectedLayers;
        var finish = SelectedFinish;
        if (material is null || thickness is null || quantity is null || size is null || layers is null || finish is null)
        {
            _result = null;
            HasQuote = false;
            Replace(BoardShapes, []);
            Replace(LayerShapes, []);
            Lines.Clear();
            FormatResult();
            Changed?.Invoke(this, EventArgs.Empty);
            return;
        }

        _result = _calculator.Calculate(new QuoteRequest
        {
            BasePricePerBoard = _preferences.Catalog.BasePricePerBoard,
            Currency = _preferences.Catalog.Currency,
            Material = material,
            Thickness = thickness,
            Size = size,
            Layers = layers,
            Finish = finish,
            Quantity = quantity
        });
        HasQuote = true;

        var request = new PcbArtworkRequest
        {
            WidthMm = size.WidthMm ?? 100,
            HeightMm = size.HeightMm ?? 80,
            LayerCount = layers.LayerCount ?? 2,
            ThicknessMm = thickness.Millimeters,
            MaskHex = _preferences.MaskHex,
            SubstrateHex = material.SubstrateHex,
            FinishId = finish.FinishId ?? "hasl"
        };
        Replace(BoardShapes, _artwork.CreateBoard(request));
        Replace(LayerShapes, _artwork.CreateLayerStack(request));
        PreviewRevision++;
        FormatResult();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void FormatResult()
    {
        var culture = _text.CultureInfo;
        var currency = _preferences.Catalog.Currency;
        IncompleteText = _text[Quote.Incomplete];
        if (_result is null)
        {
            UnitText = _text[Impact.None];
            TotalText = _text[Impact.None];
            LeadText = _text[Impact.None];
            Lines.Clear();
        }
        else
        {
            UnitText = Money.Format(_result.UnitPrice, currency, culture);
            TotalText = Money.Format(_result.Total, currency, culture);
            LeadText = _text.Format(Summary.Days, _result.ProductionDays);
            Lines.Clear();
            foreach (var line in _result.Lines)
            {
                Lines.Add(new QuoteLineViewModel
                {
                    LabelKey = line.LabelKey,
                    Amount = line.Amount,
                    Days = line.Days,
                    Label = _text[line.LabelKey],
                    AmountText = Money.Format(line.Amount, currency, culture),
                    DaysText = line.Days == 0 ? _text[Impact.None] : _text.Format(Impact.Days, line.Days)
                });
            }
        }

        var size = SelectedSize;
        var layers = SelectedLayers;
        var finish = SelectedFinish;
        LegendSize = size is null ? _text[Impact.None] : _text[size.ValueKey];
        LegendLayers = layers is null ? _text[Impact.None] : _text[layers.ValueKey];
        LegendFinish = finish is null ? _text[Impact.None] : _text[finish.NameKey];
        LegendMask = _preferences.SummaryMask;
        PreviewMaskHex = _preferences.MaskHex;
        foreach (var group in Groups)
        {
            group.ColumnParameter = _text[Quote.Column.Parameter];
            group.ColumnValue = _text[Quote.Column.Value];
            group.ColumnTime = _text[Quote.Column.Time];
            group.ColumnCost = _text[Quote.Column.Cost];
            group.Title = _text[group.GroupKey];
        }
    }

    private void Choose(ParameterKind kind, string? id)
    {
        var group = Groups.First(item => item.Kind == kind);
        var row = group.Rows.FirstOrDefault(item => item.Definition.Id == id)
            ?? group.Rows.First(item => item.Definition.IsDefault);
        group.Select(row);
    }

    private ParameterDefinition? Selected(ParameterKind kind) =>
        Groups.FirstOrDefault(group => group.Kind == kind)?.SelectedRow?.Definition;

    private static void Replace(ObservableCollection<VectorShape> target, IReadOnlyList<VectorShape> shapes)
    {
        target.Clear();
        foreach (var shape in shapes)
            target.Add(shape);
    }
}
