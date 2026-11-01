using PCBManufacturing.Core.Models.Ordering;
using PCBManufacturing.Core.Services.Ordering;

namespace PCBManufacturing.PresentationLogic.Tests.ViewModels;

internal sealed class FakeOrders : IOrderService
{
    public List<OrderDraft> Submitted { get; } = [];
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;
    public async Task<OrderConfirmation> SubmitAsync(OrderDraft draft, CancellationToken cancellationToken = default)
    {
        Submitted.Add(draft);
        if (Delay > TimeSpan.Zero)
            await Task.Delay(Delay, cancellationToken);
        return new OrderConfirmation
        {
            Number = "PCB-TEST",
            CreatedAt = DateTimeOffset.UnixEpoch,
            ProjectName = draft.ProjectName,
            Postcode = draft.Postcode,
            Total = draft.Total,
            Currency = draft.Currency,
            Quantity = draft.Quantity,
            ProductionDays = draft.ProductionDays,
            MaterialKey = draft.MaterialKey,
            MaskKey = draft.MaskKey,
            MaskHex = draft.MaskHex,
            ThicknessKey = draft.ThicknessKey,
            SizeKey = draft.SizeKey,
            FinishKey = draft.FinishKey,
            Layers = draft.Layers
        };
    }
}
