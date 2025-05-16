namespace Cox.Cmr.Payment.Api.Attributes;

/// <summary>
/// Attribute to explicitly include endpoints in automatically generated documentation.
/// Allows Opting in for safety and security.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IncludeInDocumentationAttribute : Attribute
{
}
