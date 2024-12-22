using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;

namespace AdventOfCode2024;

public sealed class Day21 : BaseDay
{
    private readonly string _input;
    
    /*
        +---+---+---+
        | 7 | 8 | 9 |
        +---+---+---+
        | 4 | 5 | 6 |
        +---+---+---+
        | 1 | 2 | 3 |
        +---+---+---+
            | 0 | A |
            +---+---+
    */
    private static readonly Dictionary<char, List<char>> Numpad = new ()
    {
        { '7', ['4', '8']},
        { '8', ['7', '5', '9']},
        { '9', ['8', '6']},
        { '4', ['7', '5', '1']},
        { '5', ['4', '8', '6', '2']},
        { '6', ['9', '5', '3']},
        { '1', ['4', '2']},
        { '2', ['1', '5', '0', '3']},
        { '3', ['2', '6', 'A']},
        { '0', ['A', '2'] },
        { 'A', ['0', '3'] },
    };

    /*
            +---+---+
            | ^ | A |
        +---+---+---+
        | < | v | > |
        +---+---+---+
    */
    private static readonly Dictionary<char, List<char>> Keypad = new ()
    {
        { 'A', ['^', '>']},
        { '^', ['A', 'v']},
        { 'v', ['^', '>', '<']},
        { '<', ['v']},
        { '>', ['A', 'v']},
    };

    // The "fun" part to write...
    private static readonly Dictionary<(char, char), char> Pairs = new()
    {
        // numpad pairs
        { ('7', '4'), 'v' }, { ('7', '8'), '>' },
        { ('8', '7'), '<' }, { ('8', '9'), '>' }, { ('8', '5'), 'v' },
        { ('9', '8'), '<' }, { ('9', '6'), 'v' },
        { ('4', '7'), '^' }, { ('4', '5'), '>' }, { ('4', '1'), 'v' },
        { ('5', '4'), '<' }, { ('5', '8'), '^' }, { ('5', '6'), '>' }, { ('5', '2'), 'v' },
        { ('6', '9'), '^' }, { ('6', '5'), '<' }, { ('6', '3'), 'v' },
        { ('1', '4'), '^' }, { ('1', '2'), '>' },
        { ('2', '1'), '<' }, { ('2', '5'), '^' }, { ('2', '3'), '>' }, { ('2', '0'), 'v' },
        { ('3', '6'), '^' }, { ('3', '2'), '<' }, { ('3', 'A'), 'v' },
        { ('0', 'A'), '>' }, { ('0', '2'), '^' },
        { ('A', '0'), '<' },
        { ('A', '3'), '^' },
        
        // keypad pairs
        { ('A', '^'), '<' }, { ('A', '>'), 'v' },
        { ('^', 'A'), '>' }, { ('^', 'v'), 'v' },
        { ('v', '^'), '^' }, { ('v', '>'), '>' }, { ('v', '<'), '<' },
        { ('<', 'v'), '>' },
        { ('>', 'A'), '^' }, { ('>', 'v'), '<' },
    };

    public Day21()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var result = _input
            .Split(Environment.NewLine)
            .Select(code => int.Parse(code[..^1]) * ComputeShortestSequence(code, 2))
            .Sum();

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = _input
            .Split(Environment.NewLine)
            .Select(code => int.Parse(code[..^1]) * ComputeShortestSequence(code, 25))
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }
    
    private long ComputeShortestSequence(string code, int depth)
    {
        var length = 0L;
        var cache = new Dictionary<(char, char, int), long>();

        var path = ("A" + code).ToCharArray();
        
        var pairs = path.Zip(path.Skip(1), (from, to) => (from, to)).ToArray();

        foreach (var (from, to) in pairs)
        {
            length += ComputeShortestPathBetweenPair(from, to, Numpad, depth, cache);
        }

        return length;
    }

    private long ComputeShortestPathBetweenPair(char from, char to, Dictionary<char, List<char>> graph, int depth, Dictionary<(char, char, int), long> cache)
    {
        if(cache.TryGetValue((from, to, depth), out var result))
            return result;
        
        var paths = GenerateSequence(from, to, graph);
        
        if(depth == 0)
            return paths.First().Count;
        
        var shortest = long.MaxValue;

        foreach (var path in paths)
        {
            path.Insert(0, 'A');

            var pairs = path.Zip(path.Skip(1), (f, t) => (f, t)).ToArray();
            var sum = pairs.Sum(p => ComputeShortestPathBetweenPair(p.f, p.t, Keypad, depth - 1, cache));

            shortest = Math.Min(shortest, sum);
        }
        
        cache.TryAdd((from, to, depth), shortest);
        
        return shortest;
    }

    private List<List<char>> GenerateSequence(char from, char to, Dictionary<char, List<char>> graph) =>
        ComputeShortestPaths(from, to, graph).Select(path =>
        {
            var pairs = path.Zip(path.Skip(1), (f, t) => (f, t)).ToArray();
            
            return pairs.Select(p => Pairs[(p.f, p.t)]).Append('A').ToList();
        }).ToList();

    private List<List<char>> ComputeShortestPaths(char from, char to, Dictionary<char, List<char>> graph)
    {
        var queue = new Queue<(char From, List<char> Path)>();
        queue.Enqueue((from, [from]));

        var candidates = new List<List<char>>();

        while (queue.TryDequeue(out var current))
        {
            if (current.From == to)
            {
                candidates.Add(current.Path);
                continue;
            }

            foreach(var node in graph[current.From])
            {
                if(current.Path.Contains(node))
                    continue;
                
                var newPath = new List<char>(current.Path) { node };
                queue.Enqueue((node, newPath));
            }
        }

        var shortest = candidates.Select(candidate => candidate.Count).Min();
        
        return candidates.Where(candidate => candidate.Count == shortest).ToList();
    }
}