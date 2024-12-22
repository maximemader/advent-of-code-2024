namespace AdventOfCode2024;

public sealed class Day22 : BaseDay
{
    private readonly string _input;

    public Day22()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var result = _input
            .Split(Environment.NewLine)
            .Select(long.Parse)
            .Select(secret =>
            {
                for (var i = 0; i < 2000; i++)
                {
                    secret = ComputeNextSecret(secret);
                }
                return secret;
            })
            .Sum();
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        const int priceChanges = 2000;
        
        var buyerPrices = _input
            .Split(Environment.NewLine)
            .Select(long.Parse)
            .Select(secret =>
            {
                var prices = new int[priceChanges + 1];
                prices[0] = (int)(secret % 10);
                
                for (var i = 1; i <= priceChanges; ++i)
                {
                    secret = ComputeNextSecret(secret);
                    prices[i] = (int)(secret % 10);
                }
                return prices;
            })
            .ToArray();

        var buyerChanges = buyerPrices
            .Select(prices =>
                prices
                    .Skip(1)
                    .Select((price, i) => price - prices[i])
                    .ToArray())
            .ToArray();

        var firstOccurencePerChanges = new Dictionary<(int, int, int, int), Dictionary<int, int>>();
        for (var i = 0; i < buyerChanges.Length; ++i)
        {
            for (var j = 0; j < buyerChanges[i].Length - 4; ++j)
            {
                var key = (buyerChanges[i][j], buyerChanges[i][j + 1], buyerChanges[i][j + 2], buyerChanges[i][j + 3]);
                
                if (!firstOccurencePerChanges.ContainsKey(key))
                {
                    firstOccurencePerChanges[key] = new Dictionary<int, int> { { i, j } };
                }
                else
                {
                    firstOccurencePerChanges[key].TryAdd(i, j);
                }
            }
        }

        var result = firstOccurencePerChanges
            .Select(sequence => 
                sequence.Value.Aggregate(0L, (acc, pair) => acc + buyerPrices[pair.Key][pair.Value + 4]))
            .Max();
        
        return new ValueTask<string>(result.ToString());
    }
    
    private long ComputeNextSecret(long secret)
    {
        // * 64, XOR, % 16777216
        secret = (secret ^ (secret << 6)) & 0xFFFFFF;
        // / 32, XOR, % 16777216 not needed
        secret ^= (secret >> 5);
        // * 2024, XOR, % 16777216
        secret = (secret ^ (secret << 11)) & 0xFFFFFF;
        return secret;
    }
}