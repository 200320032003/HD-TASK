namespace TicTacToe
{
    public interface IPlayer
    {
        char Symbol { get; }
        void MakeMove(Board board);
    }
}
