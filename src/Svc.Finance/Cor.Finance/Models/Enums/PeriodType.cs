// Models/Enums/PeriodType.cs
namespace Cor.Finance.Models.Enums
{

public static class Currency
{
    public const string USD = "USD";
    public const string EUR = "EUR";
    public const string GBP = "GBP";
    public const string JPY = "JPY";
    public const string CNY = "CNY";
    public const string ETB = "ETB";
    public const string KES = "KES";
    public const string TZS = "TZS";
    public const string UGX = "UGX";
    public const string RWF = "RWF";
    public const string ZAR = "ZAR";
    public const string NGN = "NGN";
    public const string GHS = "GHS";
    public const string AED = "AED";
    public const string SAR = "SAR";

    public static List<string> GetSupportedCurrencies()
    {
        return new List<string>
        {
            USD, EUR, GBP, JPY, CNY, ETB, KES, TZS, UGX, RWF,
            ZAR, NGN, GHS, AED, SAR
        };
    }

    public static bool IsValid(string currency)
    {
        return GetSupportedCurrencies().Contains(currency);
    }

    }
    public enum PeriodType
    {
        MONTHLY = 1,
        QUARTERLY = 2,
        YEARLY = 3,
        CUSTOM = 4
    }

    public enum PeriodStatus
        {
            DRAFT=0,
            OPEN = 1,
            CLOSED = 2,
            PENDING= 3,
            ARCHIVED = 4,
            LOCKED=5

        }

        public enum AuditStatus
            {
                Success = 1,
                Failed = 2,
                Pending = 3,
                Error=4,
                Unauthorized=5


            }

            public enum InvoiceType
            {
                Sales = 1,
                Purchase = 2
            }

            // Extension methods for InvoiceType
            public static class InvoiceTypeExtensions
            {
                public static string ToStringValue(this InvoiceType type)
                {
                    return type switch
                    {
                        InvoiceType.Sales => "Sales",
                        InvoiceType.Purchase => "Purchase",
                        _ => "Unknown"
                    };
                }

                public static InvoiceType FromString(string value)
                {
                    return value?.ToLower() switch
                    {
                        "sales" => InvoiceType.Sales,
                        "purchase" => InvoiceType.Purchase,
                        _ => InvoiceType.Sales
                    };
                }
                }
}