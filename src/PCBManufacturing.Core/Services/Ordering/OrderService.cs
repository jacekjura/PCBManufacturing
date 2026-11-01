using PCBManufacturing.Core.Models.Ordering;

namespace PCBManufacturing.Core.Services.Ordering;

public sealed class OrderService : IOrderService
{
    private readonly IOrderNumberFactory _numbers;
    private readonly TimeProvider _time;
    private readonly TimeSpan _delay;
    public OrderService(IOrderNumberFactory numbers, TimeProvider time, TimeSpan? delay = null)
    {
        _numbers = numbers;
        _time = time;
        _delay = delay ?? TimeSpan.FromMilliseconds(700);
    }
    public async Task<OrderConfirmation> SubmitAsync(OrderDraft draft, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        if (_delay > TimeSpan.Zero)
            await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
        var created = _time.GetUtcNow();
        return new OrderConfirmation
        {
            Number = _numbers.Create(created),
            CreatedAt = created,
            ProjectName = draft.ProjectName,
            Postcode = draft.Postcode,
            Notes = draft.Notes,
            MaterialKey = draft.MaterialKey,
            MaskKey = draft.MaskKey,
            MaskHex = draft.MaskHex,
            ThicknessKey = draft.ThicknessKey,
            SizeKey = draft.SizeKey,
            FinishKey = draft.FinishKey,
            Layers = draft.Layers,
            Quantity = draft.Quantity,
            Total = draft.Total,
            Currency = draft.Currency,
            ProductionDays = draft.ProductionDays
        };
    }
}
