namespace AdventOfCode2024;

public sealed class Day04 : BaseDay
{
    private readonly string _input;

    public Day04()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var letters = ParseLettersArray();
        
        var result = 0;
        
        var directions = new (int x, int y)[]
        {
            // Horizontal
            (1, 0), (-1, 0),
            // Vertical
            (0, 1), (0, -1),
            // Diagonal
            (1, 1), (-1, -1), 
            (1, -1), (-1, 1)
        };

        var height = letters.Length;
        var width = letters[0].Length;
        
        for(var y = 0; y < height; ++y)
        {
            for (var x = 0; x < width; ++x)
            {
                 if(letters[y][x] != 'X') 
                     continue;

                 foreach (var (dx, dy) in directions)
                 {
                     var limitY = y + dy * 3;
                     var limitX = x + dx * 3;
                     
                     if(limitY < 0 || limitY > height - 1 ||
                        limitX < 0 || limitX > width - 1)
                         continue;
                     
                     if (letters[y + dy][x + dx] == 'M' &&
                         letters[y + dy * 2][x + dx * 2] == 'A' &&
                         letters[y + dy * 3][x + dx * 3] == 'S')
                     {
                         ++result;
                     }
                 }
            }
        }
        
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var letters = ParseLettersArray();
        
        var result = 0;

        var height = letters.Length;
        var width = letters[0].Length;
        
        var values = new[] { "SM", "MS" };
        
        for(var y = 1; y < height - 1; ++y)
        {
            for (var x = 1; x < width - 1; ++x)
            {
                if(letters[y][x] != 'A') 
                    continue;

                if (values.Contains($"{letters[y - 1][x - 1]}{letters[y + 1][x + 1]}") && 
                    values.Contains($"{letters[y - 1][x + 1]}{letters[y + 1][x - 1]}"))
                    ++result;
            }
        }
        
        return new ValueTask<string>(result.ToString());
    }
    
    private char[][] ParseLettersArray()
    {
        return _input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(y => y.ToCharArray()).ToArray();
    }
}