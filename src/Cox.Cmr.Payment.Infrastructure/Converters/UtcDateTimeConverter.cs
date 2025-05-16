namespace Cox.Cmr.Payment.Infrastructure.Converters;

public class UtcDateTimeConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        if (value == null)
        {
            return new DynamoDBNull();
        }

        var dateTime = (DateTime)value;

        // ensure datetime is in UTC before saving
        var utcDateTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
        return utcDateTime.ToString("o");
    }

    public object? FromEntry(DynamoDBEntry entry)
    {
        if (entry is DynamoDBNull)
        {
            return null;
        }

        // get stored string and convert back to UTC datetime
        var dateTime = DateTime.Parse(entry.AsString()).ToUniversalTime();

        return dateTime;
    }
}
