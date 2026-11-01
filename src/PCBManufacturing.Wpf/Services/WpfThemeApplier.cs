using System.Windows;
using PCBManufacturing.Core.Models.Theming;

namespace PCBManufacturing.Wpf.Services;

public sealed class WpfThemeApplier : Core.Services.Theming.IThemeApplier
{
    public void Apply(AppTheme theme)
    {
        var application = Application.Current ?? throw new InvalidOperationException("The WPF application is not running.");
        var source = theme == AppTheme.Dark ? "Resources/Dark.xaml" : "Resources/Light.xaml";
        var dictionary = new ResourceDictionary { Source = new Uri(source, UriKind.Relative) };
        var merged = application.Resources.MergedDictionaries;
        if (merged.Count == 0)
            merged.Add(dictionary);
        else
            merged[0] = dictionary;
    }
}
