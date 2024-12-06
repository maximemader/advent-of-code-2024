using System.Collections.Immutable;

namespace AdventOfCode2024;

public sealed class Day06 : BaseDay
{
    private readonly string _input;

    public Day06()
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
        
        public bool IsInBounds(int width, int height) => X >= 0 && X < width && Y >= 0 && Y < height;
    };

    public override ValueTask<string> Solve_1()
    {
        var map = ParseMap();
        
        var initialGuardPosition = GetInitialGuardPosition(map);
        
        var visitedPositions = GetVisitedPositions(map, initialGuardPosition);

        var result = visitedPositions.Count;
        
        return new ValueTask<string>(result.ToString());
    }

    private HashSet<Vector2D> GetVisitedPositions(char[][] map, Vector2D guardPosition)
    {
        var visitedPositions = new HashSet<Vector2D>();
        var guardDirectionIndex = 0;
        
        while(true)
        {
            visitedPositions.Add(guardPosition);

            var forwardPosition = guardPosition.Add(Vector2D.Directions[guardDirectionIndex]);

            if(!forwardPosition.IsInBounds(map[0].Length,map.Length))
            {
                break;
            }
            
            while (map[forwardPosition.Y][forwardPosition.X] == '#')
            {
                guardDirectionIndex = (guardDirectionIndex + 1) % Vector2D.Directions.Length;
                forwardPosition = guardPosition.Add(Vector2D.Directions[guardDirectionIndex]);
                
                if(!forwardPosition.IsInBounds(map[0].Length,map.Length))
                {
                    break;
                }
            }

            guardPosition = forwardPosition;
        }

        return visitedPositions;
    }

    public override ValueTask<string> Solve_2()
    {
        var initialMap = ParseMap();
        var initialGuardPosition = GetInitialGuardPosition(initialMap);
        
        var visitedPositions = GetVisitedPositions(initialMap, initialGuardPosition);
        visitedPositions.Remove(initialGuardPosition);
        
        var result = visitedPositions.AsParallel().Count(visitedPosition =>
        {
            var map = initialMap.Select(line => line.ToArray()).ToArray();
            map[visitedPosition.Y][visitedPosition.X] = '#';

            return IsGuardStuckInALoop(map, initialGuardPosition);
        });
        
        return new ValueTask<string>(result.ToString());
    }
    
    private static bool IsGuardStuckInALoop(char[][] map, Vector2D initialGuardPosition)
    {
        var visitedPositions = new HashSet<(Vector2D, int)>();
        
        var guardDirectionIndex = 0;
        var guardPosition = initialGuardPosition;

        while(!visitedPositions.Contains((guardPosition, guardDirectionIndex)))
        {
            visitedPositions.Add((guardPosition, guardDirectionIndex));

            var forwardPosition = guardPosition.Add(Vector2D.Directions[guardDirectionIndex]);

            if(!forwardPosition.IsInBounds(map[0].Length,map.Length))
            {
                return false;
            }
            
            while (map[forwardPosition.Y][forwardPosition.X] == '#')
            {
                guardDirectionIndex = (guardDirectionIndex + 1) % Vector2D.Directions.Length;
                forwardPosition = guardPosition.Add(Vector2D.Directions[guardDirectionIndex]);
                
                if(!forwardPosition.IsInBounds(map[0].Length,map.Length))
                {
                    return false;
                }
            }

            guardPosition = forwardPosition;
        }

        return true;
    }

    private static Vector2D GetInitialGuardPosition(char[][] map)
    {
        var result= map
            .Select((line, y) => (line, y))
            .SelectMany(tuple => tuple.line.Select((cell, x) => (cell, x, tuple.y)))
            .First(tuple => tuple.cell == '^');

        return new Vector2D(result.x, result.y);
    }

    private char[][] ParseMap()
    {
        return _input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray())
            .ToArray();
    }
}