using PCBManufacturing.Core.Models.Ordering;

namespace PCBManufacturing.Core.Services.Ordering;

public interface IOrderService
{
    Task<OrderConfirmation> SubmitAsync(OrderDraft draft, CancellationToken cancellationToken = default);
}
