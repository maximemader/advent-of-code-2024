using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day05 : BaseDay
{
    private readonly string _input;

    public Day05()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var (pageOrderingRules, updates) = ParseSafetyManual();

        var result = updates
            .Where(update => IsCorrectlyOrderedUpdate(update, pageOrderingRules))
            .Select(update => update[update.Count / 2])
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var (pageOrderingRules, updates) = ParseSafetyManual();
        
        var result = updates
            .Where(update => !IsCorrectlyOrderedUpdate(update, pageOrderingRules))
            .Select(update =>
            {
                var reorderedUpdate = update
                    .Order(new PageComparer(pageOrderingRules))
                    .ToImmutableList();
                
                return reorderedUpdate[reorderedUpdate.Count / 2];
            })
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }
    
    private class PageComparer(ImmutableDictionary<int, List<int>> pageOrderingRules) : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if(pageOrderingRules.TryGetValue(x, out var pagesAfterX) && pagesAfterX.Contains(y))
            {
                return -1;
            }
            
            if(pageOrderingRules.TryGetValue(y, out var pagesAfterY) && pagesAfterY.Contains(x))
            {
                return 1;
            }
            
            return 0;
        }
    }
    
    private static bool IsCorrectlyOrderedUpdate(ImmutableList<int> update, ImmutableDictionary<int, List<int>> pageOrderingRules)
    {
        var producedPages = new HashSet<int>();
            
        foreach (var page in update)
        {
            if(pageOrderingRules.TryGetValue(page, out var afterPages) && afterPages.Any(producedPages.Contains))
            {
                return false;
            }
                
            if (!producedPages.Add(page))
            {
                return false;
            }
        }

        return true;
    }

    private record SafetyManual(
        ImmutableDictionary<int, List<int>> PageOrderingRules,
        ImmutableList<ImmutableList<int>> Updates);

    private SafetyManual ParseSafetyManual()
    {
        var safetyManual = _input.Split($"{Environment.NewLine}{Environment.NewLine}");
        
        var pageOrderingRules = safetyManual[0].Split(Environment.NewLine)
            .Select(line =>
            {
                var parts = line.Split("|");
                
                return (Before:int.Parse(parts[0]), After:int.Parse(parts[1]));
            })
            .GroupBy(
                x => x.Before,
                x => x.After, 
                (key, g) => new { Before = key, Afters = g.ToList() } )
            .ToImmutableDictionary(pageOrder => pageOrder.Before, pageOrder => pageOrder.Afters);
        
        var updates = safetyManual[1].Split(Environment.NewLine)
            .Select(line => line
                .Split(',').Select(int.Parse).ToImmutableList())
            .ToImmutableList();
        
        return new SafetyManual(pageOrderingRules, updates);
    }
}