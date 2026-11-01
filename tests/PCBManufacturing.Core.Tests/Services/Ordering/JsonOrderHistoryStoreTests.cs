using PCBManufacturing.Core.Models.Ordering;
using PCBManufacturing.Core.Services.Ordering;

namespace PCBManufacturing.Core.Tests.Services.Ordering;

public class JsonOrderHistoryStoreTests
{
    [Fact]
    public async Task History_RoundTripsAndCapsTheFile()
    {
        var path = Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString(), "orders.json");
        var store = new JsonOrderHistoryStore(path);
        var orders = Enumerable.Range(0, 60).Select(index => new OrderConfirmation { Number = "PCB-" + index, Total = index }).ToList();

        await store.SaveAsync(orders);
        var loaded = await store.LoadAsync();

        Assert.Equal(JsonOrderHistoryStore.MaxOrders, loaded.Count);
        Assert.Equal("PCB-0", loaded[0].Number);
    }
}
