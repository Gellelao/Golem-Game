namespace GolemCore.Models.Part;

public class CreatePartRequest
{
    public string TableName { get; init; } = "part";

    public Part Item { get; init; } = new();
}