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

    public long Views { get; set; }

    public long Likes { get; set; }

    public long Comments { get; set; }

    public double AgeHours { get; set; }

    public double ViewsPerHour { get; set; }

    public bool IsRelevant { get; set; }

    public bool IsShort { get; set; }

    public int RelevanceScore { get; set; }
}

public class YouTubeStatisticsResponse
{
    public List<YouTubeStatisticsItem> Items { get; set; } = [];
}

public class YouTubeStatisticsItem
{
    public string Id { get; set; } = string.Empty;

    public YouTubeStatistics Statistics { get; set; } = new();
}

public class YouTubeStatistics
{
    public string ViewCount { get; set; } = "0";

    public string LikeCount { get; set; } = "0";

    public string CommentCount { get; set; } = "0";
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