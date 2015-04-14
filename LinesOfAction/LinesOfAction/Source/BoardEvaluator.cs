using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LinesOfAction.Source
{
    /// <summary>
    /// All values range between 0 and 1
    /// </summary>
    static class BoardEvaluator
    {
        public static float evaluateBoard(EvalParameters evalParams, Board board, int player, bool considerEnemy)
        {
            float concentrationVal = concentration(board.board, player)*evalParams.concentration_weight;
            float centralizationVal = centralization(board.board, player)*evalParams.centralization_weight;
            float mobilityVal = mobility(board, player)*evalParams.mobility_weight;

            float result = concentrationVal + centralizationVal + mobilityVal;
            if (considerEnemy)
            {
                int enemy = player == 1 ? 2 : 1;
                result -= evaluateBoard(evalParams, board, enemy, false);
            }
          //  Console.WriteLine(result);
            return result;
        }

        private static float concentration(int[,] board, int player)
        {
            int centerR = 0;
            int centerC = 0;
            int count = 0;
            List<Coord> pieceCoords = new List<Coord>();
            for (int i = 0; i < Board.SIZE; i++)
            {
                for (int j = 0; j < Board.SIZE; j++)
                {
                    if (board[i, j] == player)
                    {
                        count++;
                        centerR += i;
                        centerC += j;
                        pieceCoords.Add(new Coord(i, j));
                    }
                }
            }
            centerR /= count;
            centerC /= count;
            Coord centerOfMass = new Coord(centerR, centerC);

            int distanceSums = 0;
            foreach (Coord pieceCoord in pieceCoords)
            {
                int distance = Util.manhattanDistance(pieceCoord, centerOfMass);
                distanceSums += distance;
            }
            int minDistanceSum = minDistanceSums[count];
            float surplusDistance = 0.0001f +  distanceSums - minDistanceSum;
            return 1.0f / (minDistanceSum / surplusDistance);
        }

        private static float centralization(int[,] board, int player)
        {
            float average = 0;
            int count = 0;
            for (int i = 0; i < Board.SIZE; i++)
            {
                for (int j = 0; j < Board.SIZE; j++)
                {
                    if (board[i, j] == player)
                    {
                        const int maxDistance = 6;
                        int distance = Util.manhattanDistance(new Coord(i, j),
                                                              new Coord(4, 4));
                        float score = -(distance - maxDistance);
                        average += score;
                        count++;
                    }
                }
            }
            return average/count;
        }

        private static  int mobility(Board board, int player)
        {
            return board.getPlayerMoves(player).Count;
        }

        private static readonly int[] minDistanceSums = 
            {0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14};
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    struct EvalParameters
    {
        public float concentration_weight;
        public float centralization_weight;
        public float mobility_weight;

        public EvalParameters(float co, float ce, float mo)
        {
            concentration_weight = co;
            centralization_weight = ce;
            mobility_weight = mo;
        }
    }
}
