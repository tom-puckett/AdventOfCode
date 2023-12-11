namespace Day01
{
    internal class Part1
    {
        internal void Count(bool verbose = false)
        {
            char[] digits = "0123456789".ToCharArray();
            int sum = 0;

            foreach (string line in File.ReadAllLines("InputData1.txt"))
            {
                char firstDigit = line[line.IndexOfAny(digits)];
                char lastDigit = line[line.LastIndexOfAny(digits)];
                int calibrationValue = GetNumberFromDigits(new char[] { firstDigit, lastDigit });
                sum += calibrationValue;
                if (verbose)
                {
                    Console.WriteLine("Line is <{0}>, value is {1}, sum is {2}", line, calibrationValue, sum);
                }
            }

            Console.WriteLine("Part 1 sum for all input data is {0}", sum);
        }

        private int GetNumberFromDigits(IEnumerable<char> HighestToLowest)
        {
            int result = HighestToLowest.Aggregate(0, (sum, element) => sum * 10 + element - '0');
            return result;
        }
    }
}
