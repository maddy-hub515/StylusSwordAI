using ResearchAgent.Models;

namespace ResearchAgent.Services;

public class YouTubeRelevanceService
{
    private readonly IConfiguration _configuration;

    private readonly HashSet<string> _irrelevantKeywords =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "fortnite",
            "gaming",
            "gameplay",
            "fifa",
            "minecraft",
            "roblox",
            "gta",
            "free fire",
            "pubg",
            "lays",
            "dance",
            "music",
            "song",
            "reaction",
            "meme",
            "fyp",
            "viral",
            "challenge",
            "prank"
        };

    private readonly HashSet<string> _historicalKeywords =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "ancient",
            "history",
            "historical",
            "archaeology",
            "archaeological",
            "civilization",
            "civilisation",
            "empire",
            "kingdom",
            "dynasty",
            "temple",
            "ruins",
            "artifact",
            "artefact",
            "excavation",
            "discovery",
            "archaeologist",
            "mystery",
            "mysterious",
            "lost",
            "forgotten",
            "tomb",
            "pyramid",
            "pharaoh",
            "roman",
            "rome",
            "greek",
            "greece",
            "egypt",
            "mesopotamia",
            "babylon",
            "inca",
            "maya",
            "aztec",
            "persian",
            "japan",
            "china",
            "sumerian",
            "sumer",
            "technology",
            "invention",
            "engineering"
        };

    public YouTubeRelevanceService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Analyze(YouTubeVideo video)
    {
        if (video == null)
        {
            return;
        }

        var title = video.Title ?? string.Empty;

        var score = CalculateRelevanceScore(title);

        video.RelevanceScore = score;

        video.IsShort = IsShort(video);

        video.IsRelevant = score >= 20;
    }

    public void Analyze(IEnumerable<YouTubeVideo> videos)
    {
        foreach (var video in videos)
        {
            Analyze(video);
        }
    }

    private int CalculateRelevanceScore(string title)
    {
        var normalizedTitle = title.ToLowerInvariant();

        // -----------------------------------------
        // STEP 1: Check clearly irrelevant content
        // -----------------------------------------

        foreach (var keyword in _irrelevantKeywords)
        {
            if (ContainsKeyword(normalizedTitle, keyword))
            {
                return 0;
            }
        }

        var score = 0;

        // -----------------------------------------
        // STEP 2: Historical relevance
        // -----------------------------------------

        foreach (var keyword in _historicalKeywords)
        {
            if (ContainsKeyword(normalizedTitle, keyword))
            {
                score += GetKeywordScore(keyword);
            }
        }

        // -----------------------------------------
        // STEP 3: Channel niche relevance
        // -----------------------------------------

        var channelConfig =
    _configuration.Get<ChannelConfig>();

        if (channelConfig != null)
        {
            foreach (var niche in channelConfig.Niche)
            {
                if (ContainsKeyword(normalizedTitle, niche))
                {
                    score += 10;
                }
            }
        }

        // Don't allow score to exceed 100
        return Math.Min(score, 100);
    }

    private int GetKeywordScore(string keyword)
    {
        return keyword.ToLowerInvariant() switch
        {
            "archaeology" => 20,
            "archaeological" => 20,
            "civilization" => 15,
            "civilisation" => 15,
            "artifact" => 15,
            "artefact" => 15,
            "ruins" => 15,
            "excavation" => 20,
            "lost" => 10,
            "mystery" => 10,
            "mysterious" => 10,
            "ancient" => 10,
            "historical" => 10,
            "history" => 10,
            "empire" => 10,
            "dynasty" => 10,
            "temple" => 10,
            "tomb" => 10,
            "pyramid" => 15,
            "pharaoh" => 15,
            "technology" => 10,
            "engineering" => 10,
            "invention" => 10,

            _ => 5
        };
    }

    private bool IsShort(YouTubeVideo video)
    {
        var title = video.Title ?? string.Empty;

        // Most YouTube Shorts use #shorts.
        if (title.Contains("#shorts", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (title.Contains("#short", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private bool ContainsKeyword(string text, string keyword)
    {
        return text.Contains(
            keyword,
            StringComparison.OrdinalIgnoreCase);
    }
}