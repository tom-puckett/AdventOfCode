using System.ComponentModel;

namespace Day18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool IsTest = false;
            string fileName = IsTest ? "TestData.txt" : "InputData.txt";
            int exposedSurfaceCount = 0;
            
            List<(int x, int y, int z)> allLocations = new List<(int x, int y, int z)>();


            foreach (string line in File.ReadLines(fileName))
            {
                int[] coordinates = line.Split(',').Select(c => int.Parse(c)).ToArray();
                var newLocation = (coordinates[0], coordinates[1], coordinates[2]);

                int adjacentLocations = allLocations.Count(l => AreCubesAdjacent(newLocation, l));
                allLocations.Add(newLocation);

                exposedSurfaceCount += 6 - 2*adjacentLocations;
            }

            Console.WriteLine($"Total surface area is {exposedSurfaceCount}");

        }

        static bool AreCubesAdjacent((int x,int y, int z) a, (int x, int y, int z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z) == 1;
        }
    }
}