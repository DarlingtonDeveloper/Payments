using Cox.Cmr.Payment.Api.Handlers;
using Cox.Cmr.Payment.Api.Settings;
using Cox.Cmr.Payment.Api.Extensions;

namespace Cox.Cmr.Payment.Api;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost);

        var applicationSettings = builder.Configuration.GetSection("Application").Get<ApplicationSettings>()!;

        if (!builder.Environment.IsLocalOrDocker())
        {
            builder.Services.AddDataProtection()
                .PersistKeysToAWSSystemsManager(
                    $"/Cmr/{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}/PaymentApi")
                .SetApplicationName(applicationSettings.Name);
        }

        builder.Services.AddApiVersioning(options => options.ReportApiVersions = true).AddMvc();
        builder.Services.AddControllers().AddJsonSerialisation();
        builder.Services.ConfigureLogger(builder.Host, builder.Configuration);
        builder.Services.AddExceptionHandler<JsonPatchExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services
            .AddApiDependencies(builder.Configuration)
            .AddDomainDependencies(builder.Configuration)
            .AddInfrastructureDependencies(builder.Configuration);

        builder.Services.AddSwagger(builder.Configuration);

        builder.Services.AddFeatureManagement();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://*.coxautoconnect.com")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowCredentials();
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(bearerOptions =>
        {
            bearerOptions.Audience = "https://audience";
            bearerOptions.Authority = "https://authority";
            bearerOptions.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                RequireSignedTokens = false,
            };

            bearerOptions.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.NoResult();
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context => Task.CompletedTask
            };

        });

        builder.Services.RegisterApiKeys(builder.Configuration);
        builder.Services.AddAuthorization(options =>
        {
            options.AddCdpPolicy(PaymentApiPolicies.Write, [ ]);
            options.AddCdpPolicy(PaymentApiPolicies.Read, [ ]);
        });

        builder.Services.AddScoped<PrincipalContext>();

        builder.Configuration.AddUserSecrets<Program>();

        var app = builder.Build();

        app.UseForwardedHeaders();

        app.Use((context, next) =>
        {
            context.Request.Host = new HostString(applicationSettings.RequestHost);

            // Always set https once docker environment is set up to use https
            context.Request.Scheme = app.Environment.IsDocker() ? "http" : "https";
            return next();
        });

        if (!builder.Environment.IsLocalOrDocker())
        {
            app.UseHsts();
        }

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = Logger.EnrichFromRequest;
            opts.IncludeQueryInRequestPath = true;
        });

        app.UseExceptionHandler(_ => { });

        app.UseCdpAuthentication();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseSwagger(builder.Configuration);

        app.MapHealthChecks("payment/health",
            new HealthCheckOptions { ResponseWriter = HealthResponseWriter.WriteResponse }).ShortCircuit();

        app.Run();
    }
}
