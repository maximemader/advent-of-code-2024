using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public sealed class Day13 : BaseDay
{
    private readonly string _input;

    public Day13()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private record Vector2D(long X, long Y)
    {
        public Vector2D Add(Vector2D vector)
        {
            return new Vector2D(X + vector.X, Y + vector.Y);
        }
    }

    private record ClawMachine(Vector2D Prize, Vector2D ButtonA, Vector2D ButtonB);
    
    public override ValueTask<string> Solve_1()
    {
        var clawMachines = ParseClawMachines();
        
        var result = clawMachines.Select(FewestTokensToWin).Sum();
        
        return new ValueTask<string>(result.ToString());
    }

    private List<ClawMachine> ParseClawMachines()
    {
        var machineRegex = new Regex(@"Button [AB]: X\+(\d+), Y\+(\d+)", RegexOptions.Compiled);
        var prizeRegex = new Regex(@"Prize: X=(\d+), Y=(\d+)", RegexOptions.Compiled); 
        
        return _input.Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(group => group.Split(Environment.NewLine))
            .Select(group =>
            {
                var buttonA = machineRegex.Match(group[0]).Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
                var buttonB = machineRegex.Match(group[1]).Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
                var prize = prizeRegex.Match(group[2]).Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();

                return new ClawMachine(new Vector2D(prize[0], prize[1]), new Vector2D(buttonA[0], buttonA[1]), new Vector2D(buttonB[0], buttonB[1]));
            })
            .ToList();
    }

    public override ValueTask<string> Solve_2()
    {
        var clawMachines = ParseClawMachines();
        
        var result = clawMachines
            .Select(machine => machine with { Prize = machine.Prize.Add(new Vector2D(10000000000000, 10000000000000)) })
            .Select(FewestTokensToWin)
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }

    private long FewestTokensToWin(ClawMachine clawMachine)
    {
        var tokens = 0L;
        
        var (baX, baY) = (clawMachine.ButtonA.X, clawMachine.ButtonA.Y);
        var (bbX, bbY) = (clawMachine.ButtonB.X, clawMachine.ButtonB.Y);
        var (pX, pY) = (clawMachine.Prize.X, clawMachine.Prize.Y);

        var nominator = pY * bbX - pX * bbY;
        var denominator = baY * bbX - baX * bbY;
        var a = nominator / denominator;
        var bx2 = pX - a * baX;

        if (nominator % denominator == 0L && bx2 % bbX == 0L)
            tokens += 3L * a + bx2 / bbX;

        return tokens;

        // This is a brute force solution, it works but it's not efficient for part 2! ;) 
        //
        // if((clawMachine.Prize.X > clawMachine.ButtonA.X * 100 + clawMachine.ButtonB.X * 100) ||
        //    (clawMachine.Prize.Y > clawMachine.ButtonA.Y * 100 + clawMachine.ButtonB.Y * 100))
        //     return 0;
        //
        // var solutions = new List<int>();
        //
        // for (int a = 0; a < 100; ++a)
        // {
        //     var position = new Vector2D(clawMachine.ButtonA.X * a, clawMachine.ButtonA.Y * a);
        //     
        //     if (clawMachine.Prize == position)
        //     {
        //         solutions.Add(a * 3);
        //         break;
        //     }
        //     
        //     for(int b = 1; b < 100; ++b)
        //     {
        //         position = position.Add(clawMachine.ButtonB);
        //         
        //         if(position.X > clawMachine.Prize.X || position.Y > clawMachine.Prize.Y)
        //             break;
        //
        //         if (clawMachine.Prize == position)
        //         {
        //             solutions.Add(a * 3 + b);
        //             break;
        //         }
        //     }
        // }
        //
        // return solutions.Any() ? solutions.Min():0;
    }
}