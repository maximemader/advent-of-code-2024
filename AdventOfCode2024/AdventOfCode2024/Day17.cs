using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public sealed class Day17 : BaseDay
{
    private readonly string _input;

    public Day17()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var computerState = ParseComputerState(_input);

        var output = string.Join(',', computerState.Execute());

        return new ValueTask<string>(output);
    }

    public override ValueTask<string> Solve_2()
    {
        // 2,4,1,1,7,5,0,3,1,4,4,4,5,5,3,0
        /*
         * (2,4) bst B = A % 8
         * (4,1) bxc B = B ^ C
         * (1,1) bxl B = B ^ 1
         * (1,7) bxl B = B ^ 7
         * (7,5) cdv C = A / 2^B
         * (5,0) out C % 0
         * (0,3) adv A = A / 2^3
         * (3,1) jnz A != 0
         * (1,4) bxl B = B ^ A
         * (4,4) bxc B = B ^ C
         * (4,4) bxc B = B ^ C
         * (4,5) bxc B = B ^ C
         * (5,5) out B % 8
         * (5,3) out 3
         * (3,0) jnz A != 0
         * 0
         */
        
        var computerState = ParseComputerState(_input);
        var result = 0L;
        
        var index = computerState.Program.Length - 1;
        while(index >= 0)
        {
            for (var value = 0; value < int.MaxValue; ++value)
            {
                var a = result + (1L << (index * 3)) * value;
                computerState.Registers[0] = a;

                if (computerState.Execute().Skip(index).SequenceEqual(computerState.Program.Skip(index)))
                {
                    result = a;
                    break;
                }
            }

            --index;
        }
                
        return new ValueTask<string>(result.ToString());
    }

    private ComputerState ParseComputerState(string input)
    {
        var parts = _input.Split($"{Environment.NewLine}{Environment.NewLine}");
        
        var registers = parts[0].Split(Environment.NewLine)
            .Select(line => long.Parse(line.Substring(12)))
            .ToArray();

        var programAsString = parts[1].Substring(9);
        var program = programAsString.Split(',').Select(int.Parse).ToArray();

        return new ComputerState(registers, program, programAsString);
    }
    
    
    private record ComputerState(long[] Registers, int[] Program, string ProgramAsString)
    {
        public IEnumerable<int> Execute()
        {
            var ip = 0;
            var A = 0;
            var B = 1;
            var C = 2;

            while (ip < Program.Length)
            {
                var opcode = Program[ip];
                var operand = Program[ip + 1];

                switch (opcode)
                {
                    // adv - Division to register A
                    case 0:
                        Registers[A] >>= (int)GetCombo(operand);
                        break;
                    // bxl - Bitwise XOR
                    case 1:
                        Registers[B] ^= operand;
                        break;
                    // bst - mod 8
                    case 2:
                        Registers[B] = GetCombo(operand) & 7;
                        break;
                    // jnz - jump if not zero
                    case 3:
                        if (Registers[A] != 0)
                        {
                            ip = operand;
                            ip -= 2;
                        }

                        break;
                    // bxc - Bitwise XOR B^C
                    case 4:
                        Registers[B] ^= Registers[C];
                        break;
                    // out - Output
                    case 5:
                        yield return (int)(GetCombo(operand) & 7);
                        break;
                    // bdv - Division to register B
                    case 6:
                        Registers[B] = Registers[A] >> (int)GetCombo(operand);
                        break;
                    // cdv - Division to register C
                    case 7:
                        Registers[C] = Registers[A] >> (int)GetCombo(operand);
                        break;
                }

                ip += 2;
            }

            long GetCombo(int operand) => (int)(operand is >= 0 and <= 3 ? operand : Registers[operand - 4]);
        }
    }
}