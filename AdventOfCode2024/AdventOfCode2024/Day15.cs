using System.Collections.Immutable;
using System.Numerics;

namespace AdventOfCode2024;

public sealed class Day15 : BaseDay
{
    private readonly string _input;

    public Day15()
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

    public override ValueTask<string> Solve_1()
    {
        var parts = _input.Split($"{Environment.NewLine}{Environment.NewLine}");
        var map = ParseMapForPart1(parts[0]);
        var moves = ParseMoves(parts[1]);

        var robot = new Vector2D(0,0);
        var boxes = new HashSet<Vector2D>();
        var obstacles = new HashSet<Vector2D>();
        
        for (var y = 0; y < map.Length; ++y)
        {
            for (var x = 0; x < map[y].Length; ++x)
            {
                switch (map[y][x])
                {
                    case '@':
                        robot = new Vector2D(x, y);
                        break;
                    case 'O':
                        boxes.Add(new Vector2D(x, y));
                        break;
                    case '#':
                        obstacles.Add(new Vector2D(x, y));
                        break;
                }
            }
        }

        foreach (var move in moves)
        {
            var newPosition = robot.Add(move);
            
            if(obstacles.Contains(newPosition))
                continue;
            
            if (boxes.Contains(newPosition))
            {
                var canMove = false;
                var nextPosition = newPosition;
                while (true)
                {
                    nextPosition = nextPosition.Add(move);
                    
                    if (obstacles.Contains(nextPosition))
                        break;

                    if (!boxes.Contains(nextPosition))
                    {
                        canMove = true;
                        break;
                    }
                }

                if (canMove)
                {
                    robot = newPosition;
                    boxes.Remove(newPosition);
                    boxes.Add(nextPosition);
                }
            }
            else
            {
                robot = newPosition;
            }
        }
        
        var result = boxes.Sum(box => box.X + box.Y * 100);
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var parts = _input.Split($"{Environment.NewLine}{Environment.NewLine}");
        var map = ParseMapForPart2(parts[0]);
        var moves = ParseMoves(parts[1]);

        var robot = GetRobotPosition(map);
        
        foreach (var move in moves)
        {
            var candidatePosition = robot.Add(move);

            var tile = map[candidatePosition.Y][candidatePosition.X];
            
            if(tile is '#' or '@')
                continue;

            if (tile is '[' or ']')
            {
                if (move == Vector2D.East || move == Vector2D.West)
                {
                    if (!MoveHorizontally(candidatePosition, move, map))
                        continue;
                }
                else
                {
                    if (!MoveVertically(candidatePosition, move, map))
                        continue;
                }
            }
            
            map[robot.Y][robot.X] = '.';
            robot = candidatePosition;
            map[robot.Y][robot.X] = '@';
        }
        
        var result = map.SelectMany((line, y) => 
                line.Select((c, x) => c == '[' ? x + y * 100: 0))
            .Sum();

        return new ValueTask<string>(result.ToString());
    }

    private static List<Vector2D> ParseMoves(string input)
    {
        return input.Replace(Environment.NewLine, "").Select(move => move switch
        {
            '^' => Vector2D.North,
            'v' => Vector2D.South,
            '<' => Vector2D.West,
            '>' => Vector2D.East,
            _ => throw new ArgumentOutOfRangeException()
        }).ToList();
    }
    
    private static char[][] ParseMapForPart1(string input)
    {
        return input.Split(Environment.NewLine).Select(line => line.ToCharArray()).ToArray();
    }

    private static char[][] ParseMapForPart2(string input)
    {
        return input
            .Split(Environment.NewLine)
            .Select(line =>
            {
                var v = line.Select(c => c switch
                {
                    '#' => "##",
                    'O' => "[]",
                    '@' => "@.",
                    '.' => "..",
                }).ToList();

                return v.Aggregate((i, j) => i + j).ToCharArray();
            }).ToArray();
    }

    private bool MoveVertically(Vector2D candidatePosition, Vector2D move, char[][] map)
    {
        var tile = map[candidatePosition.Y][candidatePosition.X];
        
        if (tile is '#' or '@')
            return false;

        if (tile is '.')
        {
            map[candidatePosition.Y][candidatePosition.X] = map[candidatePosition.Y - move.Y][candidatePosition.X];

            return true;
        }

        // We have a problem here, we need to check if we can move the boxes!
        Queue<(Vector2D Position, char Tile)> queue = new();
        queue.Enqueue((candidatePosition, tile));
        
        var visitedBoxes = new List<(Vector2D Position, char Tile)>();

        while (queue.TryDequeue(out var current))
        {
            var currentTile = map[current.Position.Y][current.Position.X];

            if(currentTile is '#' || currentTile is '@')
                return false;
            
            if (currentTile is '.')
                continue;

            if (currentTile is '[')
            {
                var rb = (current.Position.Add(Vector2D.East), ']');

                if (!visitedBoxes.Contains(rb))
                {
                    visitedBoxes.Add(rb);
                    queue.Enqueue(rb);
                }

                var nextPosition = current.Position.Add(move);
                queue.Enqueue((nextPosition, map[nextPosition.Y][nextPosition.X]));
            }
            
            if (currentTile is ']')
            {
                var lb = (current.Position.Add(Vector2D.West), '[');
                
                if (!visitedBoxes.Contains(lb))
                {
                    visitedBoxes.Add(lb);
                    queue.Enqueue(lb);
                }
                
                var nextPosition = current.Position.Add(move);
                queue.Enqueue((nextPosition, map[nextPosition.Y][nextPosition.X]));
            }
        }

        visitedBoxes = move.Y > 0 ? visitedBoxes.OrderBy(x => x.Position.Y).ToList()
                :  visitedBoxes.OrderByDescending(x => x.Position.Y).ToList();
        
        for(var i = visitedBoxes.Count - 1; i >= 0; --i)
        {
            var ((x, y), c) = visitedBoxes[i];
            map[y + move.Y][x] = c;
            map[y][x] = '.';
        }

        return true;
    }

    private bool MoveHorizontally(Vector2D candidatePosition, Vector2D move, char[][] map)
    {
        var tile = map[candidatePosition.Y][candidatePosition.X];
        
        if (tile is '#' or '@')
            return false;

        if (tile is '.')
        {
            map[candidatePosition.Y][candidatePosition.X] = map[candidatePosition.Y][candidatePosition.X - move.X];

            return true;
        }
        
        // We have a problem here, we need to check if we can move the boxes!
        Queue<Vector2D> queue = new();
        queue.Enqueue(candidatePosition);
        
        var visitedBoxes = new List<Vector2D>();

        while (queue.TryDequeue(out var current))
        {
            var currentTile = map[current.Y][current.X];

            if(currentTile is '#' || currentTile is '@')
                return false;
            
            if (currentTile is '.')
                continue;

            visitedBoxes.Add(current);
            queue.Enqueue(current.Add(move));
        }
        
        for(var i = visitedBoxes.Count - 1; i >= 0; --i)
        {
            var (x, y) = visitedBoxes[i];
            map[y][x + move.X] = map[y][x];
            map[y][x] = '.';
        }

        return true;
    }

    private Vector2D GetRobotPosition(char[][] map)
    {
        for (var y = 0; y < map.Length; ++y)
        {
            for (var x = 0; x < map[0].Length; ++x)
            {
                if (map[y][x] == '@')
                    return new Vector2D(x, y);
            }
        }
            
        throw new NotImplementedException();
    }
}