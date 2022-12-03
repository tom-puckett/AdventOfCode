using System.Collections.Generic;

namespace Day2
{
    internal enum Gesture
    {
        Rock = 0,
        Paper = 1,
        Scissors = 2
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<(Gesture opponent, Gesture me)> allRounds = ReadData("StrategyGuide.txt", 2);
            int counter = 0;
            int myTotalScore = 0;
            int opponentTotalScore = 0;

            foreach ((Gesture opponent, Gesture me) round in allRounds)
            {
                (int myScore, int opponentScore) scores = ScoresForRound(round.opponent, round.me);
                Console.WriteLine($"Round {++counter}, opponent gesture {round.opponent}, my gesture {round.me}: myScore {scores.myScore} opponentScore {scores.opponentScore}");
                myTotalScore += scores.myScore;
                opponentTotalScore += scores.opponentScore;
            }

            Console.WriteLine($"Final scores:{Environment.NewLine}    Mine: {myTotalScore}{Environment.NewLine}    Opponent: {opponentTotalScore}");
        }

        static List<(Gesture opponent, Gesture me)> ReadData(string fileName, int puzzlePart)
        {
            List<(Gesture, Gesture)> returnList = new List<(Gesture, Gesture)>();

            foreach (var line in File.ReadLines(fileName))
            {
                string[] round = line.Split(' ');

                Gesture opponentGesture = round[0] switch
                {
                    "A" => Gesture.Rock,
                    "B" => Gesture.Paper,
                    "C" => Gesture.Scissors,
                    _ => throw new ApplicationException()
                };
                Gesture myGesture = round[1] switch
                {
                    "X" => puzzlePart == 1 ? Gesture.Rock : LosingGesture(opponentGesture),
                    "Y" => puzzlePart == 1 ? Gesture.Paper : opponentGesture,
                    "Z" => puzzlePart == 1 ? Gesture.Scissors : WinningGesture(opponentGesture),
                    _ => throw new ApplicationException()
                };
                returnList.Add((opponentGesture, myGesture));
            }

            return returnList;
        }

        static (int myScore, int opponentScore) ScoresForRound(Gesture opponentGesture, Gesture myGesture)
        {
            int myScore = 0;
            int opponentScore = 0;
            switch (DoIWin(myGesture, opponentGesture))
            {
                // Win
                case true:
                    myScore += 6;
                    opponentScore += 0;
                    break;

                // Tie
                case null:
                    myScore += 3;
                    opponentScore += 3;
                    break;

                // Loss
                case false:
                    myScore += 0;
                    opponentScore += 6;
                    break;
            };

            myScore += myGesture switch
            {
                Gesture.Rock => 1,
                Gesture.Paper => 2,
                Gesture.Scissors => 3,
                _ => throw new NotImplementedException()  // stupid compiler warning
            };
            opponentScore += opponentGesture switch
            {
                Gesture.Rock => 1,
                Gesture.Paper => 2,
                Gesture.Scissors => 3,
                _ => throw new NotImplementedException()  // stupid compiler warning
            };

            return (myScore, opponentScore);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myGesture"></param>
        /// <param name="opponentGesture"></param>
        /// <returns>true for win, false for lose, null for tie</returns>
        /// <exception cref="NotImplementedException"></exception>
        static bool? DoIWin(Gesture myGesture, Gesture opponentGesture)
        {
            return myGesture switch
            {
                Gesture g when g == opponentGesture => null,
                Gesture g when g == WinningGesture(opponentGesture) => true,
                Gesture g when g == LosingGesture(opponentGesture) => false,
                _ => throw new NotImplementedException()
            };
        }

        static Gesture WinningGesture(Gesture opponentGesture)
        {
            return opponentGesture switch
            {
                Gesture.Rock => Gesture.Paper,
                Gesture.Paper => Gesture.Scissors,
                Gesture.Scissors => Gesture.Rock,
                _ => throw new NotImplementedException()  // stupid compiler warning
            };
        }

        static Gesture LosingGesture(Gesture opponentGesture)
        {
            return opponentGesture switch
            {
                Gesture.Rock => Gesture.Scissors,
                Gesture.Paper => Gesture.Rock,
                Gesture.Scissors => Gesture.Paper,
                _ => throw new NotImplementedException()  // stupid compiler warning
            };
        }
    }
}