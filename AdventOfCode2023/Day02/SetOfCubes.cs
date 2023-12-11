using System.Linq;
using System.Numerics;

namespace Day02;

public struct SetOfCubes
{
    public SetOfCubes() { }
    public SetOfCubes(string inputData) 
    {
        string[] cubes = inputData.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var cube in cubes)
        {
            string[] tokens = cube.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            switch (tokens[1].ToLower())
            {
                case "red":
                    Red = int.Parse(tokens[0]);
                    break;
                case "blue":
                    Blue = int.Parse(tokens[0]);
                    break;
                case "green":
                    Green = int.Parse(tokens[0]);
                    break;
            }
        }
    }

    public int Red { get; set; } = 0;
    public int Green { get; set; } = 0;
    public int Blue { get; set; } = 0;

    public int Power => Red * Green * Blue;

    public bool IsPossible(SetOfCubes limit)
    {
        return Red <= limit.Red && Green <= limit.Green && Blue <= limit.Blue;
    }

    public static SetOfCubes MaxCombinedSet(IEnumerable<SetOfCubes> sets)
    {
        return new SetOfCubes 
        { 
            Red = sets.Max(c => c.Red),
            Green = sets.Max(c => c.Green),
            Blue = sets.Max(c => c.Blue)
        };
    }
}
