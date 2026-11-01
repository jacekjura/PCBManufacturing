using PCBManufacturing.Core.Models.Ordering;

namespace PCBManufacturing.Core.Services.Ordering;

public interface IOrderHistoryStore
{
    Task<IReadOnlyList<OrderConfirmation>> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(IReadOnlyList<OrderConfirmation> orders, CancellationToken cancellationToken = default);
}
