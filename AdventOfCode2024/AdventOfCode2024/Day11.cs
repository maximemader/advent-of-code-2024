namespace AdventOfCode2024;

public sealed class Day11 : BaseDay
{
    private readonly string _input;

    public Day11()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var stones = _input.Split(" ").Select(long.Parse).ToList();
        
        var result = ComputeStoneCount(stones, 25);

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var stones = _input.Split(" ").Select(long.Parse).ToList();
        
        var result = ComputeStoneCount(stones, 75);

        return new ValueTask<string>(result.ToString());
    }

    private long ComputeStoneCount(List<long> stones, uint count)
    {
        // The depth must be less than 128 to avoid collisions in the cache
        if(count > 127)
            throw new ArgumentOutOfRangeException(nameof(count), "The depth must be less than 128");
        
        var cache = new Dictionary<long, long>();
        
        long Blink(long value, uint depth)
        {
            if (depth == 0)
                return 1L;

            --depth;
            
            // Concat the value and the depth to create a unique key
            // value is shifted by 7 to avoid collision with depth
            var key = value << 7 | depth;

            if (cache.TryGetValue(key, out var result)) 
                return result;
            
            if (value == 0)
            {
                result = Blink(1, depth);
            }
            else
            {
                // We're using power/log10 to split the number in two parts without using strings.
                // See Day07.cs for a similar approach.
                var power = (long)Math.Log10(value) + 1L;

                // Has an even number of digits?
                if ((power & 1) != 0)
                {
                    // Default rule
                    result = Blink(value * 2024, depth);
                }
                else
                {
                    var modulus = (long)Math.Pow(10, power >> 1);
                    
                    // Split the number in two parts and sum their stone count.
                    result = Blink(value / modulus, depth) + Blink(value % modulus, depth);
                }
            }
                
            cache.Add(key, result);

            return result;
        }

        return stones
            .Select(x => Blink(x, count))
            .Sum();
    }
}