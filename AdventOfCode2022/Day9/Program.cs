namespace Day9
{
    internal class Program
    {
        static HashSet<(int, int)> part1PositionsVisited = new HashSet<(int, int)>(new PositionComparer()) { (0,0) };
        static HashSet<(int, int)> part2PositionsVisited = new HashSet<(int, int)>(new PositionComparer()) { (0,0) };
        //static (int row, int col) headPosition = (0, 0);
        //static (int row, int col) tailPosition = (0, 0);
        static List<(int row, int col)> part1Knots = new List<(int row, int col)>
        {
            (0, 0),  // head
            (0, 0)   // tail
        };

        static List<(int row, int col)> part2Knots = new List<(int row, int col)>
        {
            (0, 0),  // head 
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0)   // tail
        };

        static void Main(string[] args)
        {
            foreach (string line in File.ReadLines("InputData.txt"))
            {
                var components = line.Split(' ');
                (char direction, int number) instruction = (components[0][0], int.Parse(components[1]));
                Console.WriteLine($"Instruction is {instruction}");
                for (int i=0; i<instruction.number; i++)
                {
                    switch (instruction.direction)
                    {
                        case 'D':
                            part1Knots[0] = (part1Knots[0].row + 1, part1Knots[0].col);
                            part2Knots[0] = (part2Knots[0].row + 1, part2Knots[0].col);
                            break;
                        case 'R':
                            part1Knots[0] = (part1Knots[0].row, part1Knots[0].col + 1);
                            part2Knots[0] = (part2Knots[0].row, part2Knots[0].col + 1);
                            break;
                        case 'U':
                            part1Knots[0] = (part1Knots[0].row - 1, part1Knots[0].col);
                            part2Knots[0] = (part2Knots[0].row - 1, part2Knots[0].col);
                            break;
                        case 'L':
                            part1Knots[0] = (part1Knots[0].row, part1Knots[0].col - 1);
                            part2Knots[0] = (part2Knots[0].row, part2Knots[0].col - 1);
                            break;
                    }
                    for (int j=1; j< part1Knots.Count; j++)
                    {
                        part1Knots[j] = MoveTail(part1Knots[j-1], part1Knots[j]);
                    }
                    part1PositionsVisited.Add(part1Knots.Last());
                    for (int j = 1; j< part2Knots.Count; j++)
                    {
                        part2Knots[j] = MoveTail(part2Knots[j-1], part2Knots[j]);
                    }
                    part2PositionsVisited.Add(part2Knots.Last());
                }
            }

            Console.WriteLine($"In part 1 the tail has visited {part1PositionsVisited.Count} positions");
            Console.WriteLine($"In part 2 the tail has visited {part2PositionsVisited.Count} positions");
        }

        static (int,int) MoveTail((int row, int col) newHead, (int row, int col) oldTail) 
        {
            (int row, int col) returnValue = oldTail;

            if (Math.Abs(newHead.row - oldTail.row) <= 1 && Math.Abs(newHead.col - oldTail.col) <= 1)
            {
                returnValue = oldTail;
            }
            else if (newHead.row == oldTail.row)
            {
                returnValue = (oldTail.row, oldTail.col + Math.Sign(newHead.col - oldTail.col));
            }
            else if (newHead.col == oldTail.col)
            {
                returnValue = (oldTail.row + Math.Sign(newHead.row - oldTail.row), oldTail.col);
            }
            else
            {
                returnValue = (oldTail.row + Math.Sign(newHead.row - oldTail.row), oldTail.col + Math.Sign(newHead.col - oldTail.col));
            }

            return returnValue;
        }
    }

    public class PositionComparer : IEqualityComparer<(int, int)>
    {
        public bool Equals((int, int) x, (int, int) y)
        {
            return x.Item1 == y.Item1 && x.Item2 == y.Item2;
        }

        public int GetHashCode((int, int) obj) 
        {
            return obj.Item1 * 1000 + obj.Item2;
        }
    }
}