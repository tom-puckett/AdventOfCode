namespace Day4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<(Range elf1, Range elf2)> elfPairList = ReadData("InputData.txt");

            int fullOverlapCount = elfPairList.Count(p => p.elf1.FullyContains(p.elf2) || p.elf2.FullyContains(p.elf1));
            int anyOverlapCount = elfPairList.Count(p => p.elf1.HasAnyOverlapWith(p.elf2));

            Console.WriteLine($"Part 1 overlap count is {fullOverlapCount}");
            Console.WriteLine($"Part 2 overlap count is {anyOverlapCount}");
        }

        static List<(Range,Range)> ReadData(string fileName)
        {
            List<(Range elf1, Range elf2)> returnList = new List<(Range elf1, Range elf2)>();

            foreach (string line in File.ReadAllLines(fileName))
            {
                string[] assignmentPair = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Range elf1 = new Range(int.Parse(assignmentPair[0].Split('-')[0]), int.Parse(assignmentPair[0].Split('-')[1]));
                Range elf2 = new Range(int.Parse(assignmentPair[1].Split('-')[0]), int.Parse(assignmentPair[1].Split('-')[1]));

                returnList.Add((elf1, elf2));
            }

            return returnList;
        }
    }
}

namespace System
{
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
}