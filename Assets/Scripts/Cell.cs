using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    private Image _background;    
    private TextMeshProUGUI _symbol;

    //public void ClearSymbol()
    //{
    //    _symbol.text = "";
    //}

    public void Reset()
    {
        SetSymbol("", UnityEngine.Color.white); // Color doesn't matter
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

    private void Awake()
    {        
        _background = GetComponent<Image>();
        _symbol = GetComponentInChildren<TextMeshProUGUI>();
        
        if (_background == null || _symbol == null)
            Debug.LogError("Background or symbol not set in inspector");
    }
}