using System.Diagnostics.Metrics;

namespace Day19
{
    internal class Program
    {
        internal static List<Blueprint> Blueprints = new List<Blueprint>();
        internal static Dictionary<int, int> BlueprintQualityDict = new Dictionary<int, int>();

        static void Main(string[] args)
        {
            ReadData("TestData.txt");
            //ReadData("InputData.txt");

            foreach (Blueprint blueprint in Blueprints)
            {
                Minute firstMinute = new Minute(1, Inventory.NewInventory, blueprint);
                firstMinute.Process();
            }
        }

        internal static void ReadData(string fileName)
        {
            Func<string, string, string, int, string> GetToken = (line, startMarker, fenceMarker, searchStart) => 
            {
                int startIndex = line.IndexOf(startMarker, searchStart) + startMarker.Length;
                int fenceIndex = line.IndexOf(fenceMarker, startIndex);
                string token = line.Substring(startIndex, fenceIndex - startIndex);
                return token;
            };

            foreach (string line in File.ReadAllLines(fileName))
            {

                int blueprintNumber = int.Parse(GetToken(line, "Blueprint ", ":", 0));
                Blueprint newBlueprint = new Blueprint(blueprintNumber)
                {
                    OreRobotCost = new RobotCost
                    {
                        OreCost = int.Parse(GetToken(line, "Each ore robot costs ", " ore.", 0))
                    },
                    ClayRobotCost = new RobotCost
                    {
                        OreCost = int.Parse(GetToken(line, "Each clay robot costs ", " ore.", 0))
                    },
                    ObsidianRobotCost = new RobotCost
                    {
                        OreCost = int.Parse(GetToken(line, "Each obsidian robot costs ", " ore and ", 0)),
                        ClayCost = int.Parse(GetToken(line, "ore and ", " clay.", 0))
                    },
                    GeodeRobotCost = new RobotCost
                    {
                        OreCost = int.Parse(GetToken(line, "Each geode robot costs ", " ore and ", 0)),
                        ObsidianCost = int.Parse(GetToken(line, "ore and ", " obsidian.", line.IndexOf("geode")))
                    }
                };

                Blueprints.Add(newBlueprint);
                BlueprintQualityDict.Add(newBlueprint.BlueprintNumber, 0);
            }
        }
    }

    internal class Minute
    {
        private static long _counter = 0;

        internal int Number { get; set; }

        internal Inventory MyInventory { get; set; }
        internal Blueprint MyBlueprint { get; set; }

        internal Minute(int mumber, Inventory inventory, Blueprint blueprint)
        {
            MyInventory = inventory;
            MyBlueprint = blueprint;
            Number= mumber;
        }

        internal void Process()
        {
            // 1. Make a list of choices based on available resources
            List<(int ore, int clay, int obs, int goe)> purchaseList = new();
            (int maxOreRobots, int maxClayRobots, int maxObsidianRobots, int maxGeodeRobots) overallPurchaseLimits = PurchasableRobots(MyInventory, MyBlueprint);

            for (int oreRobots = 0; oreRobots <= overallPurchaseLimits.maxOreRobots; oreRobots++)
            {
                Inventory oreUpdatedInventory = MyInventory - new Inventory { OreCount = MyBlueprint.OreRobotCost.OreCost * oreRobots };
                (int maxOreRobots, int maxClayRobots, int maxObsidianRobots, int maxGeodeRobots) limits2 = PurchasableRobots(oreUpdatedInventory, MyBlueprint);

                for (int clayRobots = 0; clayRobots <= limits2.maxClayRobots; clayRobots++)
                {
                    Inventory clayUpdatedInventory = oreUpdatedInventory - new Inventory { OreCount = MyBlueprint.ClayRobotCost.OreCost * clayRobots };
                    (int maxOreRobots, int maxClayRobots, int maxObsidianRobots, int maxGeodeRobots) limits3 = PurchasableRobots(clayUpdatedInventory, MyBlueprint);

                    for (int obsidianRobots = 0; obsidianRobots <= limits3.maxObsidianRobots; obsidianRobots++)
                    {
                        Inventory obsidianUpdatedInventory = clayUpdatedInventory - new Inventory { OreCount = MyBlueprint.ObsidianRobotCost.OreCost * obsidianRobots, 
                                                                                                    ClayCount = MyBlueprint.ObsidianRobotCost.ClayCost * obsidianRobots };
                        (int maxOreRobots, int maxClayRobots, int maxObsidianRobots, int maxGeodeRobots) limits4 = PurchasableRobots(obsidianUpdatedInventory, MyBlueprint);

                        for (int geodeRobots = 0; geodeRobots <= limits4.maxGeodeRobots; geodeRobots++)
                        {
                            purchaseList.Add((oreRobots, clayRobots, obsidianRobots, geodeRobots));
                        }
                    }
                }
            }

            foreach ((int ore, int clay, int obs, int geo) actions in purchaseList)
            {
                //Console.WriteLine($"== Minute {Number} ==");
                //Console.WriteLine($"Inventory is {MyInventory.ToString()}");
                //Console.WriteLine($"Testing actions: {actions}");

                Inventory newInventory = MyInventory.Clone();

                // 1. Spend resources to build
                if (actions.ore > 0)
                {
                    newInventory.OreCount -= actions.ore * MyBlueprint.OreRobotCost.OreCost;
                    //Console.WriteLine($"Spend {actions.ore * MyBlueprint.OreRobotCost.OreCost} ore to start building {actions.ore} ore-collecting robots.");
                }
                if (actions.clay > 0)
                {
                    newInventory.OreCount -= actions.clay * MyBlueprint.ClayRobotCost.OreCost;
                    //Console.WriteLine($"Spend {actions.clay * MyBlueprint.ClayRobotCost.OreCost} ore to start building {actions.clay} clay-collecting robots.");
                }
                if (actions.obs > 0)
                {
                    newInventory.OreCount -= actions.obs * MyBlueprint.ObsidianRobotCost.OreCost;
                    newInventory.ClayCount -= actions.obs * MyBlueprint.ObsidianRobotCost.ClayCost;
                    //Console.WriteLine($"Spend {actions.obs * MyBlueprint.ObsidianRobotCost.OreCost} ore and {actions.obs * MyBlueprint.ObsidianRobotCost.ClayCost} clay to start building {actions.obs} obsidian-collecting robots.");
                }
                if (actions.geo > 0)
                {
                    newInventory.OreCount -= actions.geo * MyBlueprint.GeodeRobotCost.OreCost;
                    newInventory.ObsidianCount -= actions.geo * MyBlueprint.GeodeRobotCost.ObsidianCost;
                    //Console.WriteLine($"Spend {actions.geo * MyBlueprint.GeodeRobotCost.OreCost} ore and {actions.geo * MyBlueprint.GeodeRobotCost.ObsidianCost} obsidian to start building {actions.obs} geode-cracking robot.");
                }

                // 2. Robots in StartingPopulation do what they do
                newInventory += new Inventory
                {
                    OreCount = newInventory.OreRobotCount,
                    ClayCount = newInventory.ClayRobotCount,
                    ObsidianCount = newInventory.ObsidianRobotCount,
                    OpenGeodeCount = newInventory.GeodeRobotCount,
                };

                if (actions.geo > 0)
                {
                    int i = 8;
                }
                // 3. Add built robots to population
                newInventory += new Inventory
                {
                    OreRobotCount = actions.ore,
                    ClayRobotCount = actions.clay,
                    ObsidianRobotCount = actions.obs,
                    GeodeRobotCount = actions.geo,
                };

                // 4. Launch the next minute
                if (Number < 24)
                {
                    Minute nextMinute = new Minute(Number + 1, newInventory, MyBlueprint);
                    nextMinute.Process();
                }
                else
                {
                    _counter++;
                    if (_counter % 100_000 == 0)
                    {
                        Console.WriteLine($"Completed {_counter} paths");
                    }

                    int quality = MyBlueprint.BlueprintNumber * newInventory.OpenGeodeCount;
                    if (quality > Program.BlueprintQualityDict[MyBlueprint.BlueprintNumber])
                    {
                        Program.BlueprintQualityDict[MyBlueprint.BlueprintNumber] = quality;
                    };
                }
            }
        }

        internal (int,int,int,int) PurchasableRobots(Inventory inventory, Blueprint blueprint, int limitTo = 1)
        {
            int affordableOreRobots = inventory.OreCount / blueprint.OreRobotCost.OreCost;
            int affordableClayRobots = inventory.OreCount / blueprint.ClayRobotCost.OreCost;
            int affordableObsidianRobots = Math.Min(inventory.OreCount / blueprint.ObsidianRobotCost.OreCost, 
                                                    inventory.ClayCount / blueprint.ObsidianRobotCost.ClayCost);
            int affordableGeodeRobots = Math.Min(inventory.OreCount / blueprint.GeodeRobotCost.OreCost, 
                                                 inventory.ObsidianCount / blueprint.GeodeRobotCost.ObsidianCost);

            return (Math.Min(affordableOreRobots, limitTo), Math.Min(affordableClayRobots, limitTo), Math.Min(affordableObsidianRobots, limitTo), Math.Min(affordableGeodeRobots, limitTo));
        }

    }

    internal class RobotCost
    {
        internal int OreCost { get; set; } = 0;
        internal int ClayCost { get; set; } = 0;
        internal int ObsidianCost { get; set; } = 0;
    }

    internal class Blueprint
    {
        internal Blueprint(int number) 
        {
            BlueprintNumber = number;
        }

        internal int BlueprintNumber { get; init; }
        internal RobotCost OreRobotCost { get; set; }
        internal RobotCost ClayRobotCost { get; set; }
        internal RobotCost ObsidianRobotCost { get; set; }
        internal RobotCost GeodeRobotCost { get; set; }
    }

    internal class Inventory
    {
        internal int OreRobotCount { get; set; } = 0;
        internal int ClayRobotCount { get; set; } = 0;
        internal int ObsidianRobotCount { get; set; } = 0;
        internal int GeodeRobotCount { get; set; } = 0;

        internal int OreCount { get; set; } = 0;
        internal int ClayCount { get; set; } = 0;
        internal int ObsidianCount { get; set; } = 0;
        internal int OpenGeodeCount { get; set; } = 0;

        internal static Inventory NewInventory => new Inventory { OreRobotCount = 1 };

        internal string ToString(bool wrapped = false)
        {
            return
                $"{OreRobotCount} OreRobots" + (wrapped ? Environment.NewLine : ", ") +
                $"{ClayRobotCount} ClayRobots" + (wrapped ? Environment.NewLine : ", ") +
                $"{ObsidianRobotCount} ObsidianRobots" + (wrapped ? Environment.NewLine : ", ") +
                $"{GeodeRobotCount} GeodeRobots" + (wrapped ? Environment.NewLine : ", ") +
                $"{OreCount} Ore" + (wrapped ? Environment.NewLine : ", ") +
                $"{ClayCount} Clay" + (wrapped ? Environment.NewLine : ", ") +
                $"{ObsidianCount} Obsidian" + (wrapped ? Environment.NewLine : ", ") +
                $"{OpenGeodeCount} OpenGeodes";
        }

        public Inventory Clone()
        {
            return new Inventory
            {
                OreRobotCount      = OreRobotCount,
                ClayRobotCount     = ClayRobotCount,
                ObsidianRobotCount = ObsidianRobotCount,
                GeodeRobotCount    = GeodeRobotCount,
                OreCount           = OreCount,
                ClayCount          = ClayCount,
                ObsidianCount      = ObsidianCount,
                OpenGeodeCount     = OpenGeodeCount
            };
        }

        public static Inventory operator -(Inventory x, Inventory y) => new Inventory
            {
                OreRobotCount      = x.OreRobotCount      - y.OreRobotCount,
                ClayRobotCount     = x.ClayRobotCount     - y.ClayRobotCount,
                ObsidianRobotCount = x.ObsidianRobotCount - y.ObsidianRobotCount,
                GeodeRobotCount    = x.GeodeRobotCount    - y.GeodeRobotCount,

                OreCount           = x.OreCount           - y.OreCount,
                ClayCount          = x.ClayCount          - y.ClayCount,
                ObsidianCount      = x.ObsidianCount      - y.ObsidianCount,
                OpenGeodeCount     = x.OpenGeodeCount     - y.OpenGeodeCount
            };

        public static Inventory operator +(Inventory x, Inventory y) => new Inventory
        {
            OreRobotCount      = x.OreRobotCount      + y.OreRobotCount,
            ClayRobotCount     = x.ClayRobotCount     + y.ClayRobotCount,
            ObsidianRobotCount = x.ObsidianRobotCount + y.ObsidianRobotCount,
            GeodeRobotCount    = x.GeodeRobotCount    + y.GeodeRobotCount,

            OreCount           = x.OreCount           + y.OreCount,
            ClayCount          = x.ClayCount          + y.ClayCount,
            ObsidianCount      = x.ObsidianCount      + y.ObsidianCount,
            OpenGeodeCount     = x.OpenGeodeCount     + y.OpenGeodeCount
        };

    }
}