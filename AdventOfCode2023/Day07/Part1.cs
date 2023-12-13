namespace Day07;

internal class Part1
{
    private static Dictionary<char, int> Denominations => new Dictionary<char, int>
    {
        { 'A', 0},
        { 'K', 1},
        { 'Q', 2},
        { 'J', 3},
        { 'T', 4},
        { '9', 5},
        { '8', 6},
        { '7', 7},
        { '6', 8},
        { '5', 9},
        { '4', 10},
        { '3', 11},
        { '2', 12},
    };

    bool _verbose;
    Game game = new Game();

    internal Part1(bool verbose = false)
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

        Console.WriteLine("Part 1 score is " + totalScore);
    }

    private Hand ParseCardInput(string line)
    {
        string[] lineSplit = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return new Hand(Denominations) { Cards = lineSplit[0].ToCharArray(), bid = int.Parse(lineSplit[1]) };
    }
}
