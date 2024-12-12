using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day12 : BaseDay
{
    private readonly string _input;

    public Day12()
    {
        _input = File.ReadAllText(InputFilePath);
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
    
    private record Map2D(char[][] Data)
    {
        public char this[Vector2D position]
        {
            get => IsInBounds(position) ? Data[position.Y][position.X]: '#';
            set => Data[position.Y][position.X] = value;
        }

        public int Width => Data[0].Length;
        public int Height => Data.Length;
        
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsInBounds(Vector2D position) => IsInBounds(position.X, position.Y);
        
        public static Map2D Parse(string input) => new(input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToArray())
            .ToArray());
    }

    public override ValueTask<string> Solve_1()
    {
        var map = Map2D.Parse(_input);

        var regions = new List<(char Plant, int Area, int Perimeter)>();

        var visitedPlants = new HashSet<Vector2D>();

        // Brute force approach, a flood fill algorithm would be more efficient
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                var position = new Vector2D(x, y);

                var plant = map[position];

                if (plant == '#' || visitedPlants.Contains(position))
                    continue;

                var area = 1;
                var perimeter = 0;

                var queue = new Queue<Vector2D>();

                queue.Enqueue(position);
                visitedPlants.Add(position);

                while (queue.TryDequeue(out var current))
                {
                    foreach (var direction in Vector2D.Directions)
                    {
                        var next = current.Add(direction);

                        if (map[next] != plant)
                        {
                            ++perimeter;
                            continue;
                        }
                        
                        if(!visitedPlants.Add(next))
                        {
                            continue;
                        }

                        ++area;
                        queue.Enqueue(next);
                    }
                }
                
                regions.Add((plant, area, perimeter));
            }
        }
        
        var result = regions.Sum(region => region.Area * region.Perimeter);
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var map = Map2D.Parse(_input);

        var regions = new List<(char Plant, int Area, int Sides)>();

        var visitedPlants = new HashSet<Vector2D>();

        // Brute force approach, a flood fill algorithm would be more efficient
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                var position = new Vector2D(x, y);

                var plant = map[position];

                if (plant == '#' || visitedPlants.Contains(position))
                    continue;

                var area = 1;
                var sides = 0;

                var fences = new HashSet<(Vector2D, Vector2D)>();

                var queue = new Queue<Vector2D>();

                queue.Enqueue(position);
                visitedPlants.Add(position);

                while (queue.TryDequeue(out var current))
                {
                    foreach (var direction in Vector2D.Directions)
                    {
                        var next = current.Add(direction);

                        if (map[next] != plant)
                        {
                            fences.Add((current, direction));
                            continue;
                        }
                        
                        if(!visitedPlants.Add(next))
                        {
                            continue;
                        }

                        ++area;
                        queue.Enqueue(next);
                    }
                }
                
                while (fences.Count > 0)
                {
                    ++sides;
                    
                    var current = fences.First();
                    fences.Remove(current);
                    var (currentPosition, currentDirection) = current;

                    foreach (var direction in Vector2D.Directions)
                    {
                        var offset = currentPosition;
                        while (fences.Contains((offset.Add(direction), currentDirection)))
                        {
                            fences.Remove((offset.Add(direction), currentDirection)); 
                            offset = offset.Add(direction);
                        }
                    }
                }
                
                regions.Add((plant, area, sides));
            }
        }
        
        var result = regions.Sum(region => region.Area * region.Sides);
        
        return new ValueTask<string>(result.ToString());
    }
}