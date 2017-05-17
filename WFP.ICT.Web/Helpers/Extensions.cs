using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static class Extensions
{
    public static string ToJson(this object value)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented);
    }

    public static string ToCapitalized(this string value)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Replace(".",""));
    }

    static Regex r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

    public static string ToCapitalLetterString(this string s)
    {
        return r.Replace(s, " ");
    }

    public static string ToSafeString(this Guid? value)
    {
        return value.HasValue ? value.Value.ToString() : string.Empty;
    }

    public static string ToSafeString(this DateTime? value, string format)
    {
        return value.HasValue ? value.Value.ToString(format) : string.Empty;
    }
}