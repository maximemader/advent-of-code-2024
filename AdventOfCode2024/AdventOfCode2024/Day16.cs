using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day16 : BaseDay
{
    private readonly string _input;

    public Day16()
    {
        _input = File.ReadAllText(InputFilePath);
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

    public override ValueTask<string> Solve_1()
    {
        var map = _input.Split(Environment.NewLine).Select(line => line.ToCharArray()).ToArray();
        
        var obstacles = new HashSet<Vector2D>();
        var start = Vector2D.Zero;
        var end = Vector2D.Zero;
        
        for (var y = 0; y < map.Length; ++y)
        {
            for (var x = 0; x < map[y].Length; ++x)
            {
                switch (map[y][x])
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

        var reindeer = (Position:start, Direction:Vector2D.East, Visited:new HashSet<Vector2D>(), Score:0, Directions:new List<Vector2D>());

        var queue = new Queue<(Vector2D Position, Vector2D Direction, HashSet<Vector2D> Visited, int Score, List<Vector2D> Directions)>();
        queue.Enqueue(reindeer);
        
        var finishers = new List<(Vector2D Position, Vector2D Direction, HashSet<Vector2D> Visited, int Score, List<Vector2D> Directions)>();
        var score = int.MaxValue;

        var positionScore = new Dictionary<Vector2D, int>();
        
        while (queue.TryDequeue(out var current))
        {
            if(current.Position == end)
            {
                if(current.Score < score)
                {
                    finishers.Clear();
                    score = current.Score;
                }
                finishers.Add(current);
                continue;
            }
            
            if(positionScore.TryGetValue(current.Position, out var lowestPositionScore) && lowestPositionScore < current.Score)
                continue;
            
            positionScore[current.Position] = current.Score;

            if(current.Score > score)
                continue;

            var directions = new List<Vector2D>()
            {
                current.Direction,
                Vector2D.Directions[(Vector2D.Directions.IndexOf(current.Direction) + 1) % 4],
                Vector2D.Directions[(Vector2D.Directions.IndexOf(current.Direction) + 3) % 4],
            };
            
            foreach(var direction in directions)
            {
                var newPosition = current.Position.Add(direction);

                if (obstacles.Contains(newPosition) || current.Visited.Contains(newPosition))
                    continue;
                
                var newVisited = new HashSet<Vector2D>(current.Visited) { newPosition };
                var newDirection = new List<Vector2D>(current.Directions) { direction };
                queue.Enqueue((newPosition, direction, newVisited, direction == current.Direction ? current.Score + 1: current.Score + 1001, newDirection));
            }
        }

        var result = finishers.OrderBy(f => f.Score).First();
        
        return new ValueTask<string>(result.Score.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var map = _input.Split(Environment.NewLine).Select(line => line.ToCharArray()).ToArray();
        
        var obstacles = new HashSet<Vector2D>();
        var start = Vector2D.Zero;
        var end = Vector2D.Zero;
        
        for (var y = 0; y < map.Length; ++y)
        {
            for (var x = 0; x < map[y].Length; ++x)
            {
                switch (map[y][x])
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

        var reindeer = (Position:start, Direction:Vector2D.East, Visited:new HashSet<Vector2D>(), Score:0);

        var queue = new Queue<(Vector2D Position, Vector2D Direction, HashSet<Vector2D> Visited, int Score)>();
        queue.Enqueue(reindeer);
        
        var finishers = new List<(Vector2D Position, Vector2D Direction, HashSet<Vector2D> Visited, int Score)>();
        var score = int.MaxValue;

        // Could be improved (BFS + DFS), but it's good enough for this problem
        // Part 1 was already close to the part 2 solution, runs in less than 3 seconds.
        var positionScore = new Dictionary<(Vector2D Position, Vector2D Direction), int>();
        
        while (queue.TryDequeue(out var current))
        {
            if(current.Position == end)
            {
                if(current.Score < score)
                {
                    finishers.Clear();
                    score = current.Score;
                }
                finishers.Add(current);
                continue;
            }
            
            if(positionScore.TryGetValue((current.Position, current.Direction), out var lowestPositionScore))
            {
                if(lowestPositionScore < current.Score)
                    continue;
            }
            
            positionScore[(current.Position, current.Direction)] = current.Score;

            if(current.Score > score)
                continue;

            var directions = new List<Vector2D>
            {
                current.Direction,
                Vector2D.Directions[(Vector2D.Directions.IndexOf(current.Direction) + 1) % 4],
                Vector2D.Directions[(Vector2D.Directions.IndexOf(current.Direction) + 3) % 4],
            };
            
            foreach(var direction in directions)
            {
                var newPosition = current.Position.Add(direction);

                if (obstacles.Contains(newPosition) || current.Visited.Contains(newPosition))
                    continue;
                
                var newVisited = new HashSet<Vector2D>(current.Visited) { newPosition };
                queue.Enqueue((newPosition, direction, newVisited, direction == current.Direction ? current.Score + 1: current.Score + 1001));
            }
        }

        var result = finishers.SelectMany(finisher => finisher.Visited).Distinct().Count() + 1;
        
        return new ValueTask<string>(result.ToString());
    }
    
    
}