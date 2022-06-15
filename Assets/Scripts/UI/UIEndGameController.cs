using TMPro;
using UnityEngine;

public class UIEndGameController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text winnerInputText;

    private void OnEnable()
    {
        if(ManagerLocator.TryGetManager(out EndGameManagerBehaviour endGameManager))
        {
            winnerInputText.text = $"Player {(endGameManager.WinnerPlayerId + 1).ToString("00")} is the winner!";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        if (ManagerLocator.TryGetManager(out EndGameManagerBehaviour endGameManager))
        {
            endGameManager.BackToMainMenu();
        }
    }
}
