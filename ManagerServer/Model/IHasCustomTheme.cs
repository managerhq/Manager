using System;

namespace ManagerServer.Model
{
    public interface IHasCustomTheme
    {
        bool CustomTheme { get; set; }
        Guid? CustomThemeId { get; set; }

        Guid? GetCustomTheme()
        {
            if (!CustomTheme) return null;
            return CustomThemeId;
        }
    }
}
