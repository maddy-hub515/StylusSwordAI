namespace ResearchAgent.Models;

public class ChannelConfig
{
    public string ChannelName { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string TargetAudience { get; set; } = string.Empty;

    public List<string> Niche { get; set; } = [];

    public List<string> ContentTypes { get; set; } = [];

    public DocumentaryDuration DocumentaryDurationMinutes { get; set; } = new();

    public ShortDuration ShortDurationSeconds { get; set; } = new();

    public List<string> Style { get; set; } = [];
}

public class DocumentaryDuration
{
    public int Minimum { get; set; }

    public int Maximum { get; set; }
}

public class ShortDuration
{
    public int Minimum { get; set; }

    public int Maximum { get; set; }
}