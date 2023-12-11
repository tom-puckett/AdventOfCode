namespace Day02
{
    internal class Part2
    {
        public void Go()
        {
            int sum = 0;

            foreach (string line in File.ReadAllLines("InputData1.txt"))
            {
                (int gameNumber, List<SetOfCubes> setsInGame) = ParseInputLine(line);
                SetOfCubes leastCubesInGame = SetOfCubes.MaxCombinedSet(setsInGame);
                int power = leastCubesInGame.Power;
                sum += power;
            }

            Console.WriteLine("Part 2 answer: " + sum);
        }

        private (int, List<SetOfCubes>) ParseInputLine(string line)
        {
            List<SetOfCubes> setList = new List<SetOfCubes>();

            string[] colonSeparated = line.Split(':', StringSplitOptions.TrimEntries);
            int gameNumber = int.Parse(colonSeparated[0].Substring("Game ".Length));

            string[] sets = colonSeparated[1].Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string set in sets)
            {
                setList.Add(new SetOfCubes(set));
            }

            return (gameNumber, setList);
        }
    }
}
