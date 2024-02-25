using System.Net;
using System.Net.Http.Json;
using ElympicsNet.Api.Services;
using FluentAssertions;

namespace ElympicsNet.Api.Tests.Integration;

public class GetPodcastsEndpointTests : IClassFixture<ElympicsNetApiFactory>
{
    private readonly HttpClient _client;

    public GetPodcastsEndpointTests(ElympicsNetApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPodcasts_Should_ReturnAllProcessedPodcasts_When_ExternalApiWorksFine()
    {
        //Act
        var response = await _client.GetAsync("/podcasts");
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var podcastResponse = await response.Content.ReadFromJsonAsync<IEnumerable<PodcastDto>>();
        var titles = podcastResponse.Select(a => a.Title);
        titles.Should().Contain(title => title.Contains("WireMock"));

    }
    
}