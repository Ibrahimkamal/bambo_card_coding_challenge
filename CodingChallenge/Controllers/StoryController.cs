using CodingChallenge.Models;
using CodingChallenge.Services;
using Microsoft.AspNetCore.Mvc;
namespace CodingChallenge.Controllers
{
    [ApiController]
    [Route("api/stories")]
    public class StoryController(StoryService storyService) : ControllerBase
    {
        private readonly StoryService _storyService = storyService;

        [HttpGet("{storiesCount}")]
        public async Task<IActionResult> GetStories(int storiesCount)
        {
            try
            {
                if (storiesCount < 0)
                {
                    return BadRequest("Count should always be positive.");
                }

                List<StoryModel> stories = await _storyService.GetStoriesDetails(storiesCount);

                var result = stories.Select(story => new
                {
                    Title = story.Title,
                    Url = story.Url,
                    Score = story.Score,
                    Time = DateTimeOffset.FromUnixTimeSeconds(story.Time).UtcDateTime,
                    PostedBy = story.By,
                    CommentCount = story.Kids.Count
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
