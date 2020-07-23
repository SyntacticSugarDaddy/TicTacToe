using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    [HideInInspector]
    public bool GameOver = false;

    public static event Action<int, bool> GameStartedEvent; // int => number of human players, bool => human player's turn to start
    public static event Action<int> PlayerMovedEvent; // int => number of human players
    public static event Action<string> GameCompletedEvent;  // string => The win message that should be displayed

    public static readonly int NUM_PLAYERS = 2;

    public ColorsConfig Colors;

    public Player[] Players { get; private set; }
    
    private enum GameStates
    {
        WAITING_FOR_PLAYER_1,
        WAITING_FOR_PLAYER_2,
        GAME_OVER
    }    

    [SerializeField]
    private GameBoardManager _board;

    private int _numMoves;
    private int _numHumanPlayers;
    private GameStates _gameState;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();

            return _instance;
        }        
    }

    public Player GetActivePlayer()
    {
        if (_gameState == GameStates.WAITING_FOR_PLAYER_1)
            return Players[0];

        if (_gameState == GameStates.WAITING_FOR_PLAYER_2)
            return Players[1];

        UnityEngine.Debug.LogError("Unexpected case");
        return null;
    }

    public void StartNewGame()
    {
        //UnityEngine.Debug.Log("StartNewGame()");        
        //_winnerText.text = "";
        GameOver = false;
        _numMoves = 0;        
        _numHumanPlayers = PlayerPrefs.GetInt("numPlayers");
        _gameState = GameStates.WAITING_FOR_PLAYER_1;
        CreatePlayers();
        //UpdateTurnDisplay();
        bool humansTurn = Players[0].Type == Player.ControlType.HUMAN;
        GameStartedEvent(_numHumanPlayers, humansTurn);

        if (_numHumanPlayers == 1 && !humansTurn)        
            StartCoroutine("MakeAiMove");
    }

    public List<string> GetPlayerSymbols()
    {
        List<string> symbols = new List<string>();

        foreach (Player player in Players)
            symbols.Add(player.Symbol.Text);

        return symbols;
    }

    private void CreatePlayers()
    {       
        Players = PlayersFactory.GetPlayers(_numHumanPlayers);
    }

    public void Quit()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator MakeAiMove()
    {
        if (_numHumanPlayers != 1)
        {
            UnityEngine.Debug.LogError("Unexpected case.");
            yield break;
        }

        //UnityEngine.Debug.LogFormat("MakeAiMove(), gameState = {0}", _gameState);        
        yield return new WaitForSeconds(2.0f);  // Delay to mimick a player thinking (computer processing)
        //UnityEngine.Debug.Log("After yield return waitforseconds");
        List<int> freeCells = _board.AvailableCells;
        int randomCell = UnityEngine.Random.Range(0, freeCells.Count - 1);
        //UnityEngine.Debug.LogFormat("MakeAiMove(): Claiming cell {0} for computer", freeCells[randomCell]);
        _board.ClaimCell(freeCells[randomCell], false);
    }

    private void OnDestroy()
    {        
        GameBoardManager.MoveMade -= OnMoveMade;    // This is needed or game crashes when you reload Game scene again after leaving
    }

    private void OnMoveMade(int cellClaimed, string winningSymbol)
    {
        //UnityEngine.Debug.LogFormat("OnMoveMade({0}, {1})", cellClaimed, winner);
        ++_numMoves;

        if (_numMoves == GameBoardManager.NUM_CELLS)
            GameOver = true;

        bool gameWon = !string.IsNullOrEmpty(winningSymbol);
        string gameResultMsg = "";

        if (gameWon)
        {
            GameOver = true;
            gameResultMsg += GetWinnerNameFromSymbol(winningSymbol) + " wins!";            
        }
       
        if (GameOver)
            EndGame(gameWon ? gameResultMsg : "Tie game");
        else        
            ProceedToNextMove();        
    }    

    private void EndGame(string gameResultMsg)
    {
        _gameState = GameStates.GAME_OVER;
        GameCompletedEvent(gameResultMsg);
    }

    private void ProceedToNextMove()
    {        
        IncrementActiveGameState();
        PlayerMovedEvent(_numHumanPlayers);        

        if (_numHumanPlayers == 1)
        {
            if (GetActivePlayer().Type == Player.ControlType.AI)
                StartCoroutine("MakeAiMove");
        }
    }

    private string GetWinnerNameFromSymbol(string winSymbol)
    {
        if (_numHumanPlayers == 1)
        {
            foreach (Player player in Players)
            {
                if (player.Symbol.Text == winSymbol)
                    return player.Type == Player.ControlType.HUMAN ? "Player" : "Computer";
            }
        }
        else
            return winSymbol;

        return "ERROR";
    }

    private void IncrementActiveGameState()
    {
        if (_gameState == GameStates.WAITING_FOR_PLAYER_1)        
            _gameState = GameStates.WAITING_FOR_PLAYER_2;        
        else if (_gameState == GameStates.WAITING_FOR_PLAYER_2)        
            _gameState = GameStates.WAITING_FOR_PLAYER_1;        
        else
            UnityEngine.Debug.LogError("Improper usage");
    }    
    
    private void Start()
    {        
        GameBoardManager.MoveMade += OnMoveMade;
        StartNewGame();        
    }    
}