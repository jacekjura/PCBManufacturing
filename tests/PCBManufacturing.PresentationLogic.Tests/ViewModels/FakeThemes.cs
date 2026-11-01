using PCBManufacturing.Core.Models.Theming;
using PCBManufacturing.Core.Services.Theming;

namespace PCBManufacturing.PresentationLogic.Tests.ViewModels;

internal sealed class FakeThemes : IThemeApplier
{
    public List<AppTheme> Applications { get; } = [];
    public void Apply(AppTheme theme) => Applications.Add(theme);
}
