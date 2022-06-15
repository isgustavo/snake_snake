using TMPro;
using UnityEngine;

public  class UIHoldInputView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text holdInputText;

    public void Show(PlayerEnterInput playerInput)
    {
        holdInputText.text = $"keep holding on <{playerInput.leftInputKeyName.ToUpper()}> and <{playerInput.rightInputKeyName.ToUpper()}>";
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
