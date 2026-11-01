namespace PCBManufacturing.Core.Models.Dialogs;

public sealed class DialogContent
{
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required string ConfirmText { get; init; }
    public string? CancelText { get; init; }
    public bool ShowCancel => !string.IsNullOrWhiteSpace(CancelText);
    public IReadOnlyList<DialogRow> Rows { get; init; } = [];
}
