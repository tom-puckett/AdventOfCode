namespace Day6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataStream = File.ReadAllText("InputData.txt");

            foreach (int markerLength in new[] {4,14})
            {
                int markerPosition = markerLength;
                for (; markerPosition <= dataStream.Length && dataStream.Substring(markerPosition-markerLength, markerLength).Distinct().Count() < markerLength; markerPosition++) { }
                Console.WriteLine($"Marker postion 1 is {markerPosition}");
            }
        }
    }
}