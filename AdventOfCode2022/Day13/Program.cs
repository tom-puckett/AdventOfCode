using System;
using System.IO;
using System.Text;

namespace Day13
{
    internal class Program
    {
        internal static Dictionary<int, (string left, string right)> rawData = new();

        static void Main(string[] args)
        {
            ReadData("InputData.txt");
            //ReadData("TestData.txt");

            List<int> validPairs = new List<int>();
            List<int> invalidPairs = new List<int>();
            List<int> equalPairs = new List<int>();

            PacketComparer myComparer = new PacketComparer();
            int sum = 0;
            foreach (KeyValuePair<int, (string left, string right)> item in rawData)
            {
                int result = myComparer.Compare(item.Value.left, item.Value.right);
                // Console.WriteLine($"{item.Value.left} compared with {item.Value.right} is {result}");

                switch (result)
                {
                    case 0:
                        equalPairs.Add(item.Key);
                        break;
                    case 1:
                        invalidPairs.Add(item.Key);
                        break;
                    case -1:
                        sum += item.Key;
                        validPairs.Add(item.Key);
                        break;

                }
                // Console.WriteLine($"Pair {item.Key}: left {(result switch { -1 => "<", 0 => "=", 1 => ">" } )} right");
            }
            Console.WriteLine($"Part 1: Sum of all valid indexes is {sum}");

            SortedSet<string> sortedSet = new SortedSet<string>(new PacketComparer());
            foreach (var pair in rawData)
            {
                sortedSet.Add(pair.Value.left);
                sortedSet.Add(pair.Value.right);
                sortedSet.Add("[[2]]");
                sortedSet.Add("[[6]]");
            }
            List<int> dividerIndexes = new();
            for (int index = 0; index < sortedSet.Count ; index++)
            {
                string message = sortedSet.ElementAt(index);
                if (new[] { "[[2]]", "[[6]]" }.Contains(message))
                {
                    dividerIndexes.Add(index+1);
                }
            }

            int decoderValue = dividerIndexes[0] * dividerIndexes[1];
            Console.WriteLine($"Part 2: Decoder value is {decoderValue}");
        }

        internal static void ReadData(string fileName)
        {
            rawData = new();

            using StreamReader reader = new StreamReader(fileName);
            for (int pairCounter = 1; ; pairCounter++)
            {
                if (reader.EndOfStream)
                {
                    break;
                }

                rawData.Add(pairCounter, (reader.ReadLine() ?? "", reader.ReadLine() ?? ""));
                reader.ReadLine();
            }
        }
    }

    public class PacketComparer : Comparer<string>
    {
        public override int Compare(string? left, string? right)
        {
            switch ((left,right))
            {
                case (string l, string r) when string.IsNullOrWhiteSpace(l) && string.IsNullOrWhiteSpace(r):
                    // Console.WriteLine($"Both left and right contain nothing");
                    return 0;

                case (string l, string) when string.IsNullOrWhiteSpace(l):
                    // Console.WriteLine($"Left contains nothing");
                    return -1;

                case (string, string r) when string.IsNullOrWhiteSpace(r):
                    // Console.WriteLine($"Right contains nothing");
                    return 1;

                case (string l, string r) when int.TryParse(l, out int iLeft) && int.TryParse(r, out int iRight):
                    return iLeft.CompareTo(iRight);

                default:  // Both are array
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (int.TryParse(left, out _)) left = $"[{left}]";
                    List<string> leftTokens = TokenizeArray(left);

                    if (int.TryParse(right, out _)) right = $"[{right}]";
                    List<string> rightTokens = TokenizeArray(right);

                    for (int tokenCounter = 0; ; tokenCounter++)
                    {
                        if (tokenCounter == leftTokens.Count && tokenCounter == rightTokens.Count)
                        {
                            //Console.WriteLine($"{left} and {right} are equal");
                            return 0;
                        }
                        if (tokenCounter == leftTokens.Count)
                        {
                            // Console.WriteLine($"Left side ran out of values");
                            return -1;
                        }
                        if (tokenCounter == rightTokens.Count)
                        {
                            // Console.WriteLine($"Right side ran out of values");
                            return 1;
                        }
                        int tokenComparison = Compare(leftTokens.ElementAt(tokenCounter), rightTokens.ElementAt(tokenCounter));
                        // Console.WriteLine($"{leftTokens.ElementAt(tokenCounter)} compared with {rightTokens.ElementAt(tokenCounter)} is {tokenComparison}");
                        if (tokenComparison != 0)
                        {
                            return tokenComparison;
                        }
                    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        public List<string> TokenizeArray(string originalValue)
        {
            List<string> tokens = new List<string>();

            StringBuilder tokenBuilder = new StringBuilder();
            string interiorValue = originalValue.Substring(1, originalValue.Length - 2);

            for (int index = 0, depth = 0; index < interiorValue.Length; index++)
            {
                tokenBuilder.Append(interiorValue[index]);

                switch (interiorValue[index]) {
                    case char c when c == ',' && depth == 0:
                        tokenBuilder.Remove(tokenBuilder.Length - 1, 1);
                        tokens.Add(tokenBuilder.ToString());
                        tokenBuilder.Clear();
                        break;
                    case char _ when index == interiorValue.Length-1:
                        tokens.Add(tokenBuilder.ToString());
                        break;
                    case '[':
                        depth++;
                        break;
                    case ']':
                        depth--;
                        break;
                }
            }

            return tokens;
        }
    }
}