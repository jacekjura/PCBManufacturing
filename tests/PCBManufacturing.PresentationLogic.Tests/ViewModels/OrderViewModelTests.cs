namespace PCBManufacturing.ViewModels.Tests;

public class OrderViewModelTests
{
    [Fact]
    public async Task InvalidPostcode_OpensTheValidationDialogAndDoesNotSubmit()
    {
        var graph = ViewModelGraph.Create();
        graph.Preferences.Postcode = "simas";

        await graph.Order.PlaceOrderCommand.ExecuteAsync(null);

        Assert.Empty(graph.Orders.Submitted);
        Assert.NotEmpty(graph.Dialogs.Alerts);
        Assert.Empty(graph.Dialogs.Confirmations);
        Assert.Contains(graph.Order.ValidationMessages, message => message.Contains("postcode", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ConfirmingAValidOrder_SubmitsAndRecordsHistory()
    {
        var graph = ViewModelGraph.Create();
        graph.Preferences.Postcode = "94105";

        await graph.Order.PlaceOrderCommand.ExecuteAsync(null);

        Assert.Single(graph.Dialogs.Confirmations);
        Assert.Single(graph.Orders.Submitted);
        Assert.Single(graph.Order.History);
        Assert.Contains("PCB-TEST", graph.Order.StatusMessage);
        var stored = await graph.History.LoadAsync();
        Assert.Single(stored);
    }

    [Fact]
    public async Task DecliningTheDialog_DoesNotSubmit()
    {
        var graph = ViewModelGraph.Create();
        graph.Preferences.Postcode = "101000";
        graph.Dialogs.ConfirmResult = false;

        await graph.Order.PlaceOrderCommand.ExecuteAsync(null);

        Assert.Empty(graph.Orders.Submitted);
        Assert.Empty(graph.Order.History);
    }

    [Fact]
    public async Task PlaceOrderCommand_IsAsyncAndBusyStateRejectsASecondSubmit()
    {
        var graph = ViewModelGraph.Create();
        graph.Preferences.Postcode = "94105";
        graph.Orders.Delay = TimeSpan.FromMilliseconds(200);

        var running = graph.Order.PlaceOrderCommand.ExecuteAsync(null);
        Assert.True(graph.Order.IsBusy);
        Assert.False(graph.Order.PlaceOrderCommand.CanExecute(null));
        await running;
        Assert.False(graph.Order.IsBusy);
        Assert.True(graph.Order.PlaceOrderCommand.CanExecute(null));
    }
}
