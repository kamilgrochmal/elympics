using ElympicsNet.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace ElympicsNet.Api.DAL;

internal sealed class DatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Podcasts.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var podcasts = new List<Podcast>
        {
            Podcast.Create("Zen Jaskiniowca", "Rafał Mazur"),
            Podcast.Create("Musisz wiedzieć", "N/A")
        };

        await dbContext.Podcasts.AddRangeAsync(podcasts, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}