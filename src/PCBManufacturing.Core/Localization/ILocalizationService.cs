using System.ComponentModel;
using System.Globalization;

namespace PCBManufacturing.Core.Localization;

public interface ILocalizationService : INotifyPropertyChanged
{
    string Culture { get; }
    CultureInfo CultureInfo { get; }
    string this[string key] { get; }
    void SetCulture(string culture);
    string Format(string key, params object[] args);
}
