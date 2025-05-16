using Asp.Versioning;
using Cox.Cdp.Api.Swagger.Filters;
using Cox.Cmr.Payment.Api.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cox.Cmr.Payment.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class SwaggerRegistrationExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, ConfigurationManager configuration)
    {
        var swaggerConfiguration = configuration.GetSection("Swagger");

        if (swaggerConfiguration.GetValue<bool>("Generate"))
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
                options.SwaggerDoc("Payment_v1", new OpenApiInfo { Title = "Payment Api v1", Version = "v1" });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var isIncluded = apiDesc.CustomAttributes().OfType<IncludeInDocumentationAttribute>().Any();
                    var docNameVersion = docName.Split("_")[1];

                    var versionDoesMatchDocName = apiDesc.CustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).Any(v => $"v{v}" == $"{docNameVersion}.0");

                    return isIncluded && versionDoesMatchDocName;
                });

                options.DocumentFilter<ReplaceVersionInPathFilter>();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.UseOneOfForPolymorphism();
            });
        }

        return services;
    }

    public static IApplicationBuilder UseSwagger(
        this IApplicationBuilder app,
        ConfigurationManager configuration)
    {
        var swaggerConfiguration = configuration.GetSection("Swagger");
        if (swaggerConfiguration.GetValue<bool>("Host"))
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/Payment_v1/swagger.json", "Payment Api v1");
            });
        }

        return app;
    }
}
