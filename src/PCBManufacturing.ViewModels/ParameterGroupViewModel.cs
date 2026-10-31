using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.ViewModels;

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

public sealed partial class ParameterGroupViewModel : ObservableObject
{
    private bool _syncing;

    public ParameterGroupViewModel(ParameterKind kind, string groupKey, IReadOnlyList<ParameterDefinition> parameters)
    {
        Kind = kind;
        GroupKey = groupKey;
        foreach (var parameter in parameters)
        {
            var row = new ParameterRowViewModel(parameter);
            row.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(ParameterRowViewModel.IsSelected) && row.IsSelected)
                    Select(row);
            };
            Rows.Add(row);
        }

        var initial = Rows.FirstOrDefault(row => row.Definition.IsDefault) ?? Rows[0];
        Select(initial);
    }

    public ParameterKind Kind { get; }
    public string GroupKey { get; }
    public ObservableCollection<ParameterRowViewModel> Rows { get; } = [];

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string columnParameter = "";
    [ObservableProperty] private string columnValue = "";
    [ObservableProperty] private string columnTime = "";
    [ObservableProperty] private string columnCost = "";
    [ObservableProperty] private ParameterRowViewModel? selectedRow;

    public event EventHandler? SelectionChanged;

    partial void OnSelectedRowChanged(ParameterRowViewModel? value)
    {
        if (_syncing)
            return;

        if (value is null)
        {
            var current = Rows.FirstOrDefault(row => row.IsSelected);
            if (current is not null)
                SelectedRow = current;
            return;
        }

        Select(value);
    }

    public void Select(ParameterRowViewModel row)
    {
        if (_syncing)
            return;

        _syncing = true;
        try
        {
            foreach (var other in Rows)
                other.IsSelected = ReferenceEquals(other, row);

            if (!ReferenceEquals(SelectedRow, row))
                SelectedRow = row;
        }
        finally
        {
            _syncing = false;
        }

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}
