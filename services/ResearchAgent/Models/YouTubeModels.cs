namespace ResearchAgent.Models;

public class YouTubeSearchResult
{
    public string Query { get; set; } = string.Empty;

    public List<YouTubeVideo> Results { get; set; } = [];
}

public class YouTubeVideo
{
    public string VideoId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Channel { get; set; } = string.Empty;

    public string PublishedAt { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}

public class YouTubeSearchResponse
{
    public List<YouTubeSearchItem> Items { get; set; } = [];
}

public class YouTubeSearchItem
{
    public YouTubeVideoId Id { get; set; } = new();

    public YouTubeVideoSnippet Snippet { get; set; } = new();
}

public class YouTubeVideoId
{
    public string VideoId { get; set; } = string.Empty;
}

public class YouTubeVideoSnippet
{
    public string PublishedAt { get; set; } = string.Empty;

    public string ChannelId { get; set; } = string.Empty;

    public string ChannelTitle { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}