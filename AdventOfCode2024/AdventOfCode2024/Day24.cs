using System.Collections;
using System.Globalization;
using System.Numerics;

namespace AdventOfCode2024;

public sealed class Day24 : BaseDay
{
    private readonly string _input;

    public Day24()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var (gates, wires) = ParseInput();

        // Plain and simple, we're using a queue just to resolve the gates in a working order.
        var queue = new Queue<(string InputA, string Op, string InputB, string Output)>(gates);
        while (queue.TryDequeue(out var gate))
        {
            if (!wires.ContainsKey(gate.InputA) || !wires.ContainsKey(gate.InputB))
            {
                queue.Enqueue(gate);
                continue;
            }
            
            wires[gate.Output] = gate.Op switch
            {
                "AND" => wires[gate.InputA] & wires[gate.InputB],
                "OR" => wires[gate.InputA] | wires[gate.InputB],
                "XOR" => wires[gate.InputA] ^ wires[gate.InputB],
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        // No upper bound with a BigInteger.
        var result = new BigInteger();
        
        foreach (var key in wires.Keys.Where(key => key.StartsWith("z")))
        {
            var p = int.Parse(key[1..]);
            
            if(wires[key])
                result += BigInteger.Pow(2, p);
        }

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var (gates, _) = ParseInput();
        
        // We need to find which gates are faulty
        // https://en.wikipedia.org/wiki/Adder_(electronics)
        // https://upload.wikimedia.org/wikipedia/commons/5/57/Fulladder.gif
        var result = new HashSet<string>();
        
        // The most significant bit (MSB).
        var msbGate = gates
            .Select(gate => gate.Output)
            .Where(output => output.StartsWith('z'))
            .OrderDescending()
            .First();

        // Only outputs may be incorrect, so we can check only one of the inputs.
        foreach (var gate in gates)
        {
            // Every gate output that is not the MSB, should be computed from an XOR operation.
            // The MSB should be computed from an OR operation.
            if (IsWireZ(gate.Output) && gate.Op != "XOR" && gate.Output != msbGate)
            {
                result.Add(gate.Output);
            } 
            // Every XOR operation should either
            // - output to a Z wire
            // - have inputs that are an X and Y wire
            else if (gate.Op == "XOR" && !IsWireZ(gate.Output) && !IsWireXorY(gate.InputA))
            {
                result.Add(gate.Output);
            }
            // Every X/Y inputs should be used with XOR or OR operations.
            // For the least significant bit, we don't have a Carry input, so there's an exception to ignore.
            else if(IsWireXorY(gate.InputA) && !IsLeastSignificantBit(gate.InputA))
            {
                // We need to find the next operation that should be used.
                // Case 1: **(X XOR Y)** XOR Cin = S, we're looking for an XOR.
                // Case 2: ((X XOR Y) AND Cin) OR **(X AND Y)** = S, we're looking for an OR.
                if (!gates.Any(g => g != gate &&
                            (g.InputA == gate.Output || g.InputB == gate.Output) && 
                            g.Op == (gate.Op == "XOR" ? "XOR" : "OR")))
                {
                    result.Add(gate.Output);
                }
            }
        }
        
        bool IsWireZ(string wire) => wire.StartsWith('z');
        
        // Unfortunately, the order of the wires aren't always InputA -> X, InputB -> Y.
        bool IsWireXorY(string wire) => wire.StartsWith('x') || wire.StartsWith('y');
        
        bool IsLeastSignificantBit(string wire) => wire.EndsWith("00");
        
        return new ValueTask<string>(string.Join(',', result.Order()));
    }
    
    private (List<(string InputA, string Op, string InputB, string Output)> gates, Dictionary<string, bool> wires) ParseInput()
    {
        var parts = _input.Split($"{Environment.NewLine}{Environment.NewLine}");

        var wires = parts[0]
            .Split(Environment.NewLine)
            .Select(line => line.Split(": "))
            .ToDictionary(p => p[0], p => int.Parse(p[1]) == 1);
        
        var gates = parts[1]
            .Split(Environment.NewLine)
            .Select(line => line.Split([" -> ", " "], StringSplitOptions.RemoveEmptyEntries))
            .Select(gate => (InputA: gate[0], Op: gate[1], InputB: gate[2], Output: gate[3]))
            .ToList();

        return (gates, wires);
    }
}