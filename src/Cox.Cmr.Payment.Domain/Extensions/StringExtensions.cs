namespace Cox.Cmr.Payment.Domain.Extensions;

public static class StringExtensions
{
    public static int? TryParse(this string input)
    {
        if (int.TryParse(input, out var value))
        {
            return value;
        }

        return null;
    }

    public static double? TryParseDouble(this string input)
    {
        if (double.TryParse(input, out var value))
        {
            return value;
        }

        return null;
    }
}
