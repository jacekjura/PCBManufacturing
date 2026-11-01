using PCBManufacturing.Core.Services.Dialogs;
using PCBManufacturing.Core.Models.Dialogs;

namespace PCBManufacturing.PresentationLogic.Tests.ViewModels;

internal sealed class FakeDialogs : IDialogService
{
    public bool ConfirmResult { get; set; } = true;
    public List<DialogContent> Confirmations { get; } = [];
    public List<DialogContent> Alerts { get; } = [];
    public Task<bool> ConfirmAsync(DialogContent content, CancellationToken cancellationToken = default)
    {
        Confirmations.Add(content);
        return Task.FromResult(ConfirmResult);
    }
    public Task AlertAsync(DialogContent content, CancellationToken cancellationToken = default)
    {
        Alerts.Add(content);
        return Task.CompletedTask;
    }
}
