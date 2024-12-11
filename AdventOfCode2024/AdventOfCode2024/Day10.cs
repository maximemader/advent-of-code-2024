using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day10 : BaseDay
{
    private readonly string _input;

    public Day10()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private record Map2D(int[][] Data)
    {
        public int this[Vector2D position]
        {
            get => Data[position.Y][position.X];
            set => Data[position.Y][position.X] = value;
        }
        
        public int Width => Data[0].Length;
        public int Height => Data.Length;
        
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsInBounds(Vector2D position) => IsInBounds(position.X, position.Y);
        
        public static Map2D Parse(string input) => new(input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Select(c => (int)(c - '0')).ToArray())
            .ToArray());
    }

    private record Vector2D(int X, int Y)
    {
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
        var map = Map2D.Parse(_input);
        
        var starts = GetTilesByAltitude(map, 0);

        var result = starts.Sum(start =>
        {
            var visited = new HashSet<Vector2D>();
            
            var tops = 0;
            
            var queue = new Queue<(Vector2D Position, int Altitude)>(); 
            
            queue.Enqueue((start, 0));
            
            while(queue.TryDequeue(out var current))
            {
                if(current.Altitude == 9)
                {
                    ++tops;
                    continue;
                }
                
                var targetAltitude = current.Altitude + 1;
                
                foreach (var direction in Vector2D.Directions)
                {
                    var nextPosition = current.Position.Add(direction);
                    
                    if (!visited.Contains(nextPosition) &&
                        map.IsInBounds(nextPosition) &&
                        map[nextPosition] == targetAltitude)
                    {
                        visited.Add(nextPosition);

                        queue.Enqueue((nextPosition, targetAltitude));
                    };
                }
            }

            return tops;
        });
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var map = Map2D.Parse(_input);
        
        var starts = GetTilesByAltitude(map, 9);

        var result = starts.Sum(start =>
        {
            var bottoms = 0;
            
            var queue = new Queue<(Vector2D Position, int Altitude)>(); 
            
            queue.Enqueue((start, 9));
            
            while(queue.TryDequeue(out var current))
            {
                if(current.Altitude == 0)
                {
                    ++bottoms;
                    continue;
                }
                
                var targetAltitude = current.Altitude - 1;
                
                foreach (var direction in Vector2D.Directions)
                {
                    var nextPosition = current.Position.Add(direction);
                    
                    if (map.IsInBounds(nextPosition) && map[nextPosition] == targetAltitude)
                        queue.Enqueue((nextPosition, targetAltitude));
                }
            }

            return bottoms;
        });
        
        return new ValueTask<string>(result.ToString());
    }

    private static IEnumerable<Vector2D> GetTilesByAltitude(Map2D map, int altitude)
    {
        return map.Data.Select((row, y) => (Row:row, Y:y))
            .SelectMany(rowData => rowData.Row.Select((v, x) => (Value:v, X:x, rowData.Y)))
            .Where(tile => tile.Value == altitude)
            .Select(tile => new Vector2D(tile.X, tile.Y));
    }
}