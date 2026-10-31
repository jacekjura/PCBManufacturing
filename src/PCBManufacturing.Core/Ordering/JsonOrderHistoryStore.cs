using System.Text.Json;
using PCBManufacturing.Core.Json;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Ordering;

public interface IOrderHistoryStore
{
    Task<IReadOnlyList<OrderConfirmation>> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(IReadOnlyList<OrderConfirmation> orders, CancellationToken cancellationToken = default);
}

public sealed class JsonOrderHistoryStore : IOrderHistoryStore
{
    public const int MaxOrders = 50;
    private readonly string _path;

    public JsonOrderHistoryStore(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Order history path is required.", nameof(path));
        _path = path;
    }

    public async Task<IReadOnlyList<OrderConfirmation>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_path))
            return [];

        try
        {
            await using var stream = File.OpenRead(_path);
            var orders = await JsonSerializer.DeserializeAsync<List<OrderConfirmation>>(stream, JsonDefaults.Options, cancellationToken).ConfigureAwait(false);
            return orders ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    public async Task SaveAsync(IReadOnlyList<OrderConfirmation> orders, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orders);
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var temporary = _path + ".tmp";
        await using (var stream = File.Create(temporary))
        {
            await JsonSerializer.SerializeAsync(stream, orders.Take(MaxOrders).ToList(), JsonDefaults.Options, cancellationToken).ConfigureAwait(false);
        }

        File.Move(temporary, _path, overwrite: true);
    }
}
