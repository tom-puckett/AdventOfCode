using System.Linq;

namespace Day16
{
    internal class Program
    {
        internal static Dictionary<string, Valve> System = new Dictionary<string, Valve>();
        internal static int totalPressureReleased = 0;
        internal static List<(int Minute, string ValveName, int rateIncrement, int totalRate, int totalReleased)> BestHistory = new();

        //static async Task Main(string[] args)
        static void Main(string[] args)
        {
            //ReadData("TestData.txt");
            ReadData("InputData.txt");

            Minute minute1 = Minute.FirstMinute;
            minute1.ExploreCandidates();

            Console.WriteLine($"Final result is score {totalPressureReleased}");
            Console.WriteLine($"Best history  is {string.Join(" ", BestHistory)}");
         }

        internal static void ReadData(string fileName)
        {
            System.Clear();

            foreach (string line in File.ReadAllLines(fileName))
            {
                int tokenStartIndex = line.IndexOf(' ') + 1;
                int fence = line.IndexOf(' ', tokenStartIndex);
                string valveName = line.Substring(tokenStartIndex, fence - tokenStartIndex);
                
                tokenStartIndex = line.IndexOf('=') + 1;
                fence = line.IndexOf(';');
                int rate = int.Parse(line.Substring(tokenStartIndex, fence - tokenStartIndex));

                tokenStartIndex = line.IndexOf(' ', line.IndexOf("to valve") + "to valve".Length);
                var tunnels = line.Substring(tokenStartIndex).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                System.Add(valveName, new Valve(valveName, rate, tunnels.ToList()));
            }
        }

        internal class Valve
        {
            internal string Name { get; init; }
            internal int Rate { get; init; }
            internal List<string> ConnectedValveNames { get; init; }

            internal Valve(string name, int rate, List<string> connectedValves)
            {
                Name = name;
                Rate = rate;
                ConnectedValveNames = connectedValves;
            }
        }

        internal class Minute
        {
            private int MinuteNumber { get; init; }
            private Valve Valve { get; init; }

            private int TotalRelease { get; init; }
            private List<(int Minute, string ValveName, int rateIncrement, int totalRate, int totalReleased)> PreviousHistory { get; set; } = new();
            private (int Minute, string ValveName, int rateIncrement, int totalRate, int totalReleased) LastHistoryElement { get; set; }
            private (int Minute, string ValveName, int rateIncrement, int totalRate, int totalReleased) MyHistorySeed { get; set; }

            private static int FullHistoryCounter = 0;
            private static int CancelCounter = 0;

            internal static Minute FirstMinute => new Minute(1, "AA", new List<(int Minute, string ValveName, int rateIncrement, int totalRate, int totalReleased)>());

            internal Minute(int minuteNumber, string valveName, List<(int Minute, string ValveName, int rateIncrement, int totalRate, int totalReleased)> previousHistory)
            {
                MinuteNumber = minuteNumber;
                Valve = System[valveName];

                PreviousHistory = previousHistory;
                LastHistoryElement = PreviousHistory.LastOrDefault();

                MyHistorySeed = (MinuteNumber, Valve.Name, 0, LastHistoryElement.totalRate, LastHistoryElement.totalReleased + LastHistoryElement.totalRate);
                //Console.WriteLine($"Minute {MinuteNumber}: releasing {LastHistoryElement.totalRate} pressure, total is {LastHistoryElement.totalReleased + LastHistoryElement.totalRate}");
            }

            internal void ExploreCandidates()
            {
                if (MinuteNumber == 30)
                {
                    FullHistoryCounter++;
                    if (FullHistoryCounter % 100_000 == 0)
                    {
                        Console.WriteLine($"Completed {FullHistoryCounter:N} searches to depth 30, {CancelCounter:N} have been canceled");
                    }
                    if (MyHistorySeed.totalReleased > totalPressureReleased)
                    {
                        //Console.WriteLine($"Minute {MinuteNumber}, counter {++FullHistoryCounter}, total relief {MyHistorySeed.totalReleased,4} from history: {string.Join(" ", PreviousHistory.Append(MyHistorySeed).Select(h => $"({h})"))}");
                        //Console.WriteLine($"New high total {MyHistorySeed.totalReleased} to replace previous high of {totalPressureReleased}");
                        totalPressureReleased = MyHistorySeed.totalReleased;
                        BestHistory = PreviousHistory.Append(MyHistorySeed).ToList();
                    }
                    return;
                }

                if (!PreviousHistory.Any(h => h.ValveName == Valve.Name && h.rateIncrement > 0) && Valve.Rate > 0)
                {
                    // this valve is closed and has nonzero rate
                    //Console.WriteLine($"Minute {MinuteNumber}: opening valve {Valve.Name} with rate {Valve.Rate}");
                    Minute nextCandidateMinute = new Minute(MinuteNumber+1, Valve.Name, PreviousHistory.Append(
                            (MyHistorySeed.Minute, MyHistorySeed.ValveName, Valve.Rate, MyHistorySeed.totalRate + Valve.Rate, MyHistorySeed.totalReleased)
                        ).ToList());
                    nextCandidateMinute.ExploreCandidates();
                }
                foreach (var connectedValveName in Valve.ConnectedValveNames)
                {
                    var candidateValve = System[connectedValveName];
                    //MyHistoryElement = (MyHistoryElement.Minute, MyHistoryElement.ValveName, 0, MyHistoryElement.totalRate, MyHistoryElement.totalReleased);

                    // check if the candidate has already been visited, still consider only if the rate has increased since then
                    if (PreviousHistory.Any(h => h.ValveName == connectedValveName) && 
                        PreviousHistory.Last(h => h.ValveName == connectedValveName).totalRate == MyHistorySeed.totalRate)
                    {
                        CancelCounter++;
                        //Console.WriteLine($"Minute {MinuteNumber}: skipping move to candidate {connectedValveName}");
                        continue;
                    }
                    else
                    {
                        // candidate has not been visited before, or the total rate has changed since it was visited
                        //Console.WriteLine($"Minute {MinuteNumber}: moving from {Valve.Name} to {connectedValveName} ");
                        Minute nextCandidateMinute = new Minute(MinuteNumber+1, connectedValveName, PreviousHistory.Append(
                                (MyHistorySeed.Minute, MyHistorySeed.ValveName, 0, MyHistorySeed.totalRate, MyHistorySeed.totalReleased)
                            ).ToList());
                        nextCandidateMinute.ExploreCandidates();
                    }
                }
            }
        }
    }
}