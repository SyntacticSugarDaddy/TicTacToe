using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{    
    public void Reset()
    {
        SetSymbol("", UnityEngine.Color.white); // Color doesn't matter since the text is empty
        Highlight(false);
    }

    public void SetSymbol(string symbol, UnityEngine.Color color)
    {        
        _symbol.text = symbol;
        _symbol.color = color;
    }

    public void Highlight(bool active)
    {
        if (_background != null)
        {
            if (active)
                _background.color = GameManager.Instance.Colors.CellHighlightColor;
            else
                _background.color = GameManager.Instance.Colors.CellDefaultColor;
        }
        else
            Debug.LogError("_background is NULL");
    }

    private Image _background;
    private TextMeshProUGUI _symbol;

    private void Awake()
    {        
        _background = GetComponent<Image>();
        _symbol = GetComponentInChildren<TextMeshProUGUI>();
        
        if (_background == null || _symbol == null)
            Debug.LogError("Background or symbol not set in inspector");
    }
}