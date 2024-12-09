namespace AdventOfCode2024;

public sealed class Day07 : BaseDay
{
    private readonly string _input;

    public Day07()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var lines = ParseEquations();
        
        var result = lines
            .Where(line => IsValidCalibration(line, DivideEquation,SubtractEquation))
            .Sum(line => line.Result);

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var lines = ParseEquations();
        
        var result = lines
            .Where(line => IsValidCalibration(line, DivideEquation,SubtractEquation,SplitEquation))
            .Sum(line => line.Result);

        return new ValueTask<string>(result.ToString());
    }
    
    private static long DivideEquation(long a, long b)
    {
        var (div, mod) = Math.DivRem(a, b);
        
        // if the remainder is 0, then we have a valid division
        // otherwise we want to exit the calculation, hence -1.
        return mod == 0 ? div : -1;
    }
    private static long SubtractEquation(long a, long b) => a - b;
    
    /// <summary>
    /// This trick will allow us to reverse the concatenation operation without having to do any string manipulations. 
    /// </summary>
    private static long SplitEquation(long a, long b) => DivideEquation(a - b, PowLogApprox(b));

    private static long PowLogApprox(long b) => b switch
    {
        < 10 => 10,
        < 100 => 100,
        < 1000 => 1000,
        _ => 10000
    };
    
    /// <summary>
    /// The idea here is to use a stack to keep track of the equations that need to be solved.
    /// We're reversing the order of the equations as well as the operations to avoid as many
    /// calculations as possible. 
    /// </summary>
    private bool IsValidCalibration((long Result, long[] Values) valueTuple, params ReadOnlySpan<Func<long, long, long>> operations)
    {
        var equations = new Stack<(long PartialResult, int NextIndex)>();
        equations.Push((valueTuple.Result, valueTuple.Values.Length - 1));

        while (equations.Count > 0)
        {
            var (result, nextIndex) = equations.Pop();
            var nextValue = valueTuple.Values[nextIndex];

            foreach (var operation in operations)
            {
                var nextResult = operation.Invoke(result, nextValue);

                if (nextResult == 0 && nextIndex == 0)
                    return true;
                
                if(nextResult > 0 && nextIndex > 0)
                    equations.Push((nextResult, nextIndex - 1));
            }
        }

        return false;
    }

    private IEnumerable<(long Result, long[] Values)> ParseEquations()
    {
        return _input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split(": ");
                
                return (Result: long.Parse(parts[0]), Values: parts[1].Split(' ').Select(long.Parse).ToArray());
            });
    }
}