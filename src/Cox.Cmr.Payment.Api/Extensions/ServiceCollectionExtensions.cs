using System.Reflection;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Cox.Cdp.Common.Providers;
using Cox.Cmr.Payment.Domain.Repositories;
using Cox.Cmr.Payment.Domain.Services;
using Cox.Cmr.Payment.Infrastructure.Repositories;
using EventClient.Metrics;
using ApplicationSettings = Amazon.Util.Internal.PlatformServices.ApplicationSettings;
using Cox.Cmr.Payment.Infrastructure.Converters;
using Cox.Cmr.Payment.Infrastructure.Repositories.Search;
using Cox.Cmr.Payment.Domain.Validators;

namespace Cox.Cmr.Payment.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .ConfigureHealthChecks()
            .Configure<ApplicationSettings>(configuration.GetSection("Application"))
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
            .AddTransient<IMetricsRegistry, MetricsRegistry>()
            .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies().Append(Assembly.Load("Cox.Cmr.Payment.Infrastructure")));

        return services;
    }

    public static IServiceCollection AddDomainDependencies(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();

        // validators
        services.AddTransient<IPaymentMethodValidator, PaymentMethodValidator>();

        return services;
    }

    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services,
        ConfigurationManager configuration)
    {

        // Clients
        services.RegisterClients(configuration);

        // AWS
        services.RegisterAwsServices(configuration);

        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IPaymentMethodSearchRepository, PaymentMethodSearchRepository>();        

        return services;
    }

    private static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<BasicHealthCheck>("Basic");

        return services;
    }

    private static IServiceCollection RegisterClients(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        return services;
    }

    private static IServiceCollection RegisterAwsServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        var awsConfiguration = configuration.GetSection("AWS");

        services.AddDynamoDbContext(awsConfiguration);

        return services;
    }

    private static IServiceCollection AddDynamoDbContext(this IServiceCollection services,
     IConfigurationSection awsConfiguration)
    {
        var dynamoDbConfig = awsConfiguration.GetSection("DynamoDb");
        var clientConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = dynamoDbConfig.GetValue<string>("ServiceURL"),
            AuthenticationRegion = awsConfiguration.GetValue<string>("Region"),
            RetryMode = RequestRetryMode.Standard,
            MaxErrorRetry = dynamoDbConfig.GetValue<int>("MaxErrorRetry"),
            Timeout = TimeSpan.FromMilliseconds(dynamoDbConfig.GetValue<int>("TimeoutInMs"))
        };
        var client = new AmazonDynamoDBClient(clientConfig);
        var contextConfig = new DynamoDBContextConfig
        {
            TableNamePrefix = dynamoDbConfig.GetValue<string>("TableNamePrefix")
        };
        var dynamodbContext = new DynamoDBContext(client, contextConfig);
        dynamodbContext.ConverterCache.Add(typeof(DateTime?), new UtcDateTimeConverter());
        dynamodbContext.ConverterCache.Add(typeof(DateTime), new UtcDateTimeConverter());
        services.AddSingleton<IDynamoDBContext>(dynamodbContext);

        return services;
    }

}
