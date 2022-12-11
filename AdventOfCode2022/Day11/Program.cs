namespace Day11
{
    internal class Program
    {
        internal static List<Monkey> monkeys= new List<Monkey>();
        internal static HashSet<uint> uniqueDivisors = new HashSet<uint>();
        internal static ulong modulus = 0;

        static void Main(string[] args)
        {
            foreach ((int iterations, bool getBored) in new(int,bool)[]{ (20, true), (10_000, false) })
            {
                ReadInput("InputData.txt");
                modulus = uniqueDivisors.Aggregate(1u, (mod, next) => mod * next);

                for (int i = 1; i<=iterations; i++)
                {
                    monkeys.ForEach(m => m.TakeTurn(getBored));

                    if (new[] { 1, 20, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10_000 }.Contains(i))
                    {
                        Console.WriteLine($"Round {i}:");
                        monkeys.ForEach(m => Console.WriteLine($"Monkey {m.number} inspected {m.inspectionCount} items, max is {(m.Items.Any()? m.Items.Max() : 0)}, list is {string.Join(",",m.Items)}"));
                        Console.WriteLine($"Total monkey business is {monkeys.OrderByDescending(m => m.inspectionCount).Take(2).Aggregate(1L, (prod, next) => prod * next.inspectionCount)}");
                    }
                }
            }
        }

        static void ReadInput(string fileName)
        {
            monkeys= new List<Monkey>();
            int MonkeyNumber = -1;
            foreach (string line in File.ReadLines(fileName)) 
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] tokens = line.Trim(' ', ':').Split(' ', StringSplitOptions.RemoveEmptyEntries);

                switch (tokens[0])
                {
                    case "Monkey":
                        MonkeyNumber = int.Parse(tokens[1]);
                        monkeys.Add(new Monkey(MonkeyNumber));
                        continue;

                    case "Starting":
                        monkeys[MonkeyNumber].Items = tokens.Where(t => int.TryParse(t.Trim(','), out _)).Select(i => ulong.Parse(i.Trim(','))).ToList();
                        break;

                    case "Operation:":
                        if (uint.TryParse(tokens[5], out uint operand))
                        {
                            monkeys[MonkeyNumber].operationFunc = num => tokens[4] switch
                            {
                                "*" => num * operand,
                                "+" => num + operand,
                                "/" => num / operand,
                                "-" => num - operand,
                                _ => throw new NotImplementedException()
                            };
                        }
                        else if (tokens[5] == "old")
                        {
                            monkeys[MonkeyNumber].operationFunc = num => num * num;
                        }
                        break;

                    case "Test:":
                        uint denominator = uint.Parse(tokens.Last());
                        uniqueDivisors.Add(denominator);
                        monkeys[MonkeyNumber].testFunc = item => item % denominator == 0;
                        break;

                    case "If":
                        switch (tokens[1])
                        {
                            case "true:":
                                monkeys[MonkeyNumber].monkeyIfTestTrue = int.Parse(tokens.Last());
                                break;
                            case "false:":
                                monkeys[MonkeyNumber].monkeyIfTestFalse = int.Parse(tokens.Last());
                                break;
                        }
                        break;

                }

            }
        }
    }

    internal class Monkey
    {
        internal List<ulong> Items = new List<ulong>();
        internal int number;
        internal long inspectionCount = 0;
        internal Func<ulong, ulong>? operationFunc;
        internal Func<ulong, bool>? testFunc;
        internal int monkeyIfTestTrue = -1;
        internal int monkeyIfTestFalse = -1;

        internal Monkey(int number)
        {
            this.number = number;
        }

        internal void TakeTurn(bool relax)
        {
            if (operationFunc is null || testFunc is null) throw new ApplicationException();

            InspectItems(operationFunc);
            if (relax)
            {
                GetBored();
            }
            Test(testFunc);
        }

        internal void InspectItems(Func<ulong, ulong> operation)
        {
            Items = Items.Select(operation).Select(i => i % Program.modulus).ToList();
            inspectionCount += Items.Count;
        }

        internal void GetBored()
        {
            Items = Items.Select(i => i/3).ToList();
        }

        internal void Test(Func<ulong, bool> test)
        {
            Items.ForEach(i =>
            {
                if (test(i))
                {
                    Program.monkeys.Single(m => m.number == monkeyIfTestTrue).CatchAnItem(i);
                }
                else
                {
                    Program.monkeys.Single(m => m.number == monkeyIfTestFalse).CatchAnItem(i);
                }
            });
            Items.Clear();
        }

        internal void CatchAnItem(ulong item)
        {
            Items.Add(item);
        }
    }
}