namespace AdventOfCode2024;

using System.Linq;
using System.Collections.Generic;

public sealed class Day01 : BaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var (leftList, rightList) = ParseLeftAndRightLists();

        leftList.Sort();
        rightList.Sort();
        
        var result = leftList
            .Select((n, i) => Math.Abs(n - rightList[i]))
            .Sum();

        return new ValueTask<string>(result.ToString());
    }
    
    public override ValueTask<string> Solve_2()
    {
        var (leftList, rightList) = ParseLeftAndRightLists();

        var occurrences = new Dictionary<long, int>();
        foreach (var n in rightList)
        {
            if (!occurrences.TryAdd(n, 1))
            {
                occurrences[n]++;
            }
        }
        
        var result = leftList
            .Select(n => occurrences.TryGetValue(n, out var occurence) ? n * occurence: 0)
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }

    private (List<long> leftList, List<long> rightList) ParseLeftAndRightLists()
    {
        List<long> leftList = [];
        List<long> rightList = [];
        foreach (var line in _input.Split('\n'))
        {
            var rawNumbers = line.Split("   ");
            
            leftList.Add(long.Parse(rawNumbers[0]));
            rightList.Add(long.Parse(rawNumbers[1]));
        }

        return (leftList, rightList);
    }
}