namespace CodingChallenge.Models;
public class StoryModel
{
    public string By { get; set; }
    public int Descendants { get; set; }
    public long Id { get; set; }
    public List<int> Kids { get; set; }
    public int Score { get; set; }
    public long Time { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
}
public class StoriesIds
{

    public List<int> Ids { get; set; } = [];
    public long Timestamp { get; set; }
}
public class StoryDetails
{
    public StoryModel storyDetails { get; set; }
    public long Timestamp { get; set; }
}