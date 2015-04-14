using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinesOfAction.Source
{
    static class Intelligence
    {
        public const int maxDepth = 2;
        public static int aiPlayer;
        public static int aiOpponent;
        public static EvalParameters evalParams = 
            new EvalParameters(0, 1.0f, 0);

        public static int negas = 0;

        public static Move chooseMove(Board board, int player)
        {
            HashSet<Move> moves = board.getPlayerMoves(player);
            aiPlayer = player;
            aiOpponent = (player == 1) ? 2 : 1;
            Move bestMove = moves.First();
            float bestValue = float.MinValue;
            foreach (Move m in moves)
            {
                Move move = m;
                board.makeMove(ref move);
                float negamaxValue = negamax(board, maxDepth, float.MinValue, float.MaxValue, 1);
                board.undoMove(m);

                if (negamaxValue >= bestValue)
                {
                    bestValue = negamaxValue;
                    bestMove = move;
                }
            }
            return bestMove;
        }

        public static float negamax(
            Board node, int depth, float alpha, float beta, int color)
        {
            negas++;
            if (depth == 0 || node.gameOver() != 0)
            {
                return color*BoardEvaluator.evaluateBoard(evalParams, node, aiPlayer, true);
            }
            
            int player = (color == 1) ? aiPlayer : aiOpponent;
            HashSet<Move> moves = node.getPlayerMoves(player);
            float bestValue = float.MinValue;
            foreach (Move m in moves)
            {
                Move move = m;
                node.makeMove(ref move);
                float val = -negamax(node, depth - 1, -beta, -alpha, -color);
                node.undoMove(move);
                bestValue = Math.Max(bestValue, val);
                alpha = Math.Max(alpha, val);
                if (alpha >= beta)
                    break;
            }
            return bestValue;
        }
    }
}
