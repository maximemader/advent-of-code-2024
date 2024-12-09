using System.Collections;

namespace AdventOfCode2024;

public sealed class Day08 : BaseDay
{
    private readonly string _input;

    public Day08()
    {
        _input = File.ReadAllText(InputFilePath);
    }
    
    private record Vector2D(int X, int Y)
    {
        public Vector2D Add(Vector2D other) => new(X + other.X, Y + other.Y);
        public Vector2D Sub(Vector2D other) => new(X - other.X, Y - other.Y);
        
        public bool IsInBounds(int width, int height) => X >= 0 && X < width && Y >= 0 && Y < height;
    };

    public override ValueTask<string> Solve_1()
    {
        var map = _input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray())
            .ToArray();
        
        var width = map[0].Length;
        var height = map.Length;
        
        var antennasByFrequency = GetAntennasByFrequency(map, width, height);
        
        var antinodes = new HashSet<Vector2D>();

        foreach (var antennas in antennasByFrequency)
        {
            for (var i = 0; i < antennas.Value.Count - 1; ++i)
            {
                for (var j = 1; j < antennas.Value.Count; ++j)
                {
                    var delta = antennas.Value[j].Sub(antennas.Value[i]);

                    var left = antennas.Value[i].Sub(delta);
                    
                    if(left.IsInBounds(width, height) && map[left.Y][left.X] != antennas.Key)
                        antinodes.Add(left);
                    
                    var right = antennas.Value[j].Add(delta);
                    
                    if(right.IsInBounds(width, height) && map[right.Y][right.X] != antennas.Key)
                        antinodes.Add(right);
                }
            }
        }
        
        var result = antinodes.Count;
        
        return new ValueTask<string>(result.ToString());
    }

    

    public override ValueTask<string> Solve_2()
    {
        var map = _input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray())
            .ToArray();
        
        var width = map[0].Length;
        var height = map.Length;
        
        var antennasByFrequency = GetAntennasByFrequency(map, width, height);
        
        var antinodes = new HashSet<Vector2D>();

        foreach (var antennas in antennasByFrequency)
        {
            for (var i = 0; i < antennas.Value.Count - 1; ++i)
            {
                for (var j = 1; j < antennas.Value.Count; ++j)
                {
                    var delta = antennas.Value[j].Sub(antennas.Value[i]);

                    var left = antennas.Value[i].Sub(delta);
                    
                    while(left.IsInBounds(width, height) && map[left.Y][left.X] != antennas.Key)
                    {
                        antinodes.Add(left);
                        left = left.Sub(delta);
                    }
                    
                    var right = antennas.Value[j].Add(delta);
                    
                    while(right.IsInBounds(width, height) && map[right.Y][right.X] != antennas.Key)
                    {
                        antinodes.Add(right);
                        right = right.Add(delta);
                    }

                    antinodes.Add(antennas.Value[i]);
                    antinodes.Add(antennas.Value[j]);
                }
            }
        }
        
        var result = antinodes.Count;
        
        return new ValueTask<string>(result.ToString());
    }
    
    private Dictionary<char, List<Vector2D>> GetAntennasByFrequency(char[][] map, int width, int height)
    {
        var result = new Dictionary<char, List<Vector2D>>();
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var c = map[y][x];
                if (c == '.')
                {
                    continue;
                }

                if (!result.ContainsKey(c))
                {
                    result[c] = [];
                }

                result[c].Add(new Vector2D(x, y));
            }
        }

        return result;
    }
}