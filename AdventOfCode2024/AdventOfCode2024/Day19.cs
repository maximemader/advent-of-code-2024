namespace AdventOfCode2024;

public sealed class Day19 : BaseDay
{
    private readonly string _input;

    public Day19()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var parts = _input.Split($"{Environment.NewLine}{Environment.NewLine}");

        var patterns = parts[0].Split(", ").ToArray();
        var designs = parts[1].Split(Environment.NewLine).ToArray();

        var cache = new Dictionary<string, bool>();
        
        var result = designs.Count(IsPossibleDesign);

        bool IsPossibleDesign(string design)
        {
            if (string.IsNullOrEmpty(design))
                return true;

            if (cache.TryGetValue(design, out var cached))
                return cached;
            
            cache[design] = patterns.Where(design.StartsWith).Any(p => IsPossibleDesign(design[p.Length..]));
            return cache[design];
        }
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var parts = _input.Split($"{Environment.NewLine}{Environment.NewLine}");

        var patterns = parts[0].Split(", ").ToArray();
        var designs = parts[1].Split(Environment.NewLine).ToArray();

        var cache = new Dictionary<string, long>();
        
        var result = designs.Sum(CountPossibleDesign);

        long CountPossibleDesign(string design)
        {
            if (string.IsNullOrEmpty(design))
                return 1L;

            if (cache.TryGetValue(design, out var cached))
                return cached;
            
            cache[design] = patterns.Where(design.StartsWith).Sum(p => CountPossibleDesign(design[p.Length..]));
            return cache[design];
        }
        
        return new ValueTask<string>(result.ToString());
    }
}