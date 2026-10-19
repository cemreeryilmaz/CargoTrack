using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CargoTrack.Entity.Extensions
{
    /// <summary>
    /// Enum için [Display(Name="...")] değerini (UI'da Türkçe gösterim) döndüren yardımcı.
    /// </summary>
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (member is null)
                return value.ToString();

            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
