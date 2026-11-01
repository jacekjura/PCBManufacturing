using PCBManufacturing.Core.Models.Catalog;
using PCBManufacturing.Core.Models.Theming;

namespace PCBManufacturing.PresentationLogic.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public async Task SaveAndReload_RestoresThemeLanguageAndSelections()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString());
        var first = ViewModelGraph.Create(directory);
        first.Preferences.Postcode = "EC1A 1BB";
        first.Preferences.ProjectName = "Sensor";
        first.Preferences.SelectedMaterial = first.Preferences.Materials.Single(option => option.Id == "aluminum");
        first.Main.ToggleThemeCommand.Execute(null);
        first.Main.IsRussian = true;
        first.Quote.Groups.Single(group => group.Kind == ParameterKind.Finish).SelectedRow =
            first.Quote.Groups.Single(group => group.Kind == ParameterKind.Finish).Rows.Single(row => row.Definition.Id == "finish-enig");

        await first.Main.SaveCommand.ExecuteAsync(null);

        var second = ViewModelGraph.Create(directory);
        await second.Main.InitializeAsync();

        Assert.Equal("Sensor", second.Preferences.ProjectName);
        Assert.Equal("EC1A 1BB", second.Preferences.Postcode);
        Assert.Equal("aluminum", second.Preferences.SelectedMaterial?.Id);
        Assert.Equal("finish-enig", second.Quote.SelectedFinish?.Id);
        Assert.Equal(AppTheme.Dark, second.Main.Theme);
        Assert.Equal("ru", second.Main.Culture);
        Assert.Contains(AppTheme.Dark, second.Themes.Applications);
    }

    [Fact]
    public void Discard_RestoresCatalogDefaultsWithoutChangingTheme()
    {
        var graph = ViewModelGraph.Create();
        graph.Main.ToggleThemeCommand.Execute(null);
        graph.Preferences.Postcode = "94105";
        graph.Preferences.ProjectName = "Temp";
        graph.Quote.Select("size-50", "layers-8", "finish-osp");

        graph.Main.DiscardCommand.Execute(null);

        Assert.Equal("AC-9001", graph.Preferences.ProjectName);
        Assert.Equal("", graph.Preferences.Postcode);
        Assert.Equal("fr4", graph.Preferences.SelectedMaterial?.Id);
        Assert.Equal("size-100", graph.Quote.SelectedSize?.Id);
        Assert.Equal(AppTheme.Dark, graph.Main.Theme);
    }

    [Fact]
    public async Task Continue_OpensTheQuoteSection()
    {
        var graph = ViewModelGraph.Create();
        Assert.False(graph.Main.IsPreferencesExpanded);
        Assert.True(graph.Main.IsQuoteExpanded);
        Assert.True(graph.Main.IsOrdersExpanded);

        graph.Main.IsQuoteExpanded = false;
        await graph.Main.ContinueCommand.ExecuteAsync(null);

        Assert.True(graph.Main.IsQuoteExpanded);
    }
}
