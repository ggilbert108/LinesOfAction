using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinesOfAction.Source
{
    class Tester
    {
        static void Main(string[] args)
        {
            Board board = new Board();

            const int aiPlayer = 1;

            int player = 1;

            while (board.gameOver() == 0)
            {
                HashSet<Move> moves = board.getPlayerMoves(player);
                if (moves.Count == 0)
                    break;

                if (player == aiPlayer)
                {
                    Move move = Intelligence.chooseMove(board, aiPlayer);
                    board.makeMove(ref move);
                }
                else
                {
                    Move move = Util.chooseMoveAtRandom(moves);
                    board.makeMove(ref move);
                }
                //Console.WriteLine(board);
                player = (player == 1) ? 2 : 1;
            }
            Console.WriteLine(Intelligence.negas);
            int winner = board.gameOver();

            Console.WriteLine(board);
            Console.WriteLine("The winner is... Player " + winner + "!");
            Console.Read();
        }
    }
}
