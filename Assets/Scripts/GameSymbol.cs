using UnityEngine;

public class GameSymbol
{
    public GameSymbol(string text, UnityEngine.Color color)
    {
        Text = text;
        Color = color;
    }
    
    public string Text { get; private set; }  // e.g. "X", "O"
    public UnityEngine.Color Color { get; private set; }    // What to color the symbol- I'm using blue and red to start
}
