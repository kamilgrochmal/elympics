namespace ElympicsNet.Api.DAL.Models;

internal sealed class Podcast
{
    public long Id { get; init; }
    public string Title { get; private set; }
    public string HostedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    private Podcast(string title, string hostedBy)
    {
        Title = title;
        HostedBy = hostedBy;
        CreatedAt = DateTimeOffset.Now;
    }

    public static Podcast Create(string title, string hostedBy) => new(title, hostedBy);
}