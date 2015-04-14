using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LinesOfAction.Source
{
    static class Util
    {
        public static Random random = new Random();
        public static Direction reverseDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.NorthEast:
                    return Direction.SouthWest;
                case Direction.East:
                    return Direction.West;
                case Direction.SouthEast:
                    return Direction.NorthWest;
                case Direction.South:
                    return Direction.North;
                case Direction.SouthWest:
                    return Direction.NorthEast;
                case Direction.West:
                    return Direction.East;
                case Direction.NorthWest:
                    return Direction.SouthEast;
                default:
                    return Direction.None;
            }
        }

        public static Move chooseMoveAtRandom(HashSet<Move> moves)
        {
            return moves.ElementAt(random.Next(moves.Count));
        }

        public static int manhattanDistance(Coord a, Coord b)
        {
            int dr = Math.Abs(a.row - b.row);
            int dc = Math.Abs(a.col - b.col);
            return dr + dc;
        }
    }
}
