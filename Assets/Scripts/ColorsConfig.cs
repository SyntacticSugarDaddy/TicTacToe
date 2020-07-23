using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Colors", menuName = "ScriptableObjects/ColorsConfig", order = 1)]
public class ColorsConfig : ScriptableObject
{
    public UnityEngine.Color32 MenuColor;
    public UnityEngine.Color32 GameBackgroundColor;
    public UnityEngine.Color32 CellDefaultColor;
    public UnityEngine.Color32 CellHighlightColor;
    public UnityEngine.Color32 Player1Color;
    public UnityEngine.Color32 Player2Color;
}