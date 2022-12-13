using System.Data;
using System.Drawing;

namespace Day12
{
    internal enum Elevation : byte
    {
        Unknown, S, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z, E
    }

    internal static class EnumExtensions
    {
        internal static byte GetHeight(this Elevation h) => h switch
        {
            Elevation.Unknown => 255,
            Elevation.S => 0,
            Elevation.a => 0,
            Elevation.b => 1,
            Elevation.c => 2,
            Elevation.d => 3,
            Elevation.e => 4,
            Elevation.f => 5,
            Elevation.g => 6,
            Elevation.h => 7,
            Elevation.i => 8,
            Elevation.j => 9,
            Elevation.k => 10,
            Elevation.l => 11,
            Elevation.m => 12,
            Elevation.n => 13,
            Elevation.o => 14,
            Elevation.p => 15,
            Elevation.q => 16,
            Elevation.r => 17,
            Elevation.s => 18,
            Elevation.t => 19,
            Elevation.u => 20,
            Elevation.v => 21,
            Elevation.w => 22,
            Elevation.x => 23,
            Elevation.y => 24,
            Elevation.z => 25,
            Elevation.E => 25,
            _ => throw new NotImplementedException(),
        };

        internal static bool MoveIsAllowed(this Elevation from, Elevation destination)
        {
            return destination.GetHeight() - from.GetHeight() <= 1;
        }
    }

    public class Coordinates
    {
        internal Coordinates() 
        { }

        internal Coordinates(int row, int col)
        {
            Row=row;
            Col=col;
        }

        internal int Row { get; init; } = -1;
        internal int Col { get; init; } = -1;

        #region operators
        public static bool operator ==(Coordinates me, (int Row, int Col) other)
        {
            return me.Row == other.Row && me.Col == other.Col;
        }
        public static bool operator !=(Coordinates me, (int Row, int Col) other)
        {
            return !(me == other);
        }
        public static bool operator ==(Coordinates me, Coordinates other)
        {
            return me == (other.Row, other.Col);
        }
        public static bool operator !=(Coordinates me, Coordinates other)
        {
            return !(me == (other.Row, other.Col));
        }
        #endregion
    }

    public class CoordinatesComparer : IEqualityComparer<Coordinates>
    {
        public bool Equals(Coordinates? obj1, Coordinates? obj2)
        {
            if (obj1 is null && obj2 is null) return true;
            if (obj1 is null) return false;
            if (obj2 is null) return false;
            return obj1.Row == obj2.Row && obj1.Col == obj2.Col;
        }

        public int GetHashCode(Coordinates obj)
        {
            return obj.Row << 16 + obj.Col;
        }
    }

    internal class RankedCoordinate : Coordinates
    {
        internal RankedCoordinate()
        { }

        internal RankedCoordinate(int row, int col, int s)
            : base(row, col)
        {
            Score= s;
        }

        internal int Score { get; init; } = -1;
        internal Elevation Elevation { get; set; }
        internal RankedCoordinate? Predecessor { get; set; }

        internal Coordinates Coordinates => this;
    }

    internal class Program
    {
        internal static Elevation[][] Map = new Elevation[0][];
        internal static List<RankedCoordinate> reachablePositions = new List<RankedCoordinate>();

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            RankedCoordinate start = new RankedCoordinate();
            RankedCoordinate end = new RankedCoordinate();

            ReadData("InputData.txt");

            for (int i = 0; i < Map.Length && (start.Coordinates == (-1, -1) || end.Coordinates == (-1, -1)); i++)
            {
                for (int j = 0; j < Map[i].Length; j++)
                {
                    switch (Map[i][j])
                    {
                        case Elevation.S:
                            start = new RankedCoordinate(i, j, 0) { Elevation = Elevation.S };
                            break;
                        case Elevation.E:
                            end = new RankedCoordinate(i, j, 0) { Elevation = Elevation.E };
                            reachablePositions.Add(end);
                            break;
                    }
                }
            }

            for (int moveCounter = 1; ; moveCounter++)
            {
                if (reachablePositions.Any(m => m.Coordinates == start))
                {
                    Visualize(/*true*/);
                    break;
                }

                Console.Write($"Move {moveCounter}, {(moveCounter % 10 == 0 ? Environment.NewLine : "")}");

                int startingListLength = reachablePositions.Count;
                for (int listcounter = 0; listcounter < startingListLength; listcounter++) 
                {
                    RankedCoordinate moveToConsider = reachablePositions.ElementAt(listcounter);
                    if (moveToConsider.Score < moveCounter-1)
                    {
                        continue;  // only evaluate moves from position(s) found in the previous round
                    }
                    List<Coordinates> neighbors = GetNeighbors(moveToConsider.Row, moveToConsider.Col).ToList();
                    var validMoves = GetValidMoves(moveToConsider.Coordinates, neighbors);
                    foreach (Coordinates neighbor in validMoves)
                    {
                        List<Coordinates> possibleMoveCoordinates = reachablePositions.Select(m => m.Coordinates).ToList();
                        if (possibleMoveCoordinates.Contains(neighbor, new CoordinatesComparer()))
                        {
                            continue;  // don't add new positions that have already been added
                        }
                        else
                        {
                            reachablePositions.Add(new RankedCoordinate(neighbor.Row, neighbor.Col, moveCounter) {Elevation = Map[neighbor.Row][neighbor.Col], Predecessor = moveToConsider });
                        }
                    }
                }
            }
        }

        internal static void Visualize(bool slow = false)
        {
            // delegate function to write the path coordinates to the console
            Action<RankedCoordinate> DocumentPath = pos => { };
            DocumentPath = pos =>
            {
                Console.WriteLine($"Coordinates {pos.Row},{pos.Col}, elevation {pos.Elevation}, steps from end position {pos.Score}");
                if (pos.Predecessor is not null)
                {
                    DocumentPath(pos.Predecessor);
                }
            };

            RankedCoordinate FoundStart = reachablePositions.First(m => m.Elevation == Elevation.S);
            RankedCoordinate FirstA = reachablePositions.First(m => (int)m.Elevation == (int)Elevation.a);

            DocumentPath.Invoke(FoundStart);
            DocumentPath.Invoke(FirstA);

            RankedCoordinate[][]? MapFromStart = Map.Clone() as RankedCoordinate[][];
            char[][] fileContents = new char[0][];
            foreach (Elevation[] line in Map)
            {
                char[] mapline = line.Select(p => p.ToString().First()).ToArray();
                fileContents = fileContents.Append(mapline).ToArray();
            }
            Console.Clear();
            for (int i = 0; i < fileContents.Length; i++)
            {
                for (int j = 0; j < fileContents[i].Length; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write(fileContents[i][j]);
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Part 1: The route has {FoundStart.Score} moves.");
            Console.WriteLine($"Part 2: The route has {FirstA.Score} moves");
            Console.BackgroundColor = ConsoleColor.Red;
            List<RankedCoordinate> redCoordinates = new List<RankedCoordinate>();
            for (RankedCoordinate position = FoundStart; position is not null; position = position.Predecessor)
            {
                Console.SetCursorPosition(position.Col, position.Row);
                Console.Write('1');
                redCoordinates.Add(position);
                if (slow) Thread.Sleep(1);
            }
            for (RankedCoordinate position = FirstA; position is not null; position = position.Predecessor)
            {
                Console.SetCursorPosition(position.Col, position.Row);
                Console.BackgroundColor = redCoordinates.Contains(position) ? ConsoleColor.DarkYellow : ConsoleColor.Green;
                Console.Write(redCoordinates.Contains(position) ? '\u00BD' : '2');
                if (slow) Thread.Sleep(1);
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, fileContents.Length+3);
        }

        static void ReadData(string filename)
        {
            Map = new Elevation[0][];
            foreach (string line in File.ReadLines(filename))
            {
                Elevation[] lineHeights = line.ToArray().Select(c => (Elevation)Enum.Parse(typeof(Elevation), c.ToString())).ToArray();
                Map = Map.Append(lineHeights).ToArray();
            }
        }

        static List<Coordinates> GetNeighbors(int row, int col)
        {
            int lowRow = Math.Max(0, row-1);
            int highRow = Math.Min(Map.Length-1, row+1);
            int lowCol = Math.Max(0, col-1);
            int highCol = Math.Min(Map[row].Length-1, col+1);

            List<Coordinates> returnVal = new List<Coordinates>();
            for (int r = lowRow; r <= highRow; r++)
            {
                returnVal.Add(new Coordinates(r, col));
            }
            for (int c = lowCol; c<=highCol; c++)
            {
                returnVal.Add(new Coordinates(row, c));
            }

            returnVal.RemoveAll(rc => rc == new Coordinates(row,col));

            return returnVal;
        }

        static List<Coordinates> GetValidMoves(Coordinates destination, IEnumerable<Coordinates> fromCandidates)
        {
            List<Coordinates> returnVal = new List<Coordinates>();

            foreach (Coordinates fromCandidate in fromCandidates)
            {
                if (Map[fromCandidate.Row][fromCandidate.Col].MoveIsAllowed(Map[destination.Row][destination.Col]))
                {
                    returnVal.Add(fromCandidate);
                }
            }

            return returnVal;
        }
    }
}