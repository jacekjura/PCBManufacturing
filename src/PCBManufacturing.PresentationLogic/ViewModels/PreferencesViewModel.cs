using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCBManufacturing.Core.Services.Catalog;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Localization.Keys;
using PCBManufacturing.Core.Models.Catalog;
using PCBManufacturing.Core.Models.Preferences;
using PCBManufacturing.Core.Services.Validation;
using PCBManufacturing.PresentationLogic;

namespace PCBManufacturing.PresentationLogic.ViewModels;

public sealed partial class PreferencesViewModel : ValidatingObservableObject
{
    private readonly IPostcodeValidator _postcodes;
    private readonly ILocalizationService _text;

    public PreferencesViewModel(ICatalogService catalogs, IPostcodeValidator postcodes, ILocalizationService text)
    {
        Catalog = catalogs.Load();
        _postcodes = postcodes;
        _text = text;
        Materials = Catalog.Materials.Select(material => new OptionViewModel
        {
            Id = material.Id,
            Value = material
        }).ToList();
        SolderMaskColors = Catalog.SolderMaskColors.Select(color => new OptionViewModel
        {
            Id = color.Id,
            Value = color,
            SwatchHex = color.Hex
        }).ToList();
        Thicknesses = Catalog.Thicknesses.Select(thickness => new OptionViewModel
        {
            Id = thickness.Id,
            Value = thickness
        }).ToList();
        Quantities = Catalog.Parameters.Where(parameter => parameter.Kind == ParameterKind.Quantity).Select(parameter => new OptionViewModel
        {
            Id = parameter.Id,
            Value = parameter
        }).ToList();

        Apply(SavedPreferences.CreateDefault(Catalog));
        _text.PropertyChanged += (_, _) => RefreshText();
    }

    public event EventHandler? DiscardRequested;
    public event EventHandler? ContinueRequested;

    public ManufacturingCatalog Catalog { get; }
    public ILocalizationService Text => _text;
    public IReadOnlyList<OptionViewModel> Materials { get; }
    public IReadOnlyList<OptionViewModel> SolderMaskColors { get; }
    public IReadOnlyList<OptionViewModel> Thicknesses { get; }
    public IReadOnlyList<OptionViewModel> Quantities { get; }

    [ObservableProperty] private OptionViewModel? selectedMaterial;
    [ObservableProperty] private OptionViewModel? selectedSolderMask;
    [ObservableProperty] private OptionViewModel? selectedThickness;
    [ObservableProperty] private OptionViewModel? selectedQuantity;
    [ObservableProperty] private string projectName = "AC-9001";
    [ObservableProperty] private string postcode = "";
    [ObservableProperty] private string notes = "";
    [ObservableProperty] private bool isPostcodeValid;
    [ObservableProperty] private bool isProjectValid;
    [ObservableProperty] private string postcodeMessage = "";
    [ObservableProperty] private string projectMessage = "";
    [ObservableProperty] private string summaryProject = "";
    [ObservableProperty] private string summaryPostcode = "";
    [ObservableProperty] private string summaryPostcodeState = "";
    [ObservableProperty] private string summaryMaterial = "";
    [ObservableProperty] private string summaryModifier = "";
    [ObservableProperty] private string summaryLead = "";
    [ObservableProperty] private string summaryMask = "";
    [ObservableProperty] private string summaryThickness = "";
    [ObservableProperty] private string summaryQuantity = "";
    [ObservableProperty] private string summaryNotes = "";
    [ObservableProperty] private string maskHex = "#1B7A3A";

    public bool HasPostcodeError => !IsPostcodeValid;
    public bool HasProjectError => !IsProjectValid;
    public MaterialDefinition? Material => SelectedMaterial?.Get<MaterialDefinition>();
    public SolderMaskColorDefinition? SolderMask => SelectedSolderMask?.Get<SolderMaskColorDefinition>();
    public BoardThicknessDefinition? Thickness => SelectedThickness?.Get<BoardThicknessDefinition>();
    public ParameterDefinition? Quantity => SelectedQuantity?.Get<ParameterDefinition>();

    public void Apply(SavedPreferences saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        ProjectName = string.IsNullOrWhiteSpace(saved.ProjectName) ? "AC-9001" : saved.ProjectName.Trim();
        Postcode = saved.Postcode?.Trim() ?? "";
        Notes = saved.Notes ?? "";
        SelectedMaterial = Find(Materials, saved.MaterialId);
        SelectedSolderMask = Find(SolderMaskColors, saved.SolderMaskColorId);
        SelectedThickness = Find(Thicknesses, saved.ThicknessId);
        SelectedQuantity = Find(Quantities, saved.QuantityId);
        ValidateProject();
        ValidatePostcode();
        RefreshText();
    }

    public void RefreshText()
    {
        foreach (var option in Materials)
        {
            var material = option.Get<MaterialDefinition>();
            option.Title = _text[material.NameKey];
            option.Impact = $"{_text.Format(Impact.Modifier, material.PriceModifier)} · {_text.Format(Summary.Days, material.ProductionDays)}";
        }

        foreach (var option in SolderMaskColors)
            option.Title = _text[option.Get<SolderMaskColorDefinition>().NameKey];

        foreach (var option in Thicknesses)
        {
            var thickness = option.Get<BoardThicknessDefinition>();
            option.Title = _text[thickness.NameKey];
            option.Impact = _text.Format(Impact.Modifier, thickness.PriceModifier);
        }

        foreach (var option in Quantities)
        {
            var quantity = option.Get<ParameterDefinition>();
            option.Title = $"{_text[quantity.NameKey]} · {_text[quantity.ValueKey]}";
            option.Impact = _text.Format(Impact.Modifier, quantity.PriceModifier);
        }

        SummaryProject = string.IsNullOrWhiteSpace(ProjectName) ? _text[Summary.Empty] : ProjectName;
        SummaryPostcode = string.IsNullOrWhiteSpace(Postcode) ? _text[Summary.Empty] : Postcode;
        SummaryPostcodeState = IsPostcodeValid ? _text[Summary.Valid] : _text[Summary.Invalid];
        SummaryMaterial = Material is null ? _text[Summary.Empty] : _text[Material.NameKey];
        SummaryModifier = Material is null ? _text[Impact.None] : _text.Format(Impact.Modifier, Material.PriceModifier);
        SummaryLead = Material is null ? _text[Impact.None] : _text.Format(Summary.Days, Material.ProductionDays);
        SummaryMask = SolderMask is null ? _text[Summary.Empty] : _text[SolderMask.NameKey];
        MaskHex = SolderMask?.Hex ?? "#1B7A3A";
        SummaryThickness = Thickness is null ? _text[Summary.Empty] : _text[Thickness.NameKey];
        SummaryQuantity = Quantity is null ? _text[Summary.Empty] : _text[Quantity.ValueKey];
        SummaryNotes = string.IsNullOrWhiteSpace(Notes) ? _text[Summary.Empty] : Notes.Trim();
        PostcodeMessage = IsPostcodeValid ? "" : PostcodeErrorText();
        ProjectMessage = IsProjectValid ? "" : _text[Validation.Project.Empty];
        SetErrors(nameof(Postcode), IsPostcodeValid ? [] : [PostcodeMessage]);
        SetErrors(nameof(ProjectName), IsProjectValid ? [] : [ProjectMessage]);
    }

    [RelayCommand]
    private void RequestDiscard() => DiscardRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void RequestContinue() => ContinueRequested?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedMaterialChanged(OptionViewModel? value) => RefreshText();
    partial void OnSelectedSolderMaskChanged(OptionViewModel? value) => RefreshText();
    partial void OnSelectedThicknessChanged(OptionViewModel? value) => RefreshText();
    partial void OnSelectedQuantityChanged(OptionViewModel? value) => RefreshText();
    partial void OnNotesChanged(string value) => RefreshText();

    partial void OnProjectNameChanged(string value)
    {
        ValidateProject();
        RefreshText();
    }

    partial void OnPostcodeChanged(string value)
    {
        ValidatePostcode();
        RefreshText();
    }

    partial void OnIsPostcodeValidChanged(bool value) => OnPropertyChanged(nameof(HasPostcodeError));
    partial void OnIsProjectValidChanged(bool value) => OnPropertyChanged(nameof(HasProjectError));

    private void ValidateProject() =>
        IsProjectValid = !string.IsNullOrWhiteSpace(ProjectName);

    private void ValidatePostcode() =>
        IsPostcodeValid = _postcodes.Validate(Postcode).IsValid;

    private string PostcodeErrorText() =>
        _postcodes.Validate(Postcode).Error == PostcodeError.Empty
            ? _text[Validation.Postcode.Empty]
            : _text[Validation.Postcode.Invalid];

    private static OptionViewModel? Find(IReadOnlyList<OptionViewModel> options, string? id) =>
        options.FirstOrDefault(option => option.Id == id) ?? options.FirstOrDefault();
}
