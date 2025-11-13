using System.ComponentModel.DataAnnotations;

namespace Leave.Domain.Enums;

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
}
