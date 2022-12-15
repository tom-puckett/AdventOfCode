namespace Day14
{
    internal class Program
    {
        internal static char[][] map = Array.Empty<char[]>();
        internal static int mapMaxRow = 0;
        internal static int mapMinCol = 0;
        internal static int mapMaxCol = 0;
        internal static char RockChar = '#';
        internal static char SandChar = 'o';
        internal static char OpenChar = '.';
        internal static (int r, int c) StartLocation = (0, 500);
        internal static Func<(int, int), (int col, int row)> GetScreenCoordinates = mapCoordinates => (mapCoordinates.Item1 - mapMinCol + 1, mapCoordinates.Item2);
        internal static Func<(int, int), (int col, int row)> GetMapCoordinates = screenCoordinates => (screenCoordinates.Item1 + mapMinCol - 1, screenCoordinates.Item2);

        static void Main(string[] args)
        {
            Console.Clear();

            foreach (int puzzlePart in new int[] { 1, 2})
            {
                ReadData("InputData.txt");
                //ReadData("TestData.txt");

                RenderInitialMap();
                Console.SetCursorPosition(0, 0);

                int grainCount = 0;
                while (AddGrainOfSand(puzzlePart))
                {
                    grainCount++;
                }

                Console.SetCursorPosition(0, mapMaxRow + 4 + puzzlePart);
                Console.WriteLine($"Part {puzzlePart}: {grainCount} grains of sand were retained.");
            }
        }

        static void ReadData(string filename)
        {
            map = Array.Empty<char[]>();

            List<List<(int col, int row)>> RockPaths = new();
            List<(int col, int row)> AllRockCoordinates = new();

            RockPaths.Clear();
            AllRockCoordinates.Clear();
            foreach (string line in File.ReadLines(filename)) 
            {
                List<(int,int)> lineCoordinatesList = line.Split(new[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries )
                                                          .Select(c => (int.Parse(c.Split(',')[0]), int.Parse(c.Split(',')[1])))
                                                          .ToList();
                RockPaths.Add(lineCoordinatesList.Skip(1)
                                                 .Aggregate(new List<(int, int)> { lineCoordinatesList[0] },
                                                            (l, next) =>
                                                            {
                                                                AddPathToMap(l, next);
                                                                return l;
                                                            }));
            }
            AllRockCoordinates = RockPaths.SelectMany(c => c).ToList();

            mapMaxRow = AllRockCoordinates.Select(c => c.row).Max();
            mapMinCol = StartLocation.c - mapMaxRow - 1;
            mapMaxCol = StartLocation.c + mapMaxRow + 1;
            for (int row=0; row <= mapMaxRow + 1; row++)  // empty row at the bottom to detect sand falling out (part 1)
            {
                char[] newMapRow = Enumerable.Repeat(OpenChar, mapMaxCol+2).ToArray();
                foreach (var rockInThisRow in AllRockCoordinates.Where(c => c.row == row))
                {
                    newMapRow[rockInThisRow.col] = RockChar;
                }

                map = map.Append(newMapRow).ToArray();
            }
            map[StartLocation.r][StartLocation.c] = '+';
        }

        static void RenderInitialMap()
        {
            for (int c = 0; c < mapMaxCol - mapMinCol + 3; c++)
            {
                for (int r = 0; r <= mapMaxRow + 1; r++)
                {
                    (int col, int row) mapCoordinates = GetMapCoordinates((c, r));
                    if (c < Console.BufferWidth)
                    {
                        Console.SetCursorPosition(c, r);
                        Console.Write(map[mapCoordinates.row][mapCoordinates.col]);
                    }
                }
            }
            for (int r = 0; r <= mapMaxRow + 1; r++)
            {
                (int col, int row) mapCoordinates = GetMapCoordinates((0, r));
                if (mapMaxCol - mapMinCol + 4 < Console.BufferWidth)
                {
                    Console.SetCursorPosition(mapMaxCol - mapMinCol + 4, r);
                    Console.WriteLine("{0,3:D1}", mapCoordinates.row);
                }
            }
        }

        static bool AddGrainOfSand(int part)
        {
            if (map[StartLocation.r][StartLocation.c]== SandChar)
            {
                return false;
            }

            List<(int r,int c)> validMoves = new List<(int, int)> {(1,0), (1,-1), (1,1), (0,0) };
            (int r, int c) grainLocation = StartLocation;

            (int r, int c) storedLocation;
            do
            {
                storedLocation = grainLocation;
                foreach (var possibleMove in validMoves)
                {
                    if (grainLocation.c + possibleMove.c >= 0 &&
                        grainLocation.c + possibleMove.c <= mapMaxCol &&
                        map[grainLocation.r + possibleMove.r][grainLocation.c + possibleMove.c] == OpenChar)
                    {
                        grainLocation = (grainLocation.r + possibleMove.r, grainLocation.c + possibleMove.c);
                        break;
                    }
                }
                switch (part) 
                {
                    case 1:
                        if (grainLocation.r > mapMaxRow || grainLocation == StartLocation)
                        {
                            return false;
                        }
                        break;
                    case 2:
                        if (grainLocation.r == mapMaxRow + 1)
                        {
                            map[grainLocation.r][grainLocation.c] = SandChar;
                            RenderChar((grainLocation.r, grainLocation.c), SandChar);
                            return true;
                        }
                        break;
                }
            }
            while (grainLocation != storedLocation);

            map[grainLocation.r][grainLocation.c] = SandChar;
            RenderChar((grainLocation.r, grainLocation.c), SandChar);
            return true;
        }

        internal static void RenderChar((int row, int col) location, char c)
        {
            (int col, int row) screenCoordinates = GetScreenCoordinates((location.col, location.row));
            if (screenCoordinates.col < Console.BufferWidth)
            {
                Console.SetCursorPosition(screenCoordinates.col, screenCoordinates.row);
                Console.Write(c);
            }
        }

        static void AddPathToMap(List<(int col, int row)> list, (int col, int row) to)
        {
            bool moveUpDown = list.Last().col == to.col;

            if (moveUpDown)
            {
                int increment = Math.Sign(to.row - list.Last().row);

                for (int i = list.Last().row + increment; ; i += increment)
                {
                    list.Remove((to.col, i));
                    list.Add((to.col, i));
                    if (i == to.row) break;
                }
            }
            else
            {
                int increment = Math.Sign(to.col - list.Last().col);

                for (int i = list.Last().col + increment; ; i += increment)
                {
                    list.Remove((i, to.row));
                    list.Add((i, to.row));
                    if (i == to.col) break;
                }
            }
        }
    }
}