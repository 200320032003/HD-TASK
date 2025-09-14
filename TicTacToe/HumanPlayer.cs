using SplashKitSDK;

namespace TicTacToe
{
    public class HumanPlayer : IPlayer
    {
        public char Symbol { get; }
        private bool _clicked;

        public HumanPlayer(char symbol)
        {
            Symbol = symbol;
        }

        public void MakeMove(Board board)
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton) && !_clicked)
            {
                if (SplashKit.MouseY() < 600) // Only inside board
                {
                    int row = (int)(SplashKit.MouseY() / board.CellSize);
                    int col = (int)(SplashKit.MouseX() / board.CellSize);

                    if (board.PlaceSymbol(row, col, Symbol))
                    {
                        _clicked = true;
                    }
                }
            }

            if (!SplashKit.MouseDown(MouseButton.LeftButton))
            {
                _clicked = false; // reset
            }
        }
    }
}
