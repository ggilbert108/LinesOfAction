using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinesOfAction.Source
{
    class Board
    {
        public static int SIZE = 8;
        public int[,] board { get; set; }

        public Board()
        {
            int[,] tempBoard =
            {
                {0, 1, 1, 1, 1, 1, 1, 0},
                {2, 0, 0, 0, 0, 0, 0, 2},
                {2, 0, 0, 0, 0, 0, 0, 2},
                {2, 0, 0, 0, 0, 0, 0, 2},
                {2, 0, 0, 0, 0, 0, 0, 2},
                {2, 0, 0, 0, 0, 0, 0, 2},
                {2, 0, 0, 0, 0, 0, 0, 2},
                {0, 1, 1, 1, 1, 1, 1, 0},
            };
            board = tempBoard;
        }

        public HashSet<Move> getPlayerMoves(int player)
        {
            HashSet<Move> moves = new HashSet<Move>();
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (board[i, j] == player)
                    {
                        HashSet<Move> pieceMoves = getPieceMoves(new Coord(i, j), player);
                        moves.UnionWith(pieceMoves);
                    }
                }
            }
            return moves;
        }

        public void makeMove(ref Move move)
        {
            move.fromInitial = getPiece(move.from);
            move.toInitial = getPiece(move.to);
            setPiece(move.from, 0);
            setPiece(move.to, move.fromInitial);
        }

        public void undoMove(Move move)
        {
            if (move.toInitial < 0)
                return;

            setPiece(move.from, move.fromInitial);
            setPiece(move.to, move.toInitial);
        }

        public int gameOver()
        {
            if (gameOver(1))
                return 1;
            if (gameOver(2))
                return 2;
            return 0;
        }

        private bool gameOver(int player)
        {
            int pieceCount = getPieceCount(player);
            if (pieceCount == 1)
            {
                return true;
            }
            HashSet<Coord> breadCrumbs = new HashSet<Coord>();
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    Coord coord = new Coord(i, j);
                    if (board[i, j] == player && !breadCrumbs.Contains(coord))
                    {
                        int groupSize = floodFill(coord, breadCrumbs);
                        if (groupSize == pieceCount)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private int floodFill(Coord coord, HashSet<Coord> breadCrumbs)
        {
            if (!inBounds(coord) || breadCrumbs.Contains(coord))
            {
                return 0;
            }

            breadCrumbs.Add(coord);
            int total = 1;

            foreach (Direction direction in Enum.GetValues(typeof (Direction)))
            {
                if (direction == Direction.None)
                    continue;
                Coord neighbor = coord.getNeighbor(direction, 1);
                total += floodFill(neighbor, breadCrumbs);
            }
            return total;
        }

        private int getPieceCount(int player)
        {
            int count = 0;
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (board[i, j] == player)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private HashSet<Move> getPieceMoves(Coord coord, int player)
        {
            HashSet<Move> moves = new HashSet<Move>();
            int enemy = (player == 1) ? 2 : 1;

            foreach (Direction direction in Enum.GetValues(typeof (Direction)))
            {
                if (direction == Direction.None)
                    continue;

                int moveDistance = countPiecesOnLine(coord, direction);
                Coord toCoord = new Coord(coord.row, coord.col);
                bool canMove = true;
                for (int i = 0; i < moveDistance - 1; i++)
                {
                    toCoord = toCoord.getNeighbor(direction, 1);
                    if (!inBounds(toCoord) || board[toCoord.row, toCoord.col] == enemy)
                    {
                        canMove = false;
                        break;
                    }
                }

                if (!canMove)
                    continue;

                toCoord = toCoord.getNeighbor(direction, 1);

                if (!inBounds(toCoord))
                    continue;
                
                if (board[toCoord.row, toCoord.col] == player)
                    continue;
                
                moves.Add(new Move(coord, toCoord));
            }
            return moves;
        }

        private int countPiecesOnLine(Coord coord, Direction direction)
        {
            Direction opposite = Util.reverseDirection(direction);
            Coord a = coord.getNeighbor(direction, 1);
            Coord b = coord.getNeighbor(opposite, 1);
            int result = countPiecesInDirection(a, direction);
            result += countPiecesInDirection(b, opposite);
            if (isPiece(coord))
                result += 1;
            return result;
        }

        private int countPiecesInDirection(Coord coord, Direction direction)
        {
            if (!inBounds(coord))
            {
                return 0;
            }
            int piece = (isPiece(coord)) ? 1 : 0;
            return piece + countPiecesInDirection(coord.getNeighbor(direction, 1), direction);
        }

        private int getPiece(Coord coord)
        {
            return board[coord.row, coord.col];
        }

        private void setPiece(Coord coord, int player)
        {
            if (!inBounds(coord))
                return;
            board[coord.row, coord.col] = player;
        }

        private bool isPiece(Coord coord)
        {
            if (!inBounds(coord))
                return false;
            return board[coord.row, coord.col] != 0;
        }

        private bool inBounds(Coord coord)
        {
            return coord.row >= 0
                   && coord.col >= 0
                   && coord.row < SIZE
                   && coord.col < SIZE;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    result += board[i, j];
                }
                result += "\n";
            }
            return result;
        }
    }

    struct Coord
    {
        public readonly int row, col;

        public Coord(int r, int c)
        {
            row = r;
            col = c;
        }

        public Coord getNeighbor(Direction direction, int distance)
        {
            int r = row;
            int c = col;
            switch (direction)
            {
                case Direction.North:
                    r -= distance;
                    break;
                case Direction.NorthEast:
                    r -= distance;
                    c += distance;
                    break;
                case Direction.East:
                    c += distance;
                    break;
                case Direction.SouthEast:
                    r += distance;
                    c += distance;
                    break;
                case Direction.South:
                    r += distance;
                    break;
                case Direction.SouthWest:
                    r += distance;
                    c -= distance;
                    break;
                case Direction.West:
                    c -= distance;
                    break;
                case Direction.NorthWest:
                    r -= distance;
                    c -= distance;
                    break;
            }
            return new Coord(r, c);
        }

        public override int GetHashCode()
        {
            return 51*row + 37*col;
        }

        public override string ToString()
        {
            return row + " " + col;
        }
    }

    struct Move
    {
        public readonly Coord from;
        public readonly Coord to;

        public int toInitial, fromInitial;

        public Move(Coord f, Coord t)
        {
            from = f;
            to = t;
            toInitial = -1;
            fromInitial = -1;
        }

        public override int GetHashCode()
        {
            return 51*from.GetHashCode() + 37*to.GetHashCode();
        }
    }

    enum Checker {None, Red, Black}

    enum Direction
    {
        None,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
}
