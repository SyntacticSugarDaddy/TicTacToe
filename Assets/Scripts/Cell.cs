using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Cell : MonoBehaviour
{
    private Image _background;    
    private TextMeshProUGUI _symbol;

    public void ClearSymbol()
    {
        _symbol.text = "";
    }

    public void SetSymbol(string symbol, UnityEngine.Color color)
    {
        //UnityEngine.Debug.LogFormat("symbol: {0}, color: {1}", symbol, color);
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
            UnityEngine.Debug.LogError("_background is NULL");
    }

    private void Awake()
    {
        //UnityEngine.Debug.Log("Cell.cs: Awake()");
        _background = GetComponent<Image>();
        _symbol = GetComponentInChildren<TextMeshProUGUI>();
        
        if (_background == null || _symbol == null)
            UnityEngine.Debug.LogError("Background or symbol not set in inspector");
    }
}