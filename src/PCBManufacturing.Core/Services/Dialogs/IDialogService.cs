using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Dialogs;

public interface IDialogService
{
    Task<bool> ConfirmAsync(DialogContent content, CancellationToken cancellationToken = default);
    Task AlertAsync(DialogContent content, CancellationToken cancellationToken = default);
}
