using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public sealed class Day03 : BaseDay
{
    private readonly string _input;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var matches = Regex.Matches(_input, @"mul\((\d{1,3}),(\d{1,3})\)");
        
        var result = matches
            .Select(match => long.Parse(match.Groups[1].ValueSpan) * long.Parse(match.Groups[2].ValueSpan))
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var matches = Regex.Matches(_input, @"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)");

        var enabled = true;
        var result = 0L;

        foreach (Match match in matches)
        {
            switch (match.Value)
            {
                case "don't()":
                    enabled = false;
                    break;
                case "do()":
                    enabled = true;
                    break;
                default:
                    if (enabled)
                    {
                        result += long.Parse(match.Groups[1].ValueSpan) * long.Parse(match.Groups[2].ValueSpan);
                    }
                    break;
            }
        }
        
        return new ValueTask<string>(result.ToString());
    }
}