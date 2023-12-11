namespace Day01;

internal class Part2
{
    internal void Count(bool verbose = false)
    {
        int sum = 0;

        foreach (string line in File.ReadAllLines("InputData1.txt"))
        {
            int calibrationValue = GetNumberFromDigits(new int[] { GetToken(line, true), GetToken(line, false) });
            sum += calibrationValue;
            if (verbose)
            {
                Console.WriteLine("Line is <{0}>, value is {1}, sum is {2}", line, calibrationValue, sum);
            }
        }

        Console.WriteLine("Part 2 sum for all input data is {0}", sum);
    }

    private int GetToken(string line, bool getFirst)
    {
        Dictionary<string, int> tokens = new Dictionary<string, int>
        {
            {"0", 0}, {"zero", 0},
            {"1", 1}, {"one", 1},
            {"2", 2}, {"two", 2},
            {"3", 3}, {"three", 3},
            {"4", 4}, {"four", 4},
            {"5", 5}, {"five", 5},
            {"6", 6}, {"six", 6},
            {"7", 7}, {"seven", 7},
            {"8", 8}, {"eight", 8},
            {"9", 9}, {"nine", 9}
        };

        if (getFirst)
        {
            (int index, string token) firstToken = tokens.Aggregate(
                (int.MaxValue, ""),
                (best, element) =>
                {
                    int index = line.IndexOf(element.Key);
                    return index < best.Item1 && index > -1
                        ? (index, element.Key) 
                        : best;
                });
            return tokens[firstToken.token];
        }
        else
        {
            (int, string) lastToken = tokens.Aggregate(
                (-1, ""),
                (best, element) =>
                {
                    int index = line.LastIndexOf(element.Key);
                    return (index > best.Item1 && index > -1 ? (index, element.Key) : best);
                });
            return tokens[lastToken.Item2];
        }
    }

    private int GetNumberFromDigits(IEnumerable<int> HighestToLowest)
    {
        int result = HighestToLowest.Aggregate(0, (sum, element) => sum * 10 + element);
        return result;
    }
}
