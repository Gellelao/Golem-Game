namespace GolemCore.Models;

public class Golem
{
    public DateTime Timestamp { get; init; } = DateTime.Now;

    public string UserId { get; init; }
    public string Size { get; init; }
    public string Class { get; init; }
}