using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static event Action<int, bool> GameStartedEvent; // int => number of human players, bool => human player's turn to start
    public static event Action<int> PlayerMovedEvent; // int => number of human players
    public static event Action<Player, int> GameCompletedEvent;  // Player => the winner, int => the num human players in the game

    public static readonly int NUM_PLAYERS = 2;

    public ColorsConfig Colors;

    public Player[] Players { get; private set; }
        
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
        if (_gameState == GameState.WAITING_FOR_PLAYER_1)
            return Players[0];

        if (_gameState == GameState.WAITING_FOR_PLAYER_2)
            return Players[1];
        
        return null;
    }

    public Player GetWinningPlayer()
    {
        if (_gameState == GameState.GAME_OVER_PLAYER_1_WIN)
            return Players[0];

        if (_gameState == GameState.GAME_OVER_PLAYER_2_WIN)
            return Players[1];

        return null;
    }

    public void StartNewGame()
    {
        _numMoves = 0;        
        _numHumanPlayers = PlayerPrefs.GetInt("numPlayers");
        _gameState = GameState.WAITING_FOR_PLAYER_1;
        CreatePlayers();
        bool humansTurn = Players[0].Type == Player.ControlType.HUMAN;
        GameStartedEvent(_numHumanPlayers, humansTurn);

        if (_numHumanPlayers == 1 && !humansTurn)        
            StartCoroutine("MakeAiMove");
    }

    public List<string> GetPlayerSymbols()
    {
        return Players.Select(p => p.Symbol.Text).ToList();
        //List<string> symbols = new List<string>();

        //foreach (Player player in Players)
        //    symbols.Add(player.Symbol.Text);

        //return symbols;
    }

    /// <summary>    
    /// </summary>
    /// <param name="human">
    /// human => inquiring if it is a human player's turn
    /// !human => inquiring if it is the AIs turn
    /// </param>
    /// <returns>Whether the player type specified by the param matches the active player type</returns>

    public bool IsPlayersTurn(bool human) 
    {        
        if (human && GetActivePlayer().Type == Player.ControlType.HUMAN)
            return true;

        if (!human && GetActivePlayer().Type == Player.ControlType.AI)
            return true;

        return false;
    }

    private enum GameState
    {
        WAITING_FOR_PLAYER_1,
        WAITING_FOR_PLAYER_2,
        GAME_OVER_PLAYER_1_WIN,
        GAME_OVER_PLAYER_2_WIN,
        GAME_OVER_TIE,
    }

    [SerializeField]
    private GameBoardManager _board;
    
    private int _numMoves;
    private int _numHumanPlayers;
    private GameState _gameState;

    private static GameManager _instance;

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
            Debug.LogError("Unexpected case.");
            yield break;
        }
        
        yield return new WaitForSeconds(2.0f);  // Delay to mimick a player thinking (computer processing)        
        List<int> freeCells = _board.AvailableCells;
        int randomCell = UnityEngine.Random.Range(0, freeCells.Count - 1);     
        _board.ClaimCell(freeCells[randomCell], false);
    }

    private void OnDestroy()
    {        
        GameBoardManager.MoveMadeEvent -= OnMoveMade;    // This is needed or game crashes when you reload Game scene again after leaving
    }

    private void OnMoveMade(int cellClaimed, bool gameWon)
    {        
        ++_numMoves;        
        bool gameOver = gameWon || _numMoves == GameBoardManager.NUM_CELLS;
        string gameResultMsg = "";

        if (gameWon)
            _gameState = _numMoves % 2 == 0 ? GameState.GAME_OVER_PLAYER_2_WIN : GameState.GAME_OVER_PLAYER_1_WIN;
        else if (gameOver)
            _gameState = GameState.GAME_OVER_TIE;
       
        if (gameOver)
            GameCompletedEvent(GetWinningPlayer(), _numHumanPlayers);             
        else        
            ProceedToNextMove();        
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

    private void IncrementActiveGameState()
    {
        if (_gameState == GameState.WAITING_FOR_PLAYER_1)        
            _gameState = GameState.WAITING_FOR_PLAYER_2;        
        else if (_gameState == GameState.WAITING_FOR_PLAYER_2)        
            _gameState = GameState.WAITING_FOR_PLAYER_1;        
        else
            Debug.LogError("Improper usage");
    }    
    
    private void Start()
    {        
        GameBoardManager.MoveMadeEvent += OnMoveMade;
        StartNewGame();        
    }    
}