namespace Day07;

internal class Part2
{
    private static Dictionary<char, int> Denominations => new Dictionary<char, int>
    {
        { 'A', 0},
        { 'K', 1},
        { 'Q', 2},
        { 'T', 3},
        { '9', 4},
        { '8', 5},
        { '7', 6},
        { '6', 7},
        { '5', 8},
        { '4', 9},
        { '3', 10},
        { '2', 11},
        { 'J', 12},
    };

    Func<IEnumerable<KeyValuePair<char, int>>, IEnumerable<KeyValuePair<char, int>>> JacksAreWild = counts =>
    {
        if (!counts.Any(kvp => kvp.Key == 'J')) return counts;

        var countsDict = new Dictionary<char, int>(counts);

        int numberJacks = countsDict.Single(kvp => kvp.Key == 'J').Value;
        char mostOccurringDenomination = countsDict.All(kvp => kvp.Key == 'J') 
            ? 'J'
            : countsDict.Where(kvp => kvp.Key != 'J').MaxBy(kvp => kvp.Value).Key;
        countsDict[mostOccurringDenomination] += numberJacks;
        countsDict['J'] -= numberJacks;

        return countsDict.AsEnumerable();
    };

    private bool _verbose;
    Game game = new Game();

    internal Part2(bool verbose)
    {
        _verbose = verbose;
    }

    internal void Go()
    {
        int totalScore = 0;

        foreach (string line in File.ReadAllLines("InputData1.txt"))
        {
            Hand hand = ParseCardInput(line);
            var x = hand.Kind;
            game.Hands.Add(hand);
        }

        int handCounter = 0;
        foreach (Hand hand in game.Hands.Order())
        {
            totalScore += ++handCounter * hand.bid;
        }

        Console.WriteLine("Part 2 score is " + totalScore);
    }

    private Hand ParseCardInput(string line)
    {
        string[] lineSplit = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return new Hand(Denominations) { Cards = lineSplit[0].ToCharArray(), bid = int.Parse(lineSplit[1]), CountsTransformer = JacksAreWild };
    }
}
