namespace Cox.Cmr.Payment.Domain.Helpers;

public  class ValidationHelper
{
    public static bool IsValidGuid(string? id)
    {
        var result = Guid.TryParse(id, out Guid idGuid);
        return result && idGuid != Guid.Empty;
    }
}
