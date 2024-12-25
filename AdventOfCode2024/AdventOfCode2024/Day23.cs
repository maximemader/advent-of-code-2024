namespace AdventOfCode2024;

public sealed class Day23 : BaseDay
{
    private readonly string _input;

    public Day23()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var connections = ParseConnections();

        var computersWithT = connections.Keys.Where(key => key[0] == 't').ToList();
        
        var hashSet = new HashSet<Triplet>();
        
        // Copilot's suggestion after I created my Triplet class, if it works, it works! ;)
        foreach (var computer in computersWithT)
        {
            var list = connections[computer];
            
            foreach (var other in list)
            {
                if (!connections.TryGetValue(other, out var otherList)) 
                    continue;
                
                foreach (var third in otherList)
                {
                    if (!connections.TryGetValue(third, out var thirdList)) 
                        continue;
                    
                    if (thirdList.Contains(computer))
                    {
                        hashSet.Add(new Triplet(computer, other, third));
                    }
                }
            }
        }
        
        return new ValueTask<string>(hashSet.Count.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var connections = ParseConnections();
        
        // Found https://en.wikipedia.org/wiki/Clique_problem
        //
        // Bron Kerbosch algorithm
        // https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
        var cliques = new List<HashSet<string>>();
        
        var graph = connections.ToDictionary(pair => pair.Key, pair => new HashSet<string>(pair.Value));
        
        BronKerbosch(graph, r: [], p: [..graph.Keys], x: [], cliques);
        
        var result = string.Join(',', (cliques.MaxBy(c => c.Count) ?? []).Order());
        
        return new ValueTask<string>(result);
    }
    
    private static void BronKerbosch(Dictionary<string, HashSet<string>> graph, HashSet<string> r, HashSet<string> p, HashSet<string> x,
        List<HashSet<string>> cliques)
    {
        // if P and X are both empty then
        // report R as a maximal clique
        if (p.Count == 0 && x.Count == 0)
        {
            cliques.Add([..r]);
            return;
        }

        // choose a pivot vertex u in P ⋃ X
        var u = p.Concat(x).First();
        
        // for each vertex v in P \ N(u) do
        var vNotU = new HashSet<string>(p.Except(graph[u]));

        foreach (var v in vNotU)
        {
            var r2 = new HashSet<string>(r) { v };
            var p2 = new HashSet<string>(p.Intersect(graph[v]));
            var x2 = new HashSet<string>(x.Intersect(graph[v]));
            
            // BronKerbosch2(R ⋃ {v}, P ⋂ N(v), X ⋂ N(v))
            BronKerbosch(graph, r2, p2, x2, cliques);
            
            // P := P \ {v}
            p.Remove(v);
            
            // X := X ⋃ {v}
            x.Add(v);
        }
    }

    private Dictionary<string, List<string>> ParseConnections()
    {
        var networkMap = _input
            .Split(Environment.NewLine)
            .Select(line => line.Split('-'))
            .ToArray();
        
        var connections = new Dictionary<string, List<string>>();
        
        foreach (var pair in networkMap)
        {
            if(connections.TryGetValue(pair[0], out var list1))
                list1.Add(pair[1]);
            else
                connections[pair[0]] = [pair[1]];
            
            if(connections.TryGetValue(pair[1], out var list2))
                list2.Add(pair[0]);
            else
                connections[pair[1]] = [pair[0]];
        }

        return connections;
    }

    private class Triplet()
    {
        public Triplet(string a, string b, string c) : this()
        {
            A = a;
            B = b;
            C = c;
        }

        public string A { get; }
        public string B { get; }
        public string C { get; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            
            var other = obj as Triplet;
            if (other == null)
                return false;

            return (A == other.A || A == other.B || A == other.C) &&
                   (B == other.A || B == other.B || B == other.C) &&
                   (C == other.A || C == other.B || C == other.C);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() + B.GetHashCode() + C.GetHashCode();
        }
    };
}