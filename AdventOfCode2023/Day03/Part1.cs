namespace Day03;

public class Part1
{
    static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public void Go(bool verbose)
    {
        int sum = 0;
        string[] inputData = File.ReadAllLines("InputData1.txt");

        for (int row = 0; row < inputData.Length; row++)
        {
            string line = inputData[row];
            List<(int index, int length, string token)> tokens = GetTokensFromLine(line);

            if (verbose) Console.WriteLine($"In row {row}, {tokens.Count} tokens are: {(string.Join(' ', tokens.Select(t => t.token)))}");

            foreach ((int index, int length, string token) token in tokens)
            {
                (int r, int c) ul = (Math.Max(row-1, 0), Math.Max(token.index-1, 0));
                (int r, int c) lr = (Math.Min(row+1, inputData.Length - 1), Math.Min(token.index+token.length, line.Length-1));
                if (verbose) Console.WriteLine($"ul: ({ul.r},{ul.c}), lr: ({lr.r},{lr.c})");

                for (int r = ul.r; r <= lr.r; r++) 
                {
                    for (int c = ul.c; c <= lr.c; c++)
                    {
                        char testChar = inputData[r][c];
                        if (testChar != '.' && !digits.Contains(testChar))
                        {
                            if (verbose) Console.WriteLine($"Token {token.token} is a part number because it's near '{testChar}' at ({r},{c})");
                            sum += int.Parse(token.token);
                            r = lr.r + 1;  // break out of outer loop too
                            break;
                        }
                    }
                }
            }
        }

        Console.WriteLine("Part 1 sum is " + sum);
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
}
