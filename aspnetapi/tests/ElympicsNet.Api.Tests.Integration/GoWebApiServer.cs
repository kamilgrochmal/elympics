using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ElympicsNet.Api.Tests.Integration;

internal sealed class GoWebApiServer : IDisposable
{
    private WireMockServer _server = null!;
    public string Url => _server.Url!;
    public string GoWebApiSectionName => "gowebapi";

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupPodcast(string? title, string? hostedBy)
    {
        _server.Given(Request.Create()
                .WithPath("/podcast")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(GeneratePodcastResponseBody(title, hostedBy))
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(200));
    }

    private string GeneratePodcastResponseBody(string? title, string? hostedBy)
    {
        return $@"{{
              ""title"": ""{title}"",
              ""hostedBy"": ""{hostedBy}""  
                }}";
    }
    

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}