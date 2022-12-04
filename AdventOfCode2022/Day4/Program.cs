List<(Range elf1, Range elf2)> elfPairList = File.ReadAllLines("InputData.txt").Aggregate(new List<(Range elf1, Range elf2)>(), (list, next) =>
    {
        string[] assignmentPair = next.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        list.Add((new Range(int.Parse(assignmentPair[0].Split('-')[0]), int.Parse(assignmentPair[0].Split('-')[1])),
                  new Range(int.Parse(assignmentPair[1].Split('-')[0]), int.Parse(assignmentPair[1].Split('-')[1]))));
        return list;
    });

int fullOverlapCount = elfPairList.Count(p => p.elf1.FullyContains(p.elf2) || p.elf2.FullyContains(p.elf1));
int anyOverlapCount = elfPairList.Count(p => p.elf1.HasAnyOverlapWith(p.elf2));

Console.WriteLine($"Part 1 overlap count is {fullOverlapCount}");
Console.WriteLine($"Part 2 overlap count is {anyOverlapCount}");

public static class RangeExtensions
{
    public static bool FullyContains(this Range me, Range other)
    {
        return (me.Start.Value <= other.Start.Value && me.End.Value >= other.End.Value) ||
                (other.Start.Value <= me.Start.Value && other.End.Value >= me.End.Value);
    }
    public static bool HasAnyOverlapWith(this Range me, Range other)
    {
        return me.FullyContains(other) || other.FullyContains(me) ||
                (me.Start.Value >= other.Start.Value && me.Start.Value <= other.End.Value) ||
                (me.End.Value >= other.Start.Value && me.End.Value <= other.End.Value);
    }
}
