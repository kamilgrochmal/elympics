using ElympicsNet.Api.DAL;
using ElympicsNet.Api.DAL.Models;
using ElympicsNet.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ElympicsNet.Api.Services;


internal sealed class PodcastsService : IPodcastsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly EntrySettings _entrySettings;

    public PodcastsService(
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext dbContext,
        IOptionsSnapshot<EntrySettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _entrySettings = options.Value;
    }

    public async Task<IReadOnlyList<PodcastDto>> ProcessPodcast(CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("gowebapi");
        var response = await client.GetAsync("/podcast", ct);
        var podcastDto = await response.Content.ReadFromJsonAsync<PodcastDto>(ct);

        ValidateInput(podcastDto);
        
        await _dbContext.Podcasts.AddAsync(Podcast.Create(podcastDto!.Title!, podcastDto!.HostedBy!), ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return await _dbContext
            .Podcasts
            .OrderByDescending(a => a.CreatedAt)
            .Take(_entrySettings.MaxFetchedAmount)
            .Select(a => new PodcastDto(a.Title, a.HostedBy))
            .ToListAsync(ct);
    }

    private void ValidateInput(PodcastDto? podcastDto)
    {
        
        if (podcastDto is null)
        {
            throw new RetrievedPodcastIsNullException();
        }

        if (string.IsNullOrEmpty(podcastDto.Title) || string.IsNullOrEmpty(podcastDto.HostedBy))
        {
            throw new PodcastMissingDataException(podcastDto);
        }
    }
    
    
}

public interface IPodcastsService
{
    Task<IReadOnlyList<PodcastDto>> ProcessPodcast(CancellationToken ct);
}

public record PodcastDto(string? Title, string? HostedBy);

public class PodcastMissingDataException(PodcastDto podcastDto)
    : ElympicsNetException($"Retrieved podcast has missing data.{@podcastDto}")
{
    public PodcastDto PodcastDto { get; } = podcastDto;
}

public class RetrievedPodcastIsNullException() : ElympicsNetException($"Retrieved podcast is null.");