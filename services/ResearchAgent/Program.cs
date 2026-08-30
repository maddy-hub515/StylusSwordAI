using ResearchAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Configuration.AddJsonFile(
    "Channel.json",
    optional: false,
    reloadOnChange: true);
builder.Services.AddHttpClient<YouTubeResearchService>();
builder.Services.AddScoped<YouTubeRelevanceService>();

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.MapGet(
    "/research/youtube",
    async (
        string query,
        YouTubeResearchService youtubeService,
        YouTubeRelevanceService relevanceService) =>
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Results.BadRequest(new
            {
                message = "Query is required."
            });
        }

        try
        {
            var result =
                await youtubeService.SearchAsync(query);

            relevanceService.Analyze(result.Results);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "YouTube research failed",
                detail: ex.Message);
        }
    });



app.Run();


