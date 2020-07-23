public class Player
{
    public Player(ControlType type, GameSymbol symbol)
    {
        Type = type;
        Symbol = symbol;
    }
    
    public enum ControlType
    {
        HUMAN,
        AI
    }

    public ControlType Type { get; private set; }
    public GameSymbol Symbol { get; private set; }
}