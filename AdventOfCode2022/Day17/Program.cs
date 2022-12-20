using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
 
namespace Day17
{
    public enum JetDirection
    {
        Right,
        Left
    }

    internal class Program
    {
        internal static char[][] Chamber = Array.Empty<char[]>();

        internal static char[][] Rock0 = { new char[]{ '@', '@', '@', '@' } };

        internal static char[][] Rock1 = { new char[]{ '.', '@', '.' },
                                           new char[]{ '@', '@', '@' },
                                           new char[]{ '.', '@', '.' } };

        internal static char[][] Rock2 = { new char[]{ '@', '@', '@' },
                                           new char[]{ '.', '.', '@' },
                                           new char[]{ '.', '.', '@' } };

        internal static char[][] Rock3 = { new char[]{ '@' },
                                           new char[]{ '@' },
                                           new char[]{ '@' },
                                           new char[]{ '@' } };

        internal static char[][] Rock4 = { new char[]{ '@', '@' },
                                           new char[]{ '@', '@' } };

        static void Main(string[] args)
        {
            JetDirection[] Jets = Array.Empty<JetDirection>();
            bool Test = false;
            if (Test)
            {
                Jets = File.ReadAllText("TestData.txt")
                           .Select(c => c switch { '>' => JetDirection.Right, '<' => JetDirection.Left, _ => throw new ApplicationException() })
                           .ToArray();
            }
            else
            {
                Jets = File.ReadAllText("InputData.txt")
                           .Select(c => c switch { '>' => JetDirection.Right, '<' => JetDirection.Left, _ => throw new ApplicationException() })
                           .ToArray();
            }

            foreach ((int part, long totalRuns) iteration in new[] {(1,2022), (2,1_000_000_000_000L)})
            {
                long groupAddCounter = 0;
                Chamber = Array.Empty<char[]>();

                int rockIndex = 0;
                int jetIndex = 0;
                long addedRowCount = 0;
                Stopwatch stopwatch = Stopwatch.StartNew();
                int[][] HighestRockHistoryInEachColumn = new int[0][];
                (int start, int number)[] jetHistory = new (int, int)[0];

                for (long rockCount = 1L; rockCount <= iteration.totalRuns; rockCount++, rockIndex = ++rockIndex % 5)
                {
                    // 1 place new rock
                    char[][] rockInstance = GetRockForIndex(rockIndex);
                    (int r, int c) currentRockLowerLeft = (Chamber.Length + 3, 2);

                    //Console.WriteLine($"Rock {rockCount}[{rockCount-1}]: type {rockIndex} placed at ({currentRockLowerLeft.r},{currentRockLowerLeft.c})");

                    jetHistory = jetHistory.Append((jetIndex, 0)).ToArray();
                    for (; ; )
                    {
                        // 1 apply jet
                        jetHistory[jetHistory.Length-1].number++;

                        switch (Jets[jetIndex])
                        {
                            case JetDirection.Left:
                                if (currentRockLowerLeft.c > 0 &&   // Ensure rock is not at left edge of chamber
                                    MoveIsAllowed(rockInstance, (currentRockLowerLeft.r, currentRockLowerLeft.c - 1)))
                                {
                                    currentRockLowerLeft.c--;
                                    //Console.WriteLine($"Moved left to ({currentRockLowerLeft.r},{currentRockLowerLeft.c})");
                                }
                                else
                                {
                                    //Console.WriteLine("No move left");
                                }
                                break;

                            case JetDirection.Right:
                                if (currentRockLowerLeft.c + rockInstance[0].Length < 7 &&   // Ensure rock is not at right edge of chamber   
                                    MoveIsAllowed(rockInstance, (currentRockLowerLeft.r, currentRockLowerLeft.c + 1)))
                                {
                                    currentRockLowerLeft.c++;
                                    //Console.WriteLine($"Moved right to ({currentRockLowerLeft.r},{currentRockLowerLeft.c})");
                                }
                                else
                                {
                                    //Console.WriteLine("No move right");
                                }
                                break;
                        }
                        jetIndex = ++jetIndex % Jets.Length;

                        // 2 if rock can move down, rock falls
                        if (MoveIsAllowed(rockInstance, (currentRockLowerLeft.r-1, currentRockLowerLeft.c)))
                        {
                            currentRockLowerLeft.r--;
                            //Console.WriteLine($"Moved down to ({currentRockLowerLeft.r},{currentRockLowerLeft.c})");
                        }
                        else   // rock comes to rest
                        {
                            for (int r = 0; r < rockInstance.Length; r++)
                            {
                                if (Chamber.Length <= currentRockLowerLeft.r + rockInstance.Length - 1)
                                {
                                    char[] newBlankLine = new char[] { '.', '.', '.', '.', '.', '.', '.' };
                                    Chamber = Chamber.Append(newBlankLine).ToArray();
                                }
                                for (int c = 0; c < rockInstance[r].Length; c++)
                                {
                                    if (rockInstance[r][c] == '@')
                                    {
                                        Chamber[r + currentRockLowerLeft.r][c + currentRockLowerLeft.c] = '#';
                                    }
                                }
                            }

                            //Console.WriteLine($"Rock {rockIndex} came to rest at ({currentRockLowerLeft.r},{currentRockLowerLeft.c})");

                            // Identity the highest rock in each column in the Chamber
                            int[] highestRockInColumn = new int[7] { -1, -1, -1, -1, -1, -1, -1 };
                            for (int c = 0; c < Chamber[0].Length; c++)
                            {
                                for (int r = Chamber.Length-1; r >= 0; r--)
                                {
                                    if (Chamber[r][c] == '#')
                                    {
                                        highestRockInColumn[c] = r;
                                        break;
                                    }
                                }
                            }

                            // Visualize the chamber
                            if (false)
                            {
                                for (int r = Chamber.Length; r > Math.Max(Chamber.Length - 100, 0); r--)
                                {
                                    Console.WriteLine($"{r,4} |{string.Join("", Chamber[r-1])}| {r-1,4}");
                                }
                                Console.WriteLine($"{"0",4} +-------+");
                            }

                            // Identify a repeating pattern in the history of highest rock in each column
                            // Every row in array HighestRockHistoryInEachColumn corresponds to one rock coming to rest
                            if (groupAddCounter == 0 && HighestRockHistoryInEachColumn.Any(r => new RockConfigurationComparer().Equals(r, highestRockInColumn)))
                            {
                                List<int[]> repeatingSet = HighestRockHistoryInEachColumn.SkipWhile(r => !new RockConfigurationComparer().Equals(r, highestRockInColumn)).ToList();
                                int rocksInRepeatingSet = repeatingSet.Count();

                                if (rocksInRepeatingSet > 1 && jetHistory[jetHistory.Length - 1 - rocksInRepeatingSet].start == jetHistory.Last().start)
                                {
                                    int highestRowOfCurrentRock = highestRockInColumn.Max();
                                    int previousHighRow = HighestRockHistoryInEachColumn[HighestRockHistoryInEachColumn.Length - rocksInRepeatingSet].Max();
                                    int repeatingChamberRowCount = highestRowOfCurrentRock - previousHighRow;
                                    
                                    while (rockCount + rocksInRepeatingSet < iteration.totalRuns)
                                    {
                                        groupAddCounter++;
                                        addedRowCount += repeatingChamberRowCount;
                                        rockCount += rocksInRepeatingSet;
                                    }
                                    Console.WriteLine($"For {groupAddCounter} times, added {rocksInRepeatingSet} (total {groupAddCounter*rocksInRepeatingSet}) rocks");
                                }

                            }
                            HighestRockHistoryInEachColumn = HighestRockHistoryInEachColumn.Append(highestRockInColumn).ToArray();

                            break;
                        }
                    }

                    if (rockCount % 1_000_000L == 0)
                    {
                        Console.WriteLine($"Performed {rockCount} rocks, row count is {Chamber.Length}");
                        Console.WriteLine($"Elapsed time for this interval is {stopwatch.Elapsed}");
                        stopwatch.Restart();
                    }
                }

                Console.WriteLine($"Part {iteration.part}: Rock is filling {RowsContainingRock() + addedRowCount} rows");
            }
        }

        /// <summary>
        /// Arrays of rock configuration, rows are bottom to top
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        internal static char[][] GetRockForIndex(int index)
        {
            return index switch
            {
                0 => Rock0,
                1 => Rock1,
                2 => Rock2,
                3 => Rock3,
                4 => Rock4,
                _ => throw new ApplicationException()
            };
        }

        internal static bool MoveIsAllowed(char[][] rock, (int r, int c) proposedLowerLeft)
        {
            if (proposedLowerLeft.r < 0)
            {
                return false;
            }

            for (int r = 0; r < rock.Length; r++)
            {
                int proposedRow = r + proposedLowerLeft.r;
                if (proposedRow < Chamber.Length)
                {
                    for (int c = 0; c < rock[r].Length; c++)
                    {
                        int proposedCol = c + proposedLowerLeft.c;
                        if (proposedCol < Chamber[proposedRow].Length && 
                            rock[r][c] == '@' && 
                            Chamber[proposedRow][proposedCol] == '#')
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        internal static long RowsContainingRock()
        {
            return Chamber.Count(r => r.Contains('#'));
        }
    }

    public class RockConfigurationComparer : IEqualityComparer<int[]>
    {
        //
        // Summary:
        //     Determines whether the specified objects are equal.
        //
        // Parameters:
        //   x:
        //     The first object of type T to compare.
        //
        //   y:
        //     The second object of type T to compare.
        //
        // Returns:
        //     true if the specified objects are equal; otherwise, false.
        public bool Equals(int[]? x, int[]? y)
        {
            if (x is null || y is null || x.Length != y.Length) return false;

            int[] xColumnDifferences = new int[x.Length - 1];
            int[] yColumnDifferences = new int[y.Length - 1];

            for (int i = 0; i < xColumnDifferences.Length; i++)
            {
                xColumnDifferences[i] = x[i+1] - x[i];
                yColumnDifferences[i] = y[i+1] - y[i];
            }

            return xColumnDifferences.SequenceEqual(yColumnDifferences);
        }

        //
        // Summary:
        //     Returns a hash code for the specified object.
        //
        // Parameters:
        //   obj:
        //     The System.Object for which a hash code is to be returned.
        //
        // Returns:
        //     A hash code for the specified object.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The type of obj is a reference type and obj is null.
        public int GetHashCode([DisallowNull] int[] obj)
        {
            return obj.GetHashCode();
        }
    }
}

