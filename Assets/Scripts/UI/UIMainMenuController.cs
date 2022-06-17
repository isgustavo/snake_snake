using System;
using UnityEngine;

public class PlayerEnterInput
{
    public string leftInputKeyName = string.Empty;
    public string rightInputKeyName = string.Empty;

    public void Clear()
    {
        leftInputKeyName = string.Empty;
        rightInputKeyName = string.Empty;
    }
}

public class UIMainMenuController : MonoBehaviour
{
    [SerializeField]
    private UIHoldInputView holdInputView;
    [SerializeField]
    private Transform playerListView;
    [SerializeField]
    private GameObject playerListCellView;

    private float holdDeltaTime;
    private PlayerEnterInput playerEnterInput = new PlayerEnterInput();

    private void OnEnable()
    {
        AddListener();
        holdInputView.Hide();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            TryStartGame();
        }

        if (Input.anyKeyDown)
        {
            if (IsHoldTwoKeys() == false)
            {
                if (Input.inputString.Length == 1)
                {
                    TryAddKeyPressed(Input.inputString);

                    if (IsHoldTwoKeys())
                    {
                        holdInputView.Show(playerEnterInput);
                    }
                }
            }
        }

        if (IsInputKeyUp(playerEnterInput.leftInputKeyName))
        {
            playerEnterInput.leftInputKeyName = string.Empty;
        }

        if (IsInputKeyUp(playerEnterInput.rightInputKeyName))
        {
            playerEnterInput.rightInputKeyName = string.Empty;
        }

        if(IsHoldTwoKeys())
        {
            UpdateHoldTime();
        } else
        {
            holdInputView.Hide();
            holdDeltaTime = 0f;
        }
    }

    private void OnDisable()
    {
        RemoveListener();
    }

    private bool IsHoldTwoKeys()
    {
        return playerEnterInput.leftInputKeyName != string.Empty &&
            playerEnterInput.rightInputKeyName != string.Empty;
    }

    private bool IsInputKeyUp(string keyName)
    {
        return keyName == string.Empty || Input.GetKeyUp(keyName);
    }

    private void TryStartGame()
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            if(ingameManager.CanStartGame())
            {
                ingameManager.StartGame();
            }
        }
    }

    private void UpdateHoldTime()
    {
        holdDeltaTime += Time.deltaTime;
        if (holdDeltaTime >= 1f)
        {
            if(ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
            {
                ingameManager.CreatePlayer(playerEnterInput);
            }

            holdInputView.Hide();
            holdDeltaTime = 0f;
            playerEnterInput.Clear();
        }
    }

    private void TryAddKeyPressed(string inputString)
    {
        if (char.TryParse(inputString, out char c))
        {
            if (CanUseKeyPressed(c))
            {
                if (playerEnterInput.leftInputKeyName == string.Empty)
                {
                    playerEnterInput.leftInputKeyName = c.ToString();
                }
                else if (playerEnterInput.rightInputKeyName == string.Empty)
                {
                    playerEnterInput.rightInputKeyName = c.ToString();
                }
            }
        }
    }

    private bool CanUseKeyPressed(char c)
    {
        if (char.IsLetterOrDigit(c))
        {
            if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
            {
                return ingameManager.CanPressKey(c.ToString());
            }
        }
        return false;
    }

    private void AddListener()
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnPlayerAdded += OnPlayerAdded;
        }
    }

    private void RemoveListener()
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnPlayerAdded -= OnPlayerAdded;
        }
    }

    private void OnPlayerAdded(Player player)
    {
        GameObject newObj = Instantiate(playerListCellView, playerListView);
        UIPlayerListViewCell playerListViewCell = newObj.GetComponent<UIPlayerListViewCell>();
        playerListViewCell.Show((ControllablePlayer) player);
    }
}
