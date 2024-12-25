namespace AdventOfCode2024;

public sealed class Day25 : BaseDay
{
    private readonly string _input;

    public Day25()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var schematics = _input.Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(block => block.Split(Environment.NewLine))
            .ToArray();
        
        var keys = new List<int[]>();
        var locks = new List<int[]>();

        foreach (var schematic in schematics)
        {
            if (schematic[0].All(c => c == '.'))
            {
                var key = new int[schematic[0].Length];
                for (var x = 0; x < schematic[0].Length; ++x)
                {
                    for(var y = schematic.Length - 2; y > 0 ; --y)
                    {
                        if (schematic[y][x] == '.')
                            break;

                        ++key[x];
                    }
                }
                keys.Add(key);
            }
            else if (schematic[0].All(c => c == '#'))
            {
                var @lock = new int[schematic[0].Length];
                for (var x = 0; x < schematic[0].Length; ++x)
                {
                    for(var y = 1; y < schematic.Length - 1 ; ++y)
                    {
                        if (schematic[y][x] != '#')
                            break;

                        ++@lock[x];
                    }
                }
                locks.Add(@lock);
            }
        }

        var result = locks
            .Sum(@lock => keys
                .Count(key => !key.Where((t, i) => t + @lock[i] > 5)
                .Any()));

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>("Thank you, Eric");
    }
}