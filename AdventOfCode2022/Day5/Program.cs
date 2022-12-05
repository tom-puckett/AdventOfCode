namespace Day5
{
    internal class MoveInstruction
    {
        internal MoveInstruction(int howmany, int from, int to)
        {
            HowMany= howmany;
            From= from;
            To= to;
        }

        internal int HowMany { get; set; }
        internal int From { get; set; }
        internal int To { get; set; }
    }

    internal class Program
    {
        private static Dictionary<int, Stack<char>> Stacks = new();
        private static List<MoveInstruction> MoveInstructions = new List<MoveInstruction>();

        static void Main(string[] args)
        {
            #region Part 1
            ReadData("Data.txt");

            foreach (MoveInstruction move in MoveInstructions)
            {
                for (int i=0; i<move.HowMany; i++)
                {
                    char thingToMove = Stacks[move.From].Pop();
                    Stacks[move.To].Push(thingToMove);
                }
            }

            string result1 = Stacks.OrderBy(s => s.Key).Aggregate(string.Empty, (result, next) => result + next.Value.Peek());
            #endregion

            #region Part 2
            ReadData("Data.txt");

            foreach (MoveInstruction move in MoveInstructions)
            {
                Stack<char> craneBucket = new Stack<char>();
                for (int i = 0; i<move.HowMany; i++)
                {
                    char thingToMove = Stacks[move.From].Pop();
                    craneBucket.Push(thingToMove);
                }
                for (int i = 0; i<move.HowMany; i++)
                {
                    char thingToMove = craneBucket.Pop();
                    Stacks[move.To].Push(thingToMove);
                }
            }

            string result2 = Stacks.OrderBy(s => s.Key).Aggregate(string.Empty, (result, next) => result + next.Value.Peek());
            #endregion

            Console.WriteLine($"Part 1 result is {result1}");
            Console.WriteLine($"Part 2 result is {result2}");
        }

        static void ReadData(string fileName)
        {
            Stacks.Clear();
            MoveInstructions.Clear();
            
            string? line;
            using TextReader fileReader = new StreamReader(fileName);

            List<string> stackLines = new List<string>();
            for (; ; )
            {
                line = fileReader.ReadLine();
                if (line == null || !line.Contains('[')) break;
                stackLines = stackLines.Prepend(line).ToList();
            }

            List<int> stackNumbers = line?.Split(' ', StringSplitOptions.RemoveEmptyEntries)?.Select(n => int.Parse(n))?.ToList() ?? new List<int>();
            stackNumbers.ForEach(n => Stacks.Add(n, new Stack<char>()));
            foreach (string stackLine in stackLines)
            {
                for (int stackNumberIndex = 0; stackNumberIndex < stackNumbers.Count; stackNumberIndex++)
                {
                    char crate = stackLine[stackNumberIndex*4 +1];
                    if (crate != ' ')
                    {
                        Stacks[stackNumbers[stackNumberIndex]].Push(crate);
                    }
                }
            }
            line = fileReader.ReadLine();  // empty line

            while (fileReader.Peek() != -1)
            {
                line = fileReader.ReadLine();
                if (line == null) break;
                string[] words = line.Split(' ');
                if (words[0] == "move")
                {
                    MoveInstructions.Add(new MoveInstruction(int.Parse(words[1]), int.Parse(words[3]), int.Parse(words[5])));
                }
            }
        }
    }
}