namespace Day04
{
    internal class Part1
    {
        bool _verbose;

        internal Part1(bool verbose = false)
        {
            _verbose = verbose;
        }

        internal void Go()
        {
            int totalScore = 0;

            foreach (string line in File.ReadAllLines("InputData1.txt"))
            {
                Card card = ParseInput(line);
                int score = GetCardScore(card);
                totalScore += score;
            }

            Console.WriteLine(totalScore);
        }

        internal Card ParseInput(string line)
        {
            Card returnVal = new();

            string[] lineSplit1 = line.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] cardSplit = lineSplit1[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] numberSplit = lineSplit1[1].Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] winningNumberSplit = numberSplit[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] haveNumberSplit = numberSplit[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            returnVal.CardNumber = int.Parse(cardSplit[1]);
            returnVal.WinningNumbers = winningNumberSplit.Select(w => int.Parse(w)).ToList();
            returnVal.HaveNumbers = haveNumberSplit.Select(h => int.Parse(h)).ToList();

            return returnVal;
        }

        internal int GetCardScore(Card card)
        {
            int matchCount = card.HaveNumbers.Intersect(card.WinningNumbers).Count();

            int score = matchCount switch
            {
                0 => 0,
                _ => (int)Math.Pow(2, matchCount-1)
            };

            if (_verbose) Console.WriteLine($"Card has {matchCount} matching numbers, score is {score}");

            return score;
        }
    }
}
