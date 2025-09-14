using SplashKitSDK;

namespace TicTacToe
{
    public static class Program
    {
        public static void Main()
        {
            SplashKit.LoadFont("Arial", @"C:\\msys64\\home\\oshad\\TicTacToe\\ARIAL.TTF");
            Window w = new("Tic Tac Toe", 600, 700);

            //  Just create the game, no console input needed
            Game game = new Game();

            while (!w.CloseRequested)
            {
                SplashKit.ProcessEvents();
                game.Update();

                w.Clear(Color.DarkOliveGreen);
                game.Draw(w);
                w.Refresh(60);
            }
        }
    }
}
