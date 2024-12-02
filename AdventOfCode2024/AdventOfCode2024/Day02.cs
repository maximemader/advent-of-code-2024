namespace AdventOfCode2024;

public sealed class Day02 : BaseDay
{
    private readonly string _input;

    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var reports = ParseReports();

        var result = reports.Count(IsSafe);
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var reports = ParseReports();

        var result = reports.Count(levels =>
        {
            for(var i = 0; i < levels.Count; ++i)
            {
                var dampenedLevels = levels.ToList();
                dampenedLevels.RemoveAt(i);

                if (IsSafe(dampenedLevels))
                    return true;
            }

            return false;
        });
        
        return new ValueTask<string>(result.ToString());
    }
    
    private static bool IsSafe(List<int> levels)
    {
        var trend = levels[1] - levels[0] > 0;
        for (var i = 0; i < levels.Count - 1; ++i)
        {
            var diff = levels[i + 1] - levels[i];
            
            if(trend != diff > 0) 
                return false;
            
            if(Math.Abs(levels[i + 1] - levels[i]) is < 1 or > 3)
                return false;
        }

        return true;
    }

    private List<List<int>> ParseReports()
    {
        var reports = _input
            .Split('\n')
            .Select(levels => levels
                .Split(' ')
                .Select(int.Parse)
                .ToList())
            .ToList();
        return reports;
    }
}