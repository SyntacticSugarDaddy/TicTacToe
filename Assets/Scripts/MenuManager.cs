using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{    
    public void LoadGame(int numPlayers)
    {
        if (numPlayers != 1 && numPlayers != 2)
            UnityEngine.Debug.LogErrorFormat("Invalid number of players: {0}", numPlayers);

        PlayerPrefs.SetInt("numPlayers", numPlayers);
        SceneManager.LoadScene("Game");
    }
}