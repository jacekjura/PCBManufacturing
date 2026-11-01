using System.Windows;

namespace PCBManufacturing.Wpf.Behaviors;

public static class DataGridColumns
{
    public static readonly DependencyProperty ParameterHeaderProperty = Register(nameof(ParameterHeaderProperty));
    public static readonly DependencyProperty ValueHeaderProperty = Register(nameof(ValueHeaderProperty));
    public static readonly DependencyProperty TimeHeaderProperty = Register(nameof(TimeHeaderProperty));
    public static readonly DependencyProperty CostHeaderProperty = Register(nameof(CostHeaderProperty));
    public static void SetParameterHeader(DependencyObject element, string? value) => element.SetValue(ParameterHeaderProperty, value);
    public static string? GetParameterHeader(DependencyObject element) => (string?)element.GetValue(ParameterHeaderProperty);
    public static void SetValueHeader(DependencyObject element, string? value) => element.SetValue(ValueHeaderProperty, value);
    public static string? GetValueHeader(DependencyObject element) => (string?)element.GetValue(ValueHeaderProperty);
    public static void SetTimeHeader(DependencyObject element, string? value) => element.SetValue(TimeHeaderProperty, value);
    public static string? GetTimeHeader(DependencyObject element) => (string?)element.GetValue(TimeHeaderProperty);
    public static void SetCostHeader(DependencyObject element, string? value) => element.SetValue(CostHeaderProperty, value);
    public static string? GetCostHeader(DependencyObject element) => (string?)element.GetValue(CostHeaderProperty);
    private static DependencyProperty Register(string name) =>
        DependencyProperty.RegisterAttached(name.Replace("Property", "", StringComparison.Ordinal), typeof(string), typeof(DataGridColumns), new PropertyMetadata(null, OnChanged));
    private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not System.Windows.Controls.DataGrid grid)
            return;
        if (grid.IsLoaded)
            Apply(grid);
        else
            grid.Loaded += (_, _) => Apply(grid);
    }
    private static void Apply(System.Windows.Controls.DataGrid grid)
    {
        if (grid.Columns.Count < 5)
            return;
        grid.Columns[1].Header = GetParameterHeader(grid);
        grid.Columns[2].Header = GetValueHeader(grid);
        grid.Columns[3].Header = GetTimeHeader(grid);
        grid.Columns[4].Header = GetCostHeader(grid);
    }
}
