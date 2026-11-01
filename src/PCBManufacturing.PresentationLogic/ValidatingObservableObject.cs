using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PCBManufacturing.PresentationLogic;

public abstract partial class ValidatingObservableObject : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new(StringComparer.Ordinal);

    public bool HasErrors => _errors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _errors.SelectMany(entry => entry.Value).ToArray();

        return _errors.TryGetValue(propertyName, out var errors) ? errors.ToArray() : [];
    }

    protected void SetErrors(string propertyName, IReadOnlyList<string> errors)
    {
        var next = errors.Where(error => !string.IsNullOrWhiteSpace(error)).ToList();
        if (next.Count == 0)
        {
            if (!_errors.Remove(propertyName))
                return;
        }
        else if (_errors.TryGetValue(propertyName, out var current) && current.SequenceEqual(next))
        {
            return;
        }
        else
        {
            _errors[propertyName] = next;
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }
}
