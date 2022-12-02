public class Program 
{
    public static async Task Main(string[] args)
    {
        Dictionary<int, List<int>> ElfItems = ReadData("CaloriesCarried.txt");
        List<(int elfNumber, int calories)> elfTotals = ElfItems.Select(e => (e.Key, e.Value.Sum())).ToList();

        #region Part 1
        (string logString, int calories) answer = ("", 0);
        foreach ((int elfNumber, int calories) elf in elfTotals)
        {
            if (elf.calories > answer.calories)
            {
                answer = ($"Elf {elf.elfNumber} is carrying the most calories ({elf.calories})", elf.calories);
            }
        }
        Console.WriteLine(answer.logString);
        #endregion

        #region Part 2
        var top3Elves = elfTotals.OrderByDescending(e => e.calories).Take(3);
        Console.WriteLine($"Top 3 elves are:");
        int totalCalories = 0;

        foreach (var elf in top3Elves)
        {
            totalCalories += elf.calories;
            Console.WriteLine($"  Elf {elf.elfNumber} with {elf.calories} calories");
        }
        Console.WriteLine($"Total of these 3 is {totalCalories} calories");
        #endregion
    }

    public static Dictionary<int, List<int>> ReadData(string fileName)
    {
        Dictionary<int, List<int>> ElfCalories = new Dictionary<int, List<int>>();
        int elfNumber = 0;
        List<int> currentElf = new List<int>();

        foreach (string line in File.ReadLines(fileName))
        {
            if (int.TryParse(line, out int thisItem))
            {
                currentElf.Add(thisItem);
            }
            else if (currentElf.Any())
            {
                ElfCalories.Add(++elfNumber, currentElf);
                currentElf = new List<int>();
            }
        }

        if (currentElf.Any())
        {
            ElfCalories.Add(++elfNumber, currentElf);
        }

        return ElfCalories;
    }
}


