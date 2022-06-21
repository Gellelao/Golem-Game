namespace GolemCore;

public class ValidationProblem
{
    public string Reason { get; private set; }

    public ValidationProblem(string reason)
    {
        Reason = reason;
    }
}