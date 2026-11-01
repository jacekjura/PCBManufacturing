using PCBManufacturing.Core.Models.Dialogs;

namespace PCBManufacturing.Core.Services.Dialogs;

public interface IDialogService
{
    Task<bool> ConfirmAsync(DialogContent content, CancellationToken cancellationToken = default);
    Task AlertAsync(DialogContent content, CancellationToken cancellationToken = default);
}
