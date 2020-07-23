using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{    
    [SerializeField]
    private TextMeshProUGUI _turnText;

    [SerializeField]
    private TextMeshProUGUI _winnerText;

    private void Reset(int numHumanPlayers, bool humansTurn)
    {
        _winnerText.text = "";
        _turnText.text = string.Format("Turn: {0}", numHumanPlayers == 1 ? (humansTurn ? "Player" : "Computer") : PlayersFactory.PLAYER_1_SYMBOL);
    }
    
    private void Awake()
    {
        Image img = GetComponent<Image>();

        if (img != null)
            img.color = GameManager.Instance.Colors.MenuColor;
        else
            Debug.LogError("Failed to get image component on game menu");

        GameManager.GameStartedEvent += Reset;
        GameManager.PlayerMovedEvent += UpdateTurnDisplay;
        GameManager.GameCompletedEvent += DisplayGameResult;
    }

    private void UpdateTurnDisplay(int numHumanPlayers)
    {
        Player player = GameManager.Instance.GetActivePlayer();

        if (numHumanPlayers == 1)
            _turnText.text = player.Type == Player.ControlType.HUMAN ? "Turn: Player" : "Turn: Computer";
        else
            _turnText.text = string.Format("Turn: {0}", player.Symbol.Text);        
    }

    private void DisplayGameResult(string winMessage)
    {
        _turnText.text = "";        
        _winnerText.text = winMessage;
    }

    private void OnDestroy()
    {
        GameManager.GameStartedEvent -= Reset;
        GameManager.PlayerMovedEvent -= UpdateTurnDisplay;
        GameManager.GameCompletedEvent -= DisplayGameResult;
    }
}