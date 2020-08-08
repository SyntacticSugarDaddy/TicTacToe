using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Security.Cryptography.X509Certificates;

// Cells are numbered from 0 to 8, starting from top left square and proceeding from left to right, top to bottom:
// [0][1][2]
// [3][4][5]
// [6][7][8]

public class GameBoardManager : MonoBehaviour
{
    public static event Action<int, bool> MoveMadeEvent;   // int => cell selected, bool => if the last move resulted in a win
    
    public const int NUM_CELLS = 9;

    public List<int> AvailableCells { get; private set; }

    public void ClaimCellForHuman(int cellIndex)
    {        
        ClaimCell(cellIndex, true);
    }

    public void ClaimCell(int cellIndex, bool forHuman) // If not forHuman, then request is on behalf of an AI player
    {        
        if (GameManager.Instance.GetActivePlayer() != null && AvailableCells.Contains(cellIndex) && 
            GameManager.Instance.IsPlayersTurn(forHuman))
        {
            string symbol = ProcessMove(cellIndex);
            List<int> winCells = WinAnalyzer.CheckForWin(_symbolToClaimedCells[symbol]);
            bool gameWon = winCells.Count > 0;

            if (gameWon)            
                HighlightCells(winCells);

            MoveMadeEvent(cellIndex, gameWon);
        }        
    }

    #region private
    [SerializeField]
    private Image _gameBackground;

    [SerializeField]
    private List<Cell> _cells = new List<Cell>();

    private Dictionary<string, List<int>> _symbolToClaimedCells = new Dictionary<string, List<int>>();

    //private void DisplayWin(List<int> winCells)
    //{
        //GameManager.Instance.GameOver = true;
        //winText = GameManager.Instance.GetActivePlayer().Symbol.Text;
        //HighlightCells(winCells);
    //}

    private void HighlightCells(List<int> cells)
    {
        //foreach (int cellIndex in cells)
          //  _cells[cellIndex].Highlight(true);

        cells.ForEach(index => _cells[index].Highlight(true));
    }

    /// <returns>the symbol of the player who moved</returns>
    private string ProcessMove(int cellIndex)
    {
        Player player = GameManager.Instance.GetActivePlayer();
        string symbol = player.Symbol.Text;
        _cells[cellIndex].SetSymbol(symbol, player.Symbol.Color);
        AvailableCells.Remove(cellIndex);
        _symbolToClaimedCells[symbol].Add(cellIndex);
        return symbol;
    }

    private void Awake()
    {
        AvailableCells = new List<int>();        
        GameManager.GameStartedEvent += OnGameStarted;

        if (_gameBackground == null)
            Debug.LogError("Game background image was not set in the inspector.");
        else
            _gameBackground.color = GameManager.Instance.Colors.GameBackgroundColor;
    }

    private void Start()
    {
        if (_cells.Count != NUM_CELLS)
            Debug.LogError("Cells were not initialized.");        
    }

    private void OnDestroy()
    {
        GameManager.GameStartedEvent -= OnGameStarted;
    }

    private void PopulateSymbolToCellsMap()
    {
        _symbolToClaimedCells.Clear();

        foreach (Player player in GameManager.Instance.Players)
            _symbolToClaimedCells.Add(player.Symbol.Text, new List<int>());
    }

    private void OnGameStarted(int numHumanPlayers, bool humansTurn)
    {
        ClearGameBoardCells();
        ResetAvailableCells();
        PopulateSymbolToCellsMap();
    }

    private void ClearGameBoardCells()
    {
        //foreach (Cell cell in _cells)        
          //  cell.Reset();

        _cells.ForEach(cell => cell.Reset());
    }

    private void ResetAvailableCells()
    {
        AvailableCells = Enumerable.Range(0, NUM_CELLS).ToList();
    }
    #endregion
}