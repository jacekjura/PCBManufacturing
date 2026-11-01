using CommunityToolkit.Mvvm.ComponentModel;

namespace PCBManufacturing.ViewModels;

public sealed partial class QuoteLineViewModel : ObservableObject
{
    public required string LabelKey { get; init; }
    public decimal Amount { get; init; }
    public int Days { get; init; }

    [ObservableProperty] private string label = "";
    [ObservableProperty] private string amountText = "";
    [ObservableProperty] private string daysText = "";
}

public sealed partial class OrderHistoryItemViewModel : ObservableObject
{
    public required Core.Models.OrderConfirmation Model { get; init; }

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string detail = "";
    [ObservableProperty] private string when = "";
    [ObservableProperty] private string maskHex = "#1B7A3A";
}
