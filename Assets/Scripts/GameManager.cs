using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    //[HideInInspector]
    //public bool HumansTurn { get; private set; }

    [HideInInspector]
    public bool GameOver = false;

    public static event Action GameStarted;

    public static readonly int NUM_PLAYERS = 2;

    public ColorsConfig Colors;

    private enum GameStates
    {
        WAITING_FOR_PLAYER_1,
        WAITING_FOR_PLAYER_2,
        GAME_OVER
    }

    [SerializeField]
    private TextMeshProUGUI _turnText;

    [SerializeField]
    private TextMeshProUGUI _winnerText;

    [SerializeField]
    private Button _newGameButton;

    [SerializeField]
    private GameBoardManager _board;

    private int _numMoves;
    private int _numHumanPlayers;
    private GameStates _gameState;

    public Player[] Players { get; private set; }

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
        _winnerText.text = "";
        GameOver = false;
        _numMoves = 0;        
        _numHumanPlayers = PlayerPrefs.GetInt("numPlayers");
        _gameState = GameStates.WAITING_FOR_PLAYER_1;
        CreatePlayers();
        UpdateTurnDisplay();

        //if (_numHumanPlayers == 2)
        //    HumansTurn = true;
        //else
        //    HumansTurn = Players[0].Type == Player.ControlType.HUMAN;

        GameStarted();        

        if (_numHumanPlayers == 1 && Players[0].Type != Player.ControlType.HUMAN)
        {            
            StartCoroutine("MakeAiMove");
        }        
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

    private void OnMoveMade(int cellClaimed, string winner)
    {
        //UnityEngine.Debug.LogFormat("OnMoveMade({0}, {1})", cellClaimed, winner);
        ++_numMoves;

        if (_numMoves == GameBoardManager.NUM_CELLS)
            GameOver = true;

        bool gameWon = !string.IsNullOrEmpty(winner);

        if (gameWon)
        {
            GameOver = true;
            _winnerText.text = winner + " wins!";
        }
       
        if (GameOver)
        {
            //_newGameButton.gameObject.active = true;
            _turnText.text = "";
            _gameState = GameStates.GAME_OVER;
            //UnityEngine.Debug.Log("GAME OVER CASE");

            if (!gameWon)
                _winnerText.text = "Tie game";
        }
        else
        {
            IncrementActiveGameState();            
            UpdateTurnDisplay();

            if (_numHumanPlayers == 1)
            {
                //HumansTurn = !HumansTurn;

                //if (!HumansTurn)
                if (GetActivePlayer().Type == Player.ControlType.AI)
                    StartCoroutine("MakeAiMove");
            }            
        }

        //UnityEngine.Debug.LogFormat("gameState: {0}", _gameState);
    }    

    //private IEnumerator MimickPlayerThinkingDelay()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    //UnityEngine.Debug.Log("Timer up");
    //}

    private void IncrementActiveGameState()
    {
        if (_gameState == GameStates.WAITING_FOR_PLAYER_1)
        {
            _gameState = GameStates.WAITING_FOR_PLAYER_2;
        }
        else if (_gameState == GameStates.WAITING_FOR_PLAYER_2)
        {
            _gameState = GameStates.WAITING_FOR_PLAYER_1;
        }
        else
            UnityEngine.Debug.LogError("Improper usage");
    }

    private void UpdateTurnDisplay()
    {
        if (_gameState == GameStates.WAITING_FOR_PLAYER_1)
        {
            if (_numHumanPlayers == 1)
                _turnText.text = Players[0].Type == Player.ControlType.HUMAN ? "Turn: Player" : "Turn: Computer";
            else
                _turnText.text = string.Format("Turn: {0}", Players[0].Symbol.Text);
        }
        else if (_gameState == GameStates.WAITING_FOR_PLAYER_2)
        {
            if (_numHumanPlayers == 1)
                _turnText.text = Players[1].Type == Player.ControlType.HUMAN ? "Turn: Player" : "Turn: Computer";
            else
                _turnText.text = string.Format("Turn: {0}", Players[1].Symbol.Text);
        }
        else if (_gameState == GameStates.GAME_OVER)
            _turnText.text = "";
    }
    
    private void Start()
    {        
        if (_turnText == null || _winnerText == null)
            UnityEngine.Debug.LogError("Menu texts not fully set");

        GameBoardManager.MoveMade += OnMoveMade;
        StartNewGame();        
    }    
}