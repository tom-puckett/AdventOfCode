using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
            return obj1 == obj2;
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

        internal Coordinates Coordinates => this;

        internal bool SameLocation(int r, int c) => r == Row && c == Col;
    }

    internal class Program
    {
        internal static Elevation[][] Map = new Elevation[0][];

        static void Main(string[] args)
        {
            List<RankedCoordinate> possibleMoves = new List<RankedCoordinate>();
            RankedCoordinate start = new RankedCoordinate();
            RankedCoordinate end = new RankedCoordinate();

            ReadData("InputData.txt");
            //ReadData("TestData.txt");
            for (int i = 0; i < Map.Length; i++)
            {
                for (int j = 0; j < Map[i].Length; j++)
                {
                    // Console.WriteLine($"At ({i},{j}), neighbors are {string.Join(",", GetNeighbors(i, j).Select(n => $"({n.Row},{n.Col})"))}");
                }
            }

            for (int i = 0; i < Map.Length; i++)
            {
                if (start.Coordinates != (-1,-1) && end.Coordinates != (-1, -1))
                {
                    break;
                }

                for (int j = 0; j < Map[i].Length; j++)
                {
                    switch (Map[i][j])
                    {
                        case Elevation.S:
                            start = new RankedCoordinate(i, j, 0) { Elevation = Elevation.S };
                            possibleMoves.Add(start);
                            break;
                        case Elevation.E:
                            end = new RankedCoordinate(i, j, 0) { Elevation = Elevation.E };
                            break;
                    }
                }
            }

            for (int moveCounter = 1; ; moveCounter++)
            {
                Console.WriteLine($"Calculating move {moveCounter}");
                int startingListLength = possibleMoves.Count;
                for (int listcounter = 0; listcounter < startingListLength; listcounter++) 
                {
                    RankedCoordinate moveToConsider = possibleMoves.ElementAt(listcounter);
                    if (moveToConsider.Score < moveCounter-1)
                    {
                        continue;
                    }
                    List<Coordinates> neighbors = GetNeighbors(moveToConsider.Row, moveToConsider.Col).ToList();
                    var validMoves = GetValidMoves(moveToConsider.Coordinates, neighbors);
                    foreach (Coordinates neighbor in validMoves)
                    {
                        List<Coordinates> possibleMoveCoordinates = possibleMoves.Select(m => m.Coordinates).ToList();
                        bool yesOrNo = possibleMoveCoordinates.Contains(neighbor);
                        if (possibleMoveCoordinates.Contains(neighbor, new CoordinatesComparer()))
                        {
                            continue;
                        }
                        else
                        {
                            possibleMoves.Add(new RankedCoordinate(neighbor.Row, neighbor.Col, moveCounter) {Elevation = Map[neighbor.Row][neighbor.Col] });
                        }
                    }
                }

                if (possibleMoves.Any(m => m.Coordinates == end))
                {
                    Console.WriteLine($"Found a route with {possibleMoves.First(m => m.Coordinates == end).Score} moves");
                    break;
                }

                if (possibleMoves.Count == startingListLength)
                {
                    Console.WriteLine($"No valid moves found at move {moveCounter}, move list size is {possibleMoves.Count}");
                }
            }

            Console.WriteLine("Hello, World!");
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
            for (int r = lowRow; r<=highRow; r++)
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

        static List<Coordinates> GetValidMoves(Coordinates from, IEnumerable<Coordinates> candidates)
        {
            List<Coordinates> returnVal = new List<Coordinates>();

            foreach (Coordinates candidate in candidates)
            {
                if (Map[from.Row][from.Col].MoveIsAllowed(Map[candidate.Row][candidate.Col]))
                {
                    returnVal.Add(candidate);
                }
            }

            return returnVal;
        }
    }
}