using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class WinAnalyzer
{    
    public static List<int> CheckForWin(List<int> cellsClaimed)
    {
        EnsureWinPatternsAreLoaded();

        foreach (List<int> pattern in _winPatterns)
        {
            if (!pattern.Except(cellsClaimed).Any())
                return pattern;
        }

        return new List<int>();
    }

    private static bool _initialized = false;
    private static List<List<int>> _winPatterns = new List<List<int>>();   // Follow the same numbering scheme for the grid: left to right, top to bottom            

    private static void EnsureWinPatternsAreLoaded()
    {
        if (!_initialized)
            LoadWinPatterns();
    }

    // See the game board cell indexing definition in the GameBoardManager class to understand the integer values
    private static void LoadWinPatterns()
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
}