using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public sealed class Day14 : BaseDay
{
    private readonly string _input;

    public Day14()
    {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1()
    {
        var robots = ParseRobots();
        var space = (Width:101, Height:103);
        
        var positions = robots.Select(robot =>
        {
            var (x, y) = robot.Position;
            for (var i = 0; i < 100; ++i)
            {
                x = (x + robot.Velocity.X + space.Width) % space.Width;
                y = (y + robot.Velocity.Y + space.Height) % space.Height;
            }
            return (X:x, Y:y);
        }).ToList();
        
        var (midX, midY) = (space.Width / 2, space.Height / 2);
        
        // We could do a group by and an aggregate, but this is more readable
        var q1 = positions.Count(p => p.X < midX && p.Y < midY);
        var q2 = positions.Count(p => p.X > midX && p.Y < midY);
        var q3 = positions.Count(p => p.X < midX && p.Y > midY);
        var q4 = positions.Count(p => p.X > midX && p.Y > midY);
        
        var result = q1 * q2 * q3 * q4;
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // We have two solutions, one is to find the lowest safety, the other is to find the unique position.
        // We could also work on the entropy of the system, detect lines, detect the trunk, but that would be
        // a bit more complex.
        //
        // Another trick would be to use the Chinese Remainder Theorem, I still have PTSD from AoC 2020
        // day 13, so I'll avoid that for now. :)
        var result = UniquePositionSolution();

        return new ValueTask<string>(result.ToString());
    }
    
    // This is a brute force solution, reusing the first part idea.
    private int LowestSafetySolution()
    {
        var robots = ParseRobots();
        var space = (Width:101, Height:103);
        
        var lowest = (Safety:int.MaxValue, Period:0);
        
        var positions = robots.Select(x => x.Position).ToList();
        
        for(var period = 1; period <= 101 * 103; ++period)
        {
            for (var i = 0; i < robots.Count; ++i)
            {
                positions[i] = ((positions[i].X + robots[i].Velocity.X + space.Width) % space.Width,
                    (positions[i].Y + robots[i].Velocity.Y + space.Height) % space.Height);
            }

            var (midX, midY) = (space.Width / 2, space.Height / 2);
            
            // We could do a group by and an aggregate, but this is more readable
            var q1 = positions.Count(p => p.X < midX && p.Y < midY);
            var q2 = positions.Count(p => p.X > midX && p.Y < midY);
            var q3 = positions.Count(p => p.X < midX && p.Y > midY);
            var q4 = positions.Count(p => p.X > midX && p.Y > midY);
            
            var safety = q1 * q2 * q3 * q4;
            
            if(safety < lowest.Safety)
                lowest = (safety, period);
        }
        
        return lowest.Period;
    }

    // This is a brute force solution, unique property of the answer, all robots are on unique positions.
    private int UniquePositionSolution()
    {
        var robots = ParseRobots();
        var space = (Width:101, Height:103);
        
        var seconds = 0;
        while(robots.Select(r => (r.Position)).Distinct().Count() != robots.Count)
        {
            for (var i = 0; i < robots.Count; i++) 
            {
                var p = ((robots[i].Position.X + robots[i].Velocity.X + space.Width) % space.Width, 
                    (robots[i].Position.Y + robots[i].Velocity.Y + space.Height) % space.Height);
                
                robots[i] = (p, robots[i].Velocity);
            }
            
            ++seconds;
        }

        return seconds;
    }

    private List<((int X, int Y) Position, (int X, int Y) Velocity)> ParseRobots()
    {
        var robotRegex = new Regex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)", RegexOptions.Compiled);
        
        return _input.Split(Environment.NewLine).Select(line =>
        {
            var matches = robotRegex.Match(line).Groups.Values
                .Skip(1).Select(g => int.Parse(g.Value)).ToList();

            return (Position:(X:matches[0], Y:matches[1]), Velocity:(X:matches[2], Y:matches[3]));
        }).ToList();
    }

    private static void DisplayRobots((int Width, int Height) space, List<((int X, int Y) Position, (int X, int Y) Velocity)> robots)
    {
        for(var y = 0; y < space.Height; y++)
        {
            for(var x = 0; x < space.Width; x++)
            {
                var c = robots.Count(r => r.Position.X == x && r.Position.Y == y);
                if(c > 0)
                    Console.Write(c);
                else
                    Console.Write(".");
            }
            Console.WriteLine();
        }
    }
}