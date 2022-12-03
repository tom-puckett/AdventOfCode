using Microsoft.VisualBasic;

namespace Day3
{
    internal class Program
    {
        static string allItemsByPriority = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static void Main(string[] args)
        {
            List<(string compartment1, string compartment2)> rucksackList = ReadData("Inventory.txt");

            #region Part 1
            int part1TotalPriorities = 0;
            foreach (var rucksack in rucksackList)
            {
                char intersection = rucksack.compartment1.Intersect(rucksack.compartment2).Single();
                part1TotalPriorities += GetPriority(intersection);
            }
            Console.WriteLine($"Total of all priorities for part 1 is {part1TotalPriorities}");
            #endregion

            #region Part 2
            int part2TotalPriorities = 0;
            for (int readCount = 0; readCount < rucksackList.Count; readCount += 3)
            {
                List<string> elfGroup = new();
                for (int i = 0; i < 3; i++)
                {
                    var rucksack = rucksackList[readCount+i];
                    elfGroup.Add(rucksack.compartment1 + rucksack.compartment2);
                }

                char intersection = elfGroup[0].Intersect(elfGroup[1]).Intersect(elfGroup[2]).Single();
                part2TotalPriorities += GetPriority(intersection);
            }
            Console.WriteLine($"Total of all priorities for part 2 is {part2TotalPriorities}");
            #endregion
        }

        internal static List<(string,string)> ReadData(string path = "")
        {

            List<(string, string)> returnList = new();

            IEnumerable<string> dataSource = 
                path == ""
                ? new List<string> { "vJrwpWtwJgWrhcsFMMfFFhFp", "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL" }
                : File.ReadLines(path);

            foreach (string line in dataSource.Where(ln => !string.IsNullOrWhiteSpace(ln)))
            {
                string trimmedLine = line.Trim();
                int trimmedLineLength = trimmedLine.Length;

                returnList.Add((trimmedLine.Substring(0,trimmedLineLength/2), trimmedLine.Substring(trimmedLineLength/2)));
            }

            return returnList;
        }

        internal static int GetPriority(char c)
        {
            return allItemsByPriority.IndexOf(c) + 1;
        }
    }
}