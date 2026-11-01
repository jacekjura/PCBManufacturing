using CommunityToolkit.Mvvm.ComponentModel;

namespace PCBManufacturing.PresentationLogic.ViewModels;

public sealed partial class OrderHistoryItemViewModel : ObservableObject
{
    public required Core.Models.Ordering.OrderConfirmation Model { get; init; }
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string detail = "";
    [ObservableProperty] private string when = "";
    [ObservableProperty] private string maskHex = "#1B7A3A";
}
