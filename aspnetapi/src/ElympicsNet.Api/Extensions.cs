using System.Runtime.CompilerServices;
using ElympicsNet.Api.DAL;
using ElympicsNet.Api.Exceptions;
using ElympicsNet.Api.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
[assembly: InternalsVisibleTo("ElympicsNet.Api.Tests.Integration")]
namespace ElympicsNet.Api;

internal static class Extensions
{
    private const string PostgresSectionName = "postgres";
    private const string GoWebApiSectionName = "gowebapi";
    private const string EntrySectionName = "Entry";

    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();
        services.AddPostgres(configuration);
        services.AddGoWebApi(configuration);
        services.AddSerilog();
        services.AddTransient<IPodcastsService, PodcastsService>();
        services.AddSingleton<ExceptionMiddleware>();

        
        services.Configure<EntrySettings>(configuration.GetRequiredSection(EntrySectionName));
        configuration.GetOptions<EntrySettings>(EntrySectionName);
        
        services.AddHostedService<DatabaseInitializer>();

    }

    private static void AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(configuration.GetRequiredSection(PostgresSectionName));
        var postgresOptions = configuration.GetOptions<PostgresOptions>(PostgresSectionName);
        services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(postgresOptions.ConnectionString));
        
        // EF Core + Npgsql issue
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    private static void AddGoWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoWebApiSettings>(configuration.GetRequiredSection(GoWebApiSectionName));
        var gowebapiOptions = configuration.GetOptions<GoWebApiSettings>(GoWebApiSectionName);
        
        services.AddHttpClient(GoWebApiSectionName, httpClient =>
        {
            httpClient.BaseAddress = new Uri(gowebapiOptions.Url);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            };
        })
        .AddPolicyHandler(GetRetryPolicy());
    }

    private static void AddSerilog(this IServiceCollection services)
    {
        Serilog.ILogger logger = new LoggerConfiguration()
            .WriteTo.Console(theme:AnsiConsoleTheme.Code)
            .CreateLogger();

        Log.Logger = logger;

        services.AddSingleton(logger);
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
    
    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);

        return options;
    }
}