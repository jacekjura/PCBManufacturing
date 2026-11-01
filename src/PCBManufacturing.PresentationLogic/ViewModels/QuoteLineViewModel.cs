using CommunityToolkit.Mvvm.ComponentModel;

namespace PCBManufacturing.PresentationLogic.ViewModels;

public sealed partial class QuoteLineViewModel : ObservableObject
{
    public required string LabelKey { get; init; }
    public decimal Amount { get; init; }
    public int Days { get; init; }

    [ObservableProperty] private string label = "";
    [ObservableProperty] private string amountText = "";
    [ObservableProperty] private string daysText = "";
}
