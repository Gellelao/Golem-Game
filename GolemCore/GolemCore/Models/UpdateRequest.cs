namespace GolemCore.Models;

public class UpdateRequest
{
  public string TableName { get; init; }

  public dynamic Key { get; init; }

  public string UpdateExpression { get; init; }

  public string ConditionExpression { get; init; }

  public Dictionary<string, object> ExpressionAttributeValues { get; init; }
}