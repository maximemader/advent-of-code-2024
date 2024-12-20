using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day20 : BaseDay
{
    private readonly string _input;

    public Day20()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var (start, end, obstacles) = ParseMap();
         
        var path = ComputePath(start, end, obstacles);
        
        var score = ComputeCheatCount(path, 2);
        
        return new ValueTask<string>(score.ToString());
    }

    private static int ComputeCheatCount(List<Vector2D> path, int cheatDuration)
    {
        var score = 0;
        for(var i = 0; i < path.Count; ++i)
        {
            for (var j = 0; j < i; ++j)
            {
                var distance = path[i].ManhattanDistance(path[j]);
                var saving = i - (j + distance);
                
                if(distance <= cheatDuration && saving >= 100)
                    ++score;
            }
        }

        return score;
    }

    public override ValueTask<string> Solve_2()
    {
        var (start, end, obstacles) = ParseMap();
         
        var path = ComputePath(start, end, obstacles);
        
        var score = ComputeCheatCount(path, 20);
        
        return new ValueTask<string>(score.ToString());
    }

    private (Vector2D Start, Vector2D End, HashSet<Vector2D> Obstacles) ParseMap()
    {
        var map = _input.Split(Environment.NewLine).Select(line => line.ToCharArray()).ToArray();
        var start = Vector2D.Zero;
        var end = Vector2D.Zero;
        var obstacles = new HashSet<Vector2D>();
         
        for(var y = 0; y < map.Length; ++y)
        {
            for(var x = 0; x < map[y].Length; ++x)
            {
                switch(map[y][x])
                {
                    case 'S':
                        start = new Vector2D(x, y);
                        break;
                    case 'E':
                        end = new Vector2D(x, y);
                        break;
                    case '#':
                        obstacles.Add(new Vector2D(x, y));
                        break;
                }
            }
        }

        return (start, end, obstacles);
    }

    private List<Vector2D> ComputePath(Vector2D start, Vector2D end, HashSet<Vector2D> obstacles)
    {
        var visited = new HashSet<Vector2D>(); 
         
        var queue = new Queue<(Vector2D Position, Vector2D Direction)>();
        queue.Enqueue((start, Vector2D.North));
        
        while (queue.TryDequeue(out var current))
        {
            if (current.Position == end) 
                break;
            
            foreach (var direction in Vector2D.Directions)
            {
                var next = current.Position.Add(direction);
                
                if (obstacles.Contains(next) || !visited.Add(next))
                    continue;

                queue.Enqueue((next, direction));
            }
        }
        
        return visited.ToList();
    }
    
    private record Vector2D(int X, int Y)
    {
        public static Vector2D Zero => new(0, 0);
        public static Vector2D North => new(0, -1);
        public static Vector2D South => new(0, 1);
        public static Vector2D East => new(1, 0);
        public static Vector2D West => new(-1, 0);
        
        public static readonly ImmutableArray<Vector2D> Directions = [
            ..new[]
            {
                North, East, South, West
            }
        ];
        
        public Vector2D Add(Vector2D other) => new(X + other.X, Y + other.Y);
        
        public int ManhattanDistance(Vector2D other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    };
}