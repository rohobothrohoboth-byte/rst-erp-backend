using System.ComponentModel.DataAnnotations;

namespace Helpers;

public static class EnumNormalizer
{
    // Mapping of enum types to their numeric-to-name mappings
    private static readonly Dictionary<Type, Dictionary<string, string>> _enumMappings = new();

    static EnumNormalizer()
    {
        // EmpState mappings
        _enumMappings[typeof(EmpState)] = new Dictionary<string, string>
        {
            { "0", "Pen" },
            { "1", "Active" },
            { "2", "Leave" },
            { "3", "Sus" },
            { "4", "Term" },
            { "5", "Retire" },
            { "6", "StandBy" },
            { "7", "Rej" }
        };

        // EmpType mappings
        _enumMappings[typeof(EmpType)] = new Dictionary<string, string>
        {
            { "0", "Rep" },
            { "1", "NewOp" },
            { "2", "AddReq" },
            { "3", "Old" }
        };

        // EmpNature mappings
        _enumMappings[typeof(EmpNature)] = new Dictionary<string, string>
        {
            { "0", "Per" },
            { "1", "Con" },
            { "2", "Pro" },
            { "3", "Inte" },
            { "4", "Par" }
        };

        // WorkArrangement mappings
        _enumMappings[typeof(WorkArrangement)] = new Dictionary<string, string>
        {
            { "0", "OnSite" },
            { "1", "Remote" },
            { "2", "Hybrid" },
            { "3", "ShiftB" },
            { "4", "Rota" }
        };
    }

    public static string NormalizeEnumValue<TEnum>(string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return "";

        var enumType = typeof(TEnum);

        // Check if we have a mapping for this enum type
        if (_enumMappings.TryGetValue(enumType, out var mapping))
        {
            if (mapping.TryGetValue(value, out var mappedValue))
            {
                return mappedValue;
            }
        }

        // If it's already a valid enum name, return it
        if (Enum.TryParse<TEnum>(value, true, out _))
        {
            return value;
        }

        // Try to parse as integer and map to enum name
        if (int.TryParse(value, out var intValue))
        {
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

    public static string GetDisplayName<TEnum>(string? value) where TEnum : struct, Enum
    {
        var normalizedValue = NormalizeEnumValue<TEnum>(value);

        if (string.IsNullOrEmpty(normalizedValue)) return "";

        if (Enum.TryParse<TEnum>(normalizedValue, true, out var parsed))
        {
            return parsed.ToDisplayName();
        }

        return normalizedValue;
    }
}