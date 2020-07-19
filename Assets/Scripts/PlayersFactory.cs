using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayersFactory
{
    public static Player[] GetPlayers(int numHumanPlayers)
    {
        if (_playerIndexToSymbol.Count == 0)        
            Initialize();

        if (numHumanPlayers == 2)
            return GetTwoPlayerGamePlayers();                    

        if (numHumanPlayers == 1)        
            return GetOnePlayerGamePlayers();
                
        UnityEngine.Debug.LogErrorFormat("Invalid parameters numHumanPlayers: {0}", numHumanPlayers);
        return new Player[2];
    }

    private static readonly string X_SYMBOL = "X";
    private static readonly string O_SYMBOL = "O";

    private static Dictionary<int, GameSymbol> _playerIndexToSymbol = new Dictionary<int, GameSymbol>();

    private static Player[] GetOnePlayerGamePlayers()
    {
        Player[] players = new Player[2];
        int humanIndex = UnityEngine.Random.Range(0, 2);
        int aiIndex = humanIndex == 1 ? 0 : 1;
        players[humanIndex] = new Player(Player.ControlType.HUMAN, _playerIndexToSymbol[humanIndex]);
        players[aiIndex] = new Player(Player.ControlType.AI, _playerIndexToSymbol[aiIndex]);
        return players;
    }

    private static Player[] GetTwoPlayerGamePlayers()
    {
        Player[] players = new Player[2];
        
        for (int i = 0; i != GameManager.NUM_PLAYERS; ++i)        
            players[i] = new Player(Player.ControlType.HUMAN, _playerIndexToSymbol[i]);

        return players;
    }

    private static void Initialize()
    {
        // I've decided player 1 is always X, player 2 is always O
        //UnityEngine.Debug.LogFormat("player1Color: {0}", GameManager.Instance.Colors.Player1Color);
        _playerIndexToSymbol.Add(0, new GameSymbol(X_SYMBOL, GameManager.Instance.Colors.Player1Color));
        _playerIndexToSymbol.Add(1, new GameSymbol(O_SYMBOL, GameManager.Instance.Colors.Player2Color));
    }
}