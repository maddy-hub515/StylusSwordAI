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

        var url =
            "https://www.googleapis.com/youtube/v3/search" +
            $"?part=snippet" +
            $"&q={Uri.EscapeDataString(query)}" +
            $"&type=video" +
            $"&order=date" +
            $"&maxResults={maxResults}" +
            $"&relevanceLanguage=en" +
            $"&key={apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"YouTube API request failed. " +
                $"Status: {response.StatusCode}. " +
                $"Response: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<YouTubeSearchResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        if (result == null)
        {
            return new YouTubeSearchResult
            {
                Query = query
            };
        }

        return new YouTubeSearchResult
        {
            Query = query,

            Results = result.Items.Select(item => new YouTubeVideo
            {
                VideoId = item.Id.VideoId,
                Title = item.Snippet.Title,
                Channel = item.Snippet.ChannelTitle,
                PublishedAt = item.Snippet.PublishedAt,
                Url = $"https://www.youtube.com/watch?v={item.Id.VideoId}"
            }).ToList()
        };




    }
}