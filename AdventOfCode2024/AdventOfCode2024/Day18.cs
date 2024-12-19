using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day18 : BaseDay
{
    private readonly string _input;

    public Day18()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        // Another grid problem, yay!
        var fallingBytes = _input.Split(Environment.NewLine).Select(line =>
        {
            var coordinates = line.Split(',');
            return new Vector2D(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
        });

        var start = Vector2D.Zero;
        var end = new Vector2D(70, 70);
        var corruptedCount = 1024;
        
        var obstacles = new HashSet<Vector2D>();
        foreach (var fallingByte in fallingBytes.Take(corruptedCount))
        {
            obstacles.Add(fallingByte);
        }

        var score = Dijkstra(start, end, obstacles);
        
        return new ValueTask<string>(score.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var fallingBytes = _input.Split(Environment.NewLine).Select(line =>
        {
            var coordinates = line.Split(',');
            return new Vector2D(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
        }).ToList();

        var start = Vector2D.Zero;
        var end = new Vector2D(70, 70);
        var corruptedCount = 1024;
        
        var obstacles = new HashSet<Vector2D>();
        foreach (var fallingByte in fallingBytes.Take(corruptedCount))
        {
            obstacles.Add(fallingByte);
        }

        // Let's corrupt the bytes one by one until we find the corrupted one that causes the path to be blocked.
        // This is a brute force solution, but it's good enough for the input size.
        //
        // How can we improve ?
        // - We can fast-forward the corrupted bytes that are not in the path.
        // - We just need a path so no need to calculate the shortest path.
        // - We use a binary search to find the corrupted byte since we have a few thousands candidates.
        while(true)
        {
            var score = Dijkstra(start, end, obstacles);
            if (score == -1)
            {
                return new ValueTask<string>(fallingBytes.ElementAt(corruptedCount).ToString());
            }
            ++corruptedCount;
            obstacles.Add(fallingBytes.ElementAt(corruptedCount));
        }
    }

    private int Dijkstra(Vector2D start, Vector2D end, HashSet<Vector2D> obstacles)
    {
        var distances = new Dictionary<Vector2D, int> {{start, 0}};
        var visited = new HashSet<Vector2D>();
        var queue = new PriorityQueue<(Vector2D Position, int Cost), int>();
        queue.Enqueue((start, 0), 0);

        while (queue.TryDequeue(out var current, out var priority))
        {
            if (current.Position == end)
            {
                return distances[current.Position];
            }

            foreach (var direction in Vector2D.Directions)
            {
                var next = current.Position.Add(direction);
                if (obstacles.Contains(next) || visited.Contains(next))
                {
                    continue;
                }
                
                if(next.X < 0 || next.Y < 0 || next.X > end.X || next.Y > end.Y)
                {
                    continue;
                }

                visited.Add(current.Position);

                var distance = distances[current.Position] + 1;
                if (!distances.TryGetValue(next, out var currentDistance) || distance < currentDistance)
                {
                    distances[next] = distance;
                    queue.Enqueue((next, distance), distance);
                }
            }
        }

        return -1;
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
    };
}