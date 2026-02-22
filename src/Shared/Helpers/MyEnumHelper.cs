using System.ComponentModel.DataAnnotations;

namespace Helpers;

public static class MyEnumHelper
{
    public static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return ((T)attributes.FirstOrDefault()!)!;//attributes.Length > 0 ? (T)attributes[0] : null;
    }

    public static string ToDisplayName(this Enum value)
    {
        var attribute = value.GetAttribute<DisplayAttribute>();
        return attribute.Name ?? value.ToString();
    }

    public static TEnum? TryParseEnum<TEnum>(string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) { return null; }
        return Enum.TryParse<TEnum>(value, out var parsed) ? parsed : null;
    }
}
