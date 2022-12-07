string dataStream = File.ReadAllText("InputData.txt");

foreach (int markerLength in new[] {4,14})
{
    int markerPosition = markerLength;
    for (; markerPosition <= dataStream.Length && dataStream.Substring(markerPosition-markerLength, markerLength).Distinct().Count() < markerLength; markerPosition++) { }
    Console.WriteLine($"For marker length {markerLength}, marker postion is {markerPosition}");
}
