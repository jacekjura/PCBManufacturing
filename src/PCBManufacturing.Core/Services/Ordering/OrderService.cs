using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Ordering;

public interface IOrderNumberFactory
{
    string Create(DateTimeOffset timestamp);
}

public sealed class OrderNumberFactory : IOrderNumberFactory
{
    public string Create(DateTimeOffset timestamp) =>
        $"PCB-{timestamp:yyyyMMdd}-{Random.Shared.Next(1000, 10000)}";
}

public interface IOrderService
{
    Task<OrderConfirmation> SubmitAsync(OrderDraft draft, CancellationToken cancellationToken = default);
}

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
