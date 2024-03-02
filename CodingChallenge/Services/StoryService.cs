using CodingChallenge.Models;
namespace CodingChallenge.Services;
public class StoryService(IHttpClientFactory httpClientFactory, ILogger<StoryService> logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<StoryService> _logger = logger;

    private readonly StoriesIds StoriesIds = new();
    private readonly Dictionary<int, StoryDetails> Stories = [];

    private readonly int RefreshTimeInSeconds = 300;
    public async Task<List<StoryModel>> GetStoriesDetails(int storiesCount)
    {
        await FetchStoriesIdsResponseAsync();
        List<Task<StoryModel>> tasks = [];

        storiesCount = Math.Min(storiesCount, StoriesIds.Ids.Count);

        for (int i = 0; i < storiesCount; i++)
        {
            tasks.Add(FetchStoryIdResponseAsync(this.StoriesIds.Ids[i]));
        }

        StoryModel[] results = await Task.WhenAll(tasks);

        return [.. results];
    }

    private async Task FetchStoriesIdsResponseAsync()
    {
        string url = "https://hacker-news.firebaseio.com/v0/beststories.json";
        long currentTimesstamp = getUnixTimestamp();
        if (currentTimesstamp - StoriesIds.Timestamp < RefreshTimeInSeconds)
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
            if (current_timestamp - this.Stories[storyId].Timestamp < RefreshTimeInSeconds)
            {
                return this.Stories[storyId].storyDetails;
            }
        }
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            var storyModel = await client.GetFromJsonAsync<StoryModel>(url);
            storyModel.CommentsCount = await GetCommentsCount(storyModel.Kids);
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
    private async Task<int> GetCommentsCount(List<int> kids)
    {
        var commentCountTasks = new List<Task<int>>();

        foreach (int kid in kids)
        {
            commentCountTasks.Add(GetCommentCountAsync(kid));
        }

        int[] results = await Task.WhenAll(commentCountTasks);
        return results.Sum();
    }

    private async Task<int> GetCommentCountAsync(int kid)
    {
        string url = $"https://hacker-news.firebaseio.com/v0/item/{kid}.json";
        HttpClient client = _httpClientFactory.CreateClient();
        CommentModel commentModel = await client.GetFromJsonAsync<CommentModel>(url);

        int count = 0;
        if (commentModel != null && commentModel.Type == "comment")
        {
            count = 1;
            if (commentModel.Kids != null && commentModel.Kids.Count > 0)
            {
                count += await GetCommentsCount(commentModel.Kids);
            }
        }
        return count;
    }

    private long getUnixTimestamp()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return now.ToUnixTimeSeconds();
    }
}