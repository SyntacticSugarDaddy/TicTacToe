using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;
using UnityEngine.Events;

//[System.Serializable]
//public class CellClickedEvent : UnityEvent<int, bool>
//{

//}

// Cells are numbered from 0 to 8, starting from top left square: left to right, top to bottom

public class GameBoardManager : MonoBehaviour
{
    //public CellClickedEvent ClickEvent;

    public static event Action<int, string> MoveMade;   // Args are: cell selected, text describing who won
    
    public const int NUM_CELLS = 9;

    public List<int> AvailableCells { get; private set; }
    
    [SerializeField]
    private List<Cell> _cells = new List<Cell>();
    
    private Dictionary<string, List<int>> _symbolToClaimedCells = new Dictionary<string, List<int>>();

    private List<List<int>> _winPatterns = new List<List<int>>();   // Follow the same numbering scheme for the grid: left to right, top to bottom    

    public bool CellAvailable(int cellIndex)
    {
        return AvailableCells.Contains(cellIndex);
    }

    public void ClaimCellForHuman(int cellIndex)
    {        
        ClaimCell(cellIndex, true);
    }

    public void ClaimCell(int cellIndex, bool forHuman) // If not forHuman, then request is on behalf of an AI player
    {
        //UnityEngine.Debug.LogFormat("ClaimCell(), gameOver: {0}, cellAvailable: {1}", GameManager.Instance.GameOver, CellAvailable(cellIndex));                

        if (!GameManager.Instance.GameOver && CellAvailable(cellIndex) && IsPlayersTurn(forHuman))
        {
            ProcessMove(cellIndex);
            string winner = CheckForWinner();

            if (!string.IsNullOrEmpty(winner))            
                GameManager.Instance.GameOver = true;            

            MoveMade(cellIndex, winner);
        }        
    }

    private bool IsPlayersTurn(bool human)  // !human is inquiring if it is the AIs turn
    {
        Player player = GameManager.Instance.GetActivePlayer();

        if (human && player.Type == Player.ControlType.HUMAN)
            return true;

        if (!human && player.Type == Player.ControlType.AI)
            return true;

        return false;
    }

    private void ProcessMove(int cellIndex)
    {
        Player player = GameManager.Instance.GetActivePlayer();
        string symbol = player.Symbol.Text;
        _cells[cellIndex].SetSymbol(symbol, player.Symbol.Color);
        AvailableCells.Remove(cellIndex);
        _symbolToClaimedCells[symbol].Add(cellIndex);        
    }

    private void Awake()
    {
        AvailableCells = new List<int>();
        LoadWinPatterns();
        GameManager.GameStarted += OnGameStarted;
    }

    private void Start()
    {
        if (_cells.Count != NUM_CELLS)
            UnityEngine.Debug.LogError("Cells were not initialized.");

        //ClickEvent.AddListener(ClaimCell);
    }

    private void OnDestroy()
    {
        GameManager.GameStarted -= OnGameStarted;
    }

    private void PopulateSymbolToCellsMap()
    {
        _symbolToClaimedCells.Clear();

        foreach (Player player in GameManager.Instance.Players)
            _symbolToClaimedCells.Add(player.Symbol.Text, new List<int>());
    }

    private void OnGameStarted()
    {
        foreach (Cell cell in _cells)
        {
            cell.Highlight(false);
            cell.ClearSymbol();
        }
        
        AvailableCells.Clear();

        for (int i = 0; i != NUM_CELLS; ++i)
            AvailableCells.Add(i);

        PopulateSymbolToCellsMap();
    }

    private void LoadWinPatterns()
    {
        _winPatterns.Add(new List<int> { 0, 1, 2 });    // top row
        _winPatterns.Add(new List<int> { 3, 4, 5 });    // middle row
        _winPatterns.Add(new List<int> { 6, 7, 8 });    // bottom row
        _winPatterns.Add(new List<int> { 0, 3, 6 });    // left column
        _winPatterns.Add(new List<int> { 1, 4, 7 });    // center column
        _winPatterns.Add(new List<int> { 2, 5, 8 });    // right column
        _winPatterns.Add(new List<int> { 0, 4, 8 });    // diagonal 1
        _winPatterns.Add(new List<int> { 6, 4, 2 });    // diagonal 2
    }

    private string CheckForWinner()
    {
        List<string> symbols = GameManager.Instance.GetPlayerSymbols();

        foreach (string symbol in symbols)
        {
            if (SymbolWins(symbol))
                return symbol;
        }        

        return "";
    }

    private bool SymbolWins(string symbol)
    {
        List<int> playedMovesForSymbol = GetMovesForSymbol(symbol);

        foreach (List<int> pattern in _winPatterns)
        {
            if (!pattern.Except(playedMovesForSymbol).Any())
            {
                foreach (int cellIndex in pattern)
                    _cells[cellIndex].Highlight(true);

                return true;
            }
        }

        return false;
    }

    private List<int> GetMovesForSymbol(string symbol)
    {        
        return _symbolToClaimedCells[symbol];
    }
}