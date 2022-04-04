using GolemCore.Models;

namespace GolemApp;

public class ConsoleView
{
    private readonly IConsoleWrapper _console;

    public ConsoleView(IConsoleWrapper console)
    {
        _console = console;
    }

    public void PrintPart(Part part)
    {
        _console.WriteLine($"Id:{part.Id}, {part.Tier} tier, {part.Material}, {part.Type}");
    }
}