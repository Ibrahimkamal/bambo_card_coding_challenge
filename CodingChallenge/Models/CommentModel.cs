
namespace CodingChallenge.Models;
public class CommentModel
{
    public string By { get; set; }
    public int Id { get; set; }
    public List<int> Kids { get; set; } = new List<int>();
    public int Parent { get; set; }
    public string Text { get; set; }
    public int Time { get; set; }
    public string Type { get; set; }
}
