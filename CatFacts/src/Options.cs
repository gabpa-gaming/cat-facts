
namespace CatFacts.src;

public class Options
{
    public bool ShowHelp { get; }
    public string Url { get; } = "https://catfact.ninja/fact";

    public int Count { get; } = 1;

    static Options Parse(string[] args)
    {
        // Implement argument parsing logic here
        return new Options();
    }
}