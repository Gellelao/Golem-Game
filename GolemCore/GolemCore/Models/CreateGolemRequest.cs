namespace GolemCore.Models;

public class CreateGolemRequest
{
    public const string TableName = "golem";
    public Golem Item { get; init; } = new();
}