Func<(int x,int y), (int x, int y),int> GetDistance = (p1, p2) => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
List<((int x, int y) sensor, (int x, int y) beacon, int distance)> rawData = new();

ReadData("InputData.txt");  int testRow = 2_000_000;  int part2MaxSearchIndex = 4_000_000;
//ReadData("TestData.txt");  int testRow = 10;  int part2MaxSearchIndex = 20;

List<(int x, int y)> allBeaconLocations = rawData.Select(d => d.beacon).ToList();
int largestDistance = rawData.Select(d => d.distance).Max();
int xLow = rawData.Select(d => d.sensor).Min(s => s.x) - largestDistance;
int xHigh = rawData.Select(d => d.sensor).Max(s => s.x) + largestDistance;

#region Part 1
int coveredPointCount = 0;
for (int x = xLow; x <= xHigh; x++)
{
    if (rawData.Any(d => GetDistance((x, testRow), d.sensor) <= d.distance) &&
        !allBeaconLocations.Any(b => (x, testRow) == b))
    {
        coveredPointCount++;
    }
}
Console.WriteLine($"Line {testRow} has {coveredPointCount} covered points");
#endregion

#region Part 2
bool found = false;
for (int x = 0; x < part2MaxSearchIndex && !found; x++)
{
    for (int y = 0; y <= part2MaxSearchIndex; y++)
    {
        try
        {
            ((int x, int y) sensor, (int x, int y) beacon, int distance)? coveredBy = rawData.First(d => GetDistance((x, y), d.sensor) <= d.distance);
            y = coveredBy.Value.sensor.y + coveredBy.Value.distance - Math.Abs(x - coveredBy.Value.sensor.x);
        }
        catch (InvalidOperationException e) when (e.Message.Contains("no matching element"))
        {
            ulong tuningFrequency = (ulong)x * 4_000_000UL + (uint)y;
            Console.WriteLine($"Found uncovered point ({x},{y}) with tuning frequency {tuningFrequency}");
            found = true;
            break;
        }
    }
}
#endregion

List<(int x,int y)> GetCoveredPoints((int x, int y) sensorLocation, (int x, int y) beaconLocation)
{
    List<(int x, int y)> returnList = new List<(int x, int y)>();

    int coveredDistance = GetDistance(sensorLocation, beaconLocation);
    for (int x = sensorLocation.x - coveredDistance; x <= sensorLocation.x + coveredDistance; x++)
    {
        int yMaxDeviation = coveredDistance - Math.Abs(x - sensorLocation.x);
        for (int y = sensorLocation.y - yMaxDeviation; y <= sensorLocation.y + yMaxDeviation; y++)
        {
            if ((x,y) != beaconLocation)
            {
                returnList.Add((x, y));
            }
        }
    }

    return returnList;
}

void ReadData(string fileName)
{
    rawData.Clear();

    foreach (string line in File.ReadAllLines(fileName))
    {
        Func<string, string, string?, int, int> GetIntToken = (str, startMarker, endToken, startIndex) =>
        {
            int tokenStart = line.IndexOf(startMarker, startIndex) + startMarker.Length;
            if (endToken is null)
            {
                return int.Parse(line.Substring(tokenStart));
            }
            else
            {
                int fence = line.IndexOf(endToken, tokenStart);
                return int.Parse(line.Substring(tokenStart, fence - tokenStart));
            }
        };

        (int x, int y) sensor = (GetIntToken(line, "x=", ",", 0), GetIntToken(line, "y=", ":", 0));
        (int x, int y) beacon = (GetIntToken(line, "x=", ",", line.IndexOf("beacon")), GetIntToken(line, "y=", null, line.IndexOf("beacon")));

        rawData.Add((sensor, beacon, GetDistance(sensor, beacon)));
    }
}
