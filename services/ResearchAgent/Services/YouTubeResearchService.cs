using System.Text.Json;
using ResearchAgent.Models;

namespace ResearchAgent.Services;

public class YouTubeResearchService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public YouTubeResearchService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<YouTubeSearchResult> SearchAsync(
    string query,
    int maxResults = 10)
    {
        var apiKey = _configuration["YouTube:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "YouTube API key is not configured.");
        }

        // -----------------------------------------
        // STEP 1: Search YouTube videos
        // -----------------------------------------

        var searchUrl =
            "https://www.googleapis.com/youtube/v3/search" +
            $"?part=snippet" +
            $"&q={Uri.EscapeDataString(query)}" +
            $"&type=video" +
            $"&order=date" +
            $"&maxResults={maxResults}" +
            $"&relevanceLanguage=en" +
            $"&key={apiKey}";

        var searchResponse = await _httpClient.GetAsync(searchUrl);

        if (!searchResponse.IsSuccessStatusCode)
        {
            var error = await searchResponse.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"YouTube Search API request failed. " +
                $"Status: {searchResponse.StatusCode}. " +
                $"Response: {error}");
        }

        var searchJson = await searchResponse.Content.ReadAsStringAsync();

        var searchResult =
            JsonSerializer.Deserialize<YouTubeSearchResponse>(
                searchJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (searchResult == null || searchResult.Items.Count == 0)
        {
            return new YouTubeSearchResult
            {
                Query = query
            };
        }

        // -----------------------------------------
        // STEP 2: Convert search response
        // into our YouTubeVideo model
        // -----------------------------------------

        var videos = searchResult.Items
            .Where(item => !string.IsNullOrWhiteSpace(item.Id.VideoId))
            .Select(item => new YouTubeVideo
            {
                VideoId = item.Id.VideoId,
                Title = item.Snippet.Title,
                Channel = item.Snippet.ChannelTitle,
                PublishedAt = item.Snippet.PublishedAt,
                Url = $"https://www.youtube.com/watch?v={item.Id.VideoId}"
            })
            .ToList();

        // -----------------------------------------
        // STEP 3: Get video IDs
        // -----------------------------------------

        var videoIds = string.Join(
            ",",
            videos.Select(video => video.VideoId));

        // -----------------------------------------
        // STEP 4: Get statistics
        // -----------------------------------------

        var statisticsUrl =
            "https://www.googleapis.com/youtube/v3/videos" +
            $"?part=statistics" +
            $"&id={videoIds}" +
            $"&key={apiKey}";

        var statisticsResponse =
            await _httpClient.GetAsync(statisticsUrl);

        if (!statisticsResponse.IsSuccessStatusCode)
        {
            var error =
                await statisticsResponse.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"YouTube Statistics API request failed. " +
                $"Status: {statisticsResponse.StatusCode}. " +
                $"Response: {error}");
        }

        var statisticsJson =
            await statisticsResponse.Content.ReadAsStringAsync();

        var statisticsResult =
            JsonSerializer.Deserialize<YouTubeStatisticsResponse>(
                statisticsJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        // -----------------------------------------
        // STEP 5: Merge statistics with videos
        // -----------------------------------------

        if (statisticsResult != null)
        {
            foreach (var video in videos)
            {
                var statistics =
                    statisticsResult.Items.FirstOrDefault(
                        item => item.Id == video.VideoId);

                if (statistics == null)
                {
                    continue;
                }

                video.Views =
                    long.TryParse(
                        statistics.Statistics.ViewCount,
                        out var views)
                        ? views
                        : 0;

                video.Likes =
                    long.TryParse(
                        statistics.Statistics.LikeCount,
                        out var likes)
                        ? likes
                        : 0;

                video.Comments =
                    long.TryParse(
                        statistics.Statistics.CommentCount,
                        out var comments)
                        ? comments
                        : 0;

                // -----------------------------------------
                // STEP 6: Calculate age
                // -----------------------------------------

                if (DateTime.TryParse(
                    video.PublishedAt,
                    out var publishedAt))
                {
                    video.AgeHours =
                        Math.Max(
                            1,
                            (DateTime.UtcNow - publishedAt.ToUniversalTime())
                                .TotalHours);

                    // -----------------------------------------
                    // STEP 7: Calculate views per hour
                    // -----------------------------------------

                    video.ViewsPerHour =
                        video.Views / video.AgeHours;
                }
            }
        }

        // -----------------------------------------
        // STEP 8: Return final research result
        // -----------------------------------------

        return new YouTubeSearchResult
        {
            Query = query,
            Results = videos
        };
    }
}