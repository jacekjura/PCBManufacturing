using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PCBManufacturing.Core.Models.Catalog;

namespace PCBManufacturing.PresentationLogic.ViewModels;

public sealed partial class ParameterRowViewModel : ObservableObject
{
    public ParameterRowViewModel(ParameterDefinition definition) => Definition = definition;
    public ParameterDefinition Definition { get; }
    [ObservableProperty] private string name = "";
    [ObservableProperty] private string value = "";
    [ObservableProperty] private string timeImpact = "";
    [ObservableProperty] private string costImpact = "";
    [ObservableProperty] private bool isSelected;
}
