using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerListViewCell : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerText;
    [SerializeField]
    private Image playerColor;

    public void Show(ControllablePlayer player)
    {
        playerText.text = $"Player {(player.PlayerId + 1).ToString("00")} :: <{player.InputHandler.LeftInput.ToUpper()}> and <{player.InputHandler.RightInput.ToUpper()}>";
        playerColor.color = player.Color;
    }
}
