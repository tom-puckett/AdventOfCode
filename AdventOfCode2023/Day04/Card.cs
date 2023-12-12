namespace Day04;

internal class Card
{
    internal int CardNumber { get; set; }
    internal List<int> WinningNumbers { get; set; } = new();
    internal List<int> HaveNumbers { get; set; } = new();
}
