using PCBManufacturing.Core.Models.Theming;

namespace PCBManufacturing.Core.Services.Theming;

public interface IThemeApplier
{
    void Apply(AppTheme theme);
}
