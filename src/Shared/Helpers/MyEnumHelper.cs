using System.ComponentModel.DataAnnotations;

namespace Helpers;

public static class MyEnumHelper
{
    public static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return ((T)attributes.FirstOrDefault()!)!;
    }

    public static string ToDisplayName(this Enum value)
    {
        var attribute = value.GetAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }

    public static TEnum? TryParseEnum<TEnum>(string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) { return null; }
        return Enum.TryParse<TEnum>(value, out var parsed) ? parsed : null;
    }

    // ✅ NEW: Convert numeric values to enum names
    public static string NormalizeEnumValue<TEnum>(string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return "";

        // If it's already a valid enum name, return it
        if (Enum.TryParse<TEnum>(value, true, out _))
        {
            return value;
        }

        // Try to parse as integer and map to enum name
        if (int.TryParse(value, out var intValue))
        {
            var enumType = typeof(TEnum);

            // Get all enum values and their underlying integer values
            var enumValues = Enum.GetValues(enumType);
            foreach (var enumValue in enumValues)
            {
                var underlyingValue = Convert.ToInt32(enumValue);
                if (underlyingValue == intValue)
                {
                    return enumValue.ToString()!;
                }
            }
        }

        return value;
    }

    public static string FormatEnum<TEnum>(string? value) where TEnum : struct, Enum
    {
        // ✅ Normalize the value first
        var normalizedValue = NormalizeEnumValue<TEnum>(value);

        // Try to parse the normalized value
        var parsed = TryParseEnum<TEnum>(normalizedValue);

        // If parsing fails, try case-insensitive
        if (parsed == null && !string.IsNullOrEmpty(normalizedValue))
        {
            if (Enum.TryParse<TEnum>(normalizedValue, true, out var caseInsensitiveParsed))
            {
                parsed = caseInsensitiveParsed;
            }
        }

        return parsed?.ToDisplayName() ?? normalizedValue ?? "";
    }

    // ✅ Helper to get enum name from numeric value for any enum type
    public static string GetEnumNameFromValue<TEnum>(string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return "";

        if (int.TryParse(value, out var intValue))
        {
            var enumType = typeof(TEnum);
            var enumValues = Enum.GetValues(enumType);
            foreach (var enumValue in enumValues)
            {
                var underlyingValue = Convert.ToInt32(enumValue);
                if (underlyingValue == intValue)
                {
                    return enumValue.ToString()!;
                }
            }
        }

        return value;
    }
}