using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics;

public class Cell : MonoBehaviour
{
    private Image _background;    
    private TextMeshProUGUI _symbol;

    private static readonly UnityEngine.Color CELL_HIGHLIGHT_COLOR = UnityEngine.Color.yellow;
    private static readonly UnityEngine.Color CELL_DEFAULT_COLOR = UnityEngine.Color.white;

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
                _background.color = CELL_HIGHLIGHT_COLOR;
            else
                _background.color = CELL_DEFAULT_COLOR;
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

    //private void Start()
    //{
    //    UnityEngine.Debug.Log("Cell.cs Start()");
    //}

    //private void OnDestroy()
    //{
    //    UnityEngine.Debug.Log("OnDestroy called from Cell class");
    //}
}