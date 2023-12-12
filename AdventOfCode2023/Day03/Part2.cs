using System.Collections.Generic;

namespace Day03;

public class Part2
{
    static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public void Go(bool verbose)
    {
        int sum = 0;
        string[] inputData = File.ReadAllLines("InputData1.txt");

        for (int row = 0; row < inputData.Length; row++)
        {
            string line = inputData[row];
            List<int> starIndexes = GetStarIndexesFromLine(line);

            if (verbose) Console.WriteLine($"In row {row} '*' characters are at index: {string.Join(", ", starIndexes)}");

            List<(int index, int length, string token)> tokensInNeighboringLines = new();

            for (int r = Math.Max(row-1, 0); r <= Math.Min(row+1, inputData.Length-1); r++)
            {
                tokensInNeighboringLines.AddRange(GetTokensFromLine(inputData[r]));
            }

            foreach (int starIndex in starIndexes)
            {
                List<int> neighboringTokens = tokensInNeighboringLines.Aggregate(
                    new List<int>(),
                    (accum, token) =>
                    {
                        for (int c = token.index; c < token.index + token.length; c++)
                        {
                            if (Math.Abs(c - starIndex) <= 1)
                            {
                                accum.Add(int.Parse(token.token));
                                break;
                            }
                        }
                        return accum;
                    }
                    );

                if (verbose) Console.WriteLine($"At starIndex {starIndex}, found numbers {string.Join(", ", neighboringTokens)}");
                if (neighboringTokens.Count > 1)
                {
                    sum += neighboringTokens.Aggregate(1, (s, t) => s * t);
                }
            }
        }

        Console.WriteLine("Part 2 sum is " + sum);
    }

    private List<(int index, int length, string token)> GetTokensFromLine(string line)
    {
        int searchPosition = 0;
        List<(int index, int length, string token)> result = new();

        do
        {
            (int index, int length, string token) newToken = (-1, 0, string.Empty);

            newToken.index = line.IndexOfAny(digits, searchPosition);
            if (newToken.index < 0) break;

            while (newToken.index + newToken.length < line.Length && digits.Contains(line[newToken.index + newToken.length]))
            {
                newToken.token += line[newToken.index + newToken.length++];
            }
            searchPosition = newToken.index + newToken.length;
            result.Add(newToken);
        }
        while (searchPosition < line.Length);

        return result;
    }

    private List<int> GetStarIndexesFromLine(string line)
    {
        List<int> result = new();

        for (int c = 0; c < line.Length; c++)
        {
            if (line[c] == '*')
            {
                result.Add(c);
            }
        }

        return result;
    }
}
