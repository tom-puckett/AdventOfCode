using System.ComponentModel.Design.Serialization;

namespace Day7
{
    internal class Program
    {

        static void Main(string[] args)
        {
            FileSystem fs = new FileSystem();

            string lastCommand = string.Empty;
            foreach (string line in System.IO.File.ReadLines("InputData.txt"))
            {
                string[] lineTokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (lineTokens[0] == "$")
                {
                    lastCommand = lineTokens[1];
                    switch (lineTokens[1])
                    {
                        case "cd":
                            fs.CurrentDirectory = fs.cd(lineTokens.Length >= 3 ? lineTokens[2] : "");
                            break;
                        case "ls":
                            fs.ls();
                            break;
                    }
                }
                else if (lastCommand == "ls" && long.TryParse(lineTokens[0], out long Size)) 
                {
                    if (!fs.CurrentDirectory.entries.OfType<File>().Any(f => f.Name == lineTokens[1]))
                    {
                        fs.CurrentDirectory.AddNewEntry(new File(lineTokens[1], Size));
                    }
                }
                else if (lastCommand == "ls" && lineTokens[0] == "dir")
                {
                    if (!fs.CurrentDirectory.entries.OfType<Directory>().Any(f => f.Name == lineTokens[1]))
                    {
                        fs.CurrentDirectory.AddNewEntry(new Directory(lineTokens[1]));
                    }
                }
            }

            Dictionary<string, long> allDirSizes = ScanAllDirectorySizes(fs.Root);
            var howManyDistinctDirectories = allDirSizes.Select(d => d.Key).Distinct().Count();

            #region Part 1
            Dictionary<string, long> smallDirSizes = allDirSizes.Where(d => d.Value <= 100_000).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Console.WriteLine($"There are {smallDirSizes.Select(d => d.Key).Distinct().Count()} small directories with total size {smallDirSizes.Sum(d => d.Value)}");
            #endregion

            #region Part 2
            long currentEmptySpace = 70_000_000 - fs.Root.GetContainedFilesSize();
            long requiredAdditionalSpace = 30_000_000 - currentEmptySpace;

            KeyValuePair<string, long> selectedDir = allDirSizes.OrderBy(d => d.Value).First(d => d.Value > requiredAdditionalSpace);
            Console.WriteLine($"Directory {selectedDir.Key} is the smallest directory to delete with size {selectedDir.Value}");
            #endregion
        }

        internal static Dictionary<string, long> ScanAllDirectorySizes(Directory startHere)
        {
            Dictionary<string, long> returnList = new()
            {
                { startHere.GetPath(), startHere.GetContainedFilesSize() }
            };
            foreach (Directory dir in startHere.entries.OfType<Directory>())
            {
                returnList = returnList.Union(ScanAllDirectorySizes(dir)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            return returnList;
        }
    }

    internal class FileSystem
    {
        internal Directory Root { get; init; } = new Directory("");
        internal Directory CurrentDirectory;

        internal FileSystem()
        {
            CurrentDirectory = Root;
        }
        internal void ls()
        { }

        internal Directory cd(string target)
        {
            return target switch
            {
                string t when string.IsNullOrWhiteSpace(t) => CurrentDirectory,
                "/" => Root,
                ".." => CurrentDirectory.parentDirectory ?? Root,
                _ => CurrentDirectory.entries.OfType<Directory>().Single(e => e.Name == target)
            };
        }

    }

    internal abstract class DirectoryEntry
    {
        internal string Name { get; set; }
        internal Directory? parentDirectory = default;

        internal DirectoryEntry(string name)
        {
            Name=name;
        }

        internal string GetPath()
        {
            string returnVal = $"/{Name}";
            for (Directory? parent = parentDirectory; parent is not null && parent.parentDirectory is not null; parent = parent.parentDirectory)
            {
                returnVal = $"/{parent.Name}{returnVal}";
            }
            // returnVal = $"/{returnVal}";
            return returnVal;
        }
    }

    internal class File : DirectoryEntry
    {
        internal long Size { get; set; }

        internal File(string name, long size)
            : base(name)
        {
            Size=size;
        }
    }

    internal class Directory : DirectoryEntry
    {
        internal List<DirectoryEntry> entries = new();

        internal Directory(string name)
            : base(name)
        { }

        internal long GetContainedFilesSize()
        {
            long returnVal = 0;
            foreach (DirectoryEntry entry in entries)
            {
                returnVal +=
                    entry switch
                    {
                        var t when t is File => ((File)entry).Size,
                        var t when t is Directory => ((Directory)entry).GetContainedFilesSize(),
                        _ => throw new NotImplementedException()
                    };
            }

            return returnVal;
        }

        internal void AddNewEntry(DirectoryEntry entry)
        {
            entry.parentDirectory = this;
            entries.Add(entry);
        }

    }

}