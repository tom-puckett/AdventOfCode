namespace Day10
{
    internal class Program
    {
        internal static List<int> milestones = new() { 20, 60, 100, 140, 180, 220 };
        internal static int cyclesCompleted = 0;
        internal static int registerValue = 1;
        internal static int signalStrengthSum = 0;

        static void Main(string[] args)
        {
            foreach (string line in File.ReadLines("InputData.txt"))
            {
                string[] lineTokens = line.Split(' ');

                switch (lineTokens[0])
                {
                    case "addx":
                        ClockCycle();
                        ClockCycle(int.Parse(lineTokens[1]));
                        break;

                    case "noop":
                        ClockCycle();
                        break;
                }
            }

            Console.WriteLine($"Final signal strength sum is {signalStrengthSum}");
        }

        internal static void ClockCycle(int registerIncrement = 0)
        {
            int pixelPosition = cyclesCompleted % 40;
            char displayChar = Math.Abs(pixelPosition - registerValue)<2 ? '#' : '.';

            Console.Write(displayChar);
            if (pixelPosition == 39)
            {
                Console.WriteLine();
            }

            cyclesCompleted++;
            registerValue += registerIncrement;

            if (milestones.Contains(cyclesCompleted))
            {
                signalStrengthSum += cyclesCompleted * registerValue;
            }
        }
    }
}