using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.ViewModels;

public sealed partial class DialogViewModel : ObservableObject
{
    public DialogViewModel(DialogContent content) => Content = content;

    public DialogContent Content { get; }

    public event EventHandler<bool>? CloseRequested;

    [RelayCommand]
    private void Confirm() => CloseRequested?.Invoke(this, true);

    [RelayCommand]
    private void Dismiss() => CloseRequested?.Invoke(this, false);
}
