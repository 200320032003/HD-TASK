using System;

namespace TicTacToe
{
    public class AIPlayer : IPlayer
    {
        public char Symbol { get; }

        public AIPlayer(char symbol)
        {
            Symbol = symbol;
        }

        public void MakeMove(Board board)
        {
            int bestScore = int.MinValue;
            (int row, int col) bestMove = (-1, -1);

            foreach (var (r, c) in board.EmptyCells())
            {
                board.PlaceSymbol(r, c, Symbol);
                int score = Minimax(board, 0, false);
                board.UndoMove(r, c);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (r, c);
                }
            }

            if (bestMove != (-1, -1))
            {
                board.PlaceSymbol(bestMove.row, bestMove.col, Symbol);
            }
        }

        private int Minimax(Board board, int depth, bool isMaximizing)
        {
            char opponent = (Symbol == 'O') ? 'X' : 'O';

            if (board.HasWinner(Symbol)) return 10 - depth;
            if (board.HasWinner(opponent)) return depth - 10;
            if (board.IsDraw()) return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                foreach (var (r, c) in board.EmptyCells())
                {
                    board.PlaceSymbol(r, c, Symbol);
                    int score = Minimax(board, depth + 1, false);
                    board.UndoMove(r, c);
                    bestScore = Math.Max(bestScore, score);
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (var (r, c) in board.EmptyCells())
                {
                    board.PlaceSymbol(r, c, opponent);
                    int score = Minimax(board, depth + 1, true);
                    board.UndoMove(r, c);
                    bestScore = Math.Min(bestScore, score);
                }
                return bestScore;
            }
        }
    }
}
