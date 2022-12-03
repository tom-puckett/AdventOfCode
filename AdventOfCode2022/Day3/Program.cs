using Microsoft.VisualBasic;

namespace Day3
{
    internal class Program
    {
        static string allItemsByPriority = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static void Main(string[] args)
        {
            List<(string compartment1, string compartment2)> returnList = ReadData("Inventory.txt");

            int totalPriorities = 0;
            foreach (var rucksack in returnList)
            {
                char intersection = rucksack.compartment1.Intersect(rucksack.compartment2).Single();
                int priority = GetPriority(intersection);
                totalPriorities += priority;
                Console.WriteLine($"{intersection}, {priority}");
            }
            Console.WriteLine($"Total of all priorities is {totalPriorities}");
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