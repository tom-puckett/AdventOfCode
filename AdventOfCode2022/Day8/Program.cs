namespace Day8
{
    internal class Program
    {
        static int[][] forest;

        static void Main(string[] args)
        {
            string[] fileLines = File.ReadAllLines("InputData.txt");
            forest = new int[fileLines.Length][];

            int i = 0;
            foreach (string line in fileLines)
            {
                forest[i++] = line.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
            }

            #region Part 1
            int visibleCount = 0;
            for (int x = 0; x < forest.Length; x++)
            {
                for (int y = 0; y < forest[x].Length; y++)
                {
                    if (IsVisible(x, y)) visibleCount++;
                }
            }
            Console.WriteLine($"{visibleCount} trees are visible");
            #endregion

            #region Part 2
            int maxScore = 0;
            for (int x = 0; x < forest.Length; x++)
            {
                for (int y = 0; y < forest[x].Length; y++)
                {
                    maxScore = Math.Max(maxScore, ScenicScore(x,y));
                }
            }
            Console.WriteLine($"Maximum visibility is {maxScore}");
            #endregion
        }

        static bool IsVisible(int x, int y)
        {
            if (x == 0 || y == 0 || x == forest.Length-1 || y == forest[x].Length-1)
            {
                return true;
            }

            List<int> left = new List<int>(), above = new List<int>(), right = new List<int>(), below = new List<int>();

            for (int j = 0; j < y; j++)
            {
                left.Add(forest[x][j]);
            }
            for (int i = 0; i < x; i++)
            {
                above.Add(forest[i][y]);
            }
            for (int j = y+1; j < forest.Length; j++)
            {
                right.Add(forest[x][j]);
            }
            for (int i = x+1; i < forest[y].Length; i++)
            {
                below.Add(forest[i][y]);
            ;};

            return left.All(t => t < forest[x][y]) ||
                  above.All(t => t < forest[x][y]) ||
                  right.All(t => t < forest[x][y]) ||
                  below.All(t => t < forest[x][y]);
        }

        static int maxscore = 0;

        static int ScenicScore(int x, int y)
        {
            int left = 0, above = 0, right = 0, below = 0;

            for (int j = y-1; j >= 0; j--)
            {
                left++;
                if (forest[x][j] >= forest[x][y])
                    break;
            }
            for (int i = x-1; i >= 0; i--)
            {
                above++;
                if (forest[i][y] >= forest[x][y])
                    break;
            }
            for (int j = y+1; j < forest[x].Length; j++)
            {
                right++;
                if (forest[x][j] >= forest[x][y])
                    break;
            }
            for (int i = x+1; i < forest.Length; i++)
            {
                below++;
                if (forest[i][y] >= forest[x][y])
                    break;
            };

            if (left * above * right * below > maxscore)
            {
                maxscore = left * above * right * below;
                Console.WriteLine($"new max score {maxscore} with left {left}, above {above}, right {right}, below {below} at {x},{y}");
            }
            return left * above * right * below;
        }

    }
}