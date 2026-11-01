using CommunityToolkit.Mvvm.ComponentModel;

namespace PCBManufacturing.PresentationLogic.ViewModels;

public sealed partial class OptionViewModel : ObservableObject
{
    public required string Id { get; init; }
    public required object Value { get; init; }
    public string? SwatchHex { get; init; }
    public bool HasSwatch => !string.IsNullOrWhiteSpace(SwatchHex);

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string impact = "";

    public T Get<T>() where T : class => (T)Value;

    public override string ToString() => string.IsNullOrWhiteSpace(Title) ? Id : Title;
}
