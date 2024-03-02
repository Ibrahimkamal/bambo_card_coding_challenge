using CodingChallenge.Models;
namespace CodingChallenge.Services;
public class StoryService(IHttpClientFactory httpClientFactory, ILogger<StoryService> logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<StoryService> _logger = logger;

    private readonly StoriesIds StoriesIds = new();
    private readonly Dictionary<int, StoryDetails> Stories = [];

    public async Task<List<StoryModel>> GetStoriesDetails(int storiesCount)
    {
        await FetchStoriesIdsResponseAsync();
        List<Task<StoryModel>> tasks = [];

        storiesCount = Math.Min(storiesCount, StoriesIds.Ids.Count);

        for (int i = 0; i < storiesCount; i++)
        {
            tasks.Add(FetchStoryIdResponseAsync(this.StoriesIds.Ids[i]));
        }

        // Await all the tasks to complete and then extract the results.
        StoryModel[] results = await Task.WhenAll(tasks);

        // Convert the array to a list and return it.
        return [.. results];
    }

    private async Task FetchStoriesIdsResponseAsync()
    {
        string url = "https://hacker-news.firebaseio.com/v0/beststories.json";
        long currentTimesstamp = getUnixTimestamp();
        if (currentTimesstamp - StoriesIds.Timestamp < 60)
        {
            return;
        }

        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            List<int> response = await client.GetFromJsonAsync<List<int>>(url);
            StoriesIds.Ids = response;
            StoriesIds.Timestamp = currentTimesstamp;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private async Task<StoryModel> FetchStoryIdResponseAsync(int storyId)
    {

        string url = $"https://hacker-news.firebaseio.com/v0/item/{storyId}.json";

        long current_timestamp = getUnixTimestamp();
        if (Stories.ContainsKey(storyId))
        {
            if (current_timestamp - this.Stories[storyId].Timestamp > 60)
            {
                return this.Stories[storyId].storyDetails;
            }
        }
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            var storyModel = await client.GetFromJsonAsync<StoryModel>(url);
            StoryDetails storyDetails = new()
            {
                Timestamp = current_timestamp,
                storyDetails = storyModel
            };
            this.Stories[storyId] = storyDetails;
            return storyModel;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private long getUnixTimestamp()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return now.ToUnixTimeSeconds();
    }
}