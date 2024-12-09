namespace AdventOfCode2024;

public sealed class Day09 : BaseDay
{
    private readonly string _input;

    public Day09()
    {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1()
    {
        var data = ParseInput();
        
        for (int tail = data.Length - 1, head = 0; tail > head; --tail)
        {
            while (data[head] >= 0)
                ++head;
            
            while (data[tail] < 0)
                --tail;

            if (tail <= head)
                continue;

            data[head] = data[tail];
            data[tail] = -1;
        }
        
        var checksum = ComputeChecksum(data);
        
        return new ValueTask<string>(checksum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var data = ParseInput();
        
        var tmp = new int[10];

        for (var tail = data.Length - 1; tail > tmp[1]; --tail)
        {
            if (data[tail] < 0)
                continue;

            int head, freeBlockCount, blockCount = 0;

            while (tail >= 0 && data[tail] == data[tail + blockCount])
            {
                --tail;
                ++blockCount;
            }
            
            for (++tail, head = tmp[blockCount], freeBlockCount = 0; tail > head; ++head)
            {
                if (data[head] >= 0)
                {
                    freeBlockCount = 0;
                }
                else if (++freeBlockCount == blockCount)
                {
                    tmp[blockCount] = head + 1;
                    break;
                }
            }
            
            SwapFile(data, tail, head, blockCount);
        }
        
        var checksum = ComputeChecksum(data);
        
        return new ValueTask<string>(checksum.ToString());
    }
    
    void SwapFile(int[] data, int tail, int head, int blockCount)
    {
        if (tail <= head)
            return;
        
        while (blockCount > 0)
        {
            data[head] = data[tail];
            data[tail] = -1;

            ++tail; 
            --head; 
            
            --blockCount;
        }
    }

    private static long ComputeChecksum(int[] input)
    {
        return input
            .Select((v, i) => v < 0 ? 0L : v * i)
            .Sum();
    }

    private int[] ParseInput()
    {
        return _input
            .SelectMany((c, i) => Enumerable.Repeat(i % 2 == 0 ? i / 2 : -1, c - '0'))
            .ToArray();
    }
}