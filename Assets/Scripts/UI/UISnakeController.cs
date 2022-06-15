using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISnakeController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text snakePlayerText;
    [SerializeField]
    private TMP_Text snakeLivesText;
    [SerializeField]
    private TMP_Text leftInputText;
    [SerializeField]
    private TMP_Text rightInputText;
    [SerializeField]
    private Image clockImage;
    [SerializeField]
    private Transform content;

    Player player;
    RectTransform canvas;

    public void Setup(Player player)
    {
        this.player = player;
        canvas = GetComponent<RectTransform>();

        snakePlayerText.text = (player.PlayerId + 1).ToString("00");
        UpdateLifeStat(player.Snake.Stats.GetStats<LifeStat>().Value);
        UpdateClock(player.Snake.Stats.GetStats<ClockTimeStat>().Value);
        player.Snake.Stats.OnStatChanged += OnStatChanged;

        TryShowInputHandler();
    }

    void TryShowInputHandler()
    {
        ControllablePlayer controllablePlayer = player as ControllablePlayer;
        if (controllablePlayer != null)
        {
            leftInputText.text = controllablePlayer.InputHandler.LeftInput;
            rightInputText.text = controllablePlayer.InputHandler.RightInput;
        }

        leftInputText.gameObject.SetActive(controllablePlayer != null);
        rightInputText.gameObject.SetActive(controllablePlayer != null);
    }

    public void Remove()
    {
        if (player != null)
        {
            player.Snake.Stats.OnStatChanged -= OnStatChanged;
        }

        player = null;

        gameObject.SetActive(false);
    }

    public void UpdateLifeStat(int? value)
    {
        snakeLivesText.text = value?.ToString("00");
    }

    public void UpdateClock(bool? value)
    {
        clockImage.gameObject.SetActive(value ?? false);
    }


    private void OnStatChanged(IStat stat)
    {
        if(stat is LifeStat)
        {
            UpdateLifeStat(player?.Snake?.Stats?.GetStats<LifeStat>()?.Value);
        }
        else if (stat is ClockTimeStat)
        {
            UpdateClock(player?.Snake?.Stats?.GetStats<ClockTimeStat>()?.Value);
        }
    }

    public void UpdateForward(Vector3 direction)
    {
        canvas.rotation = Quaternion.Euler(90f, 0f, 90f * direction.x + 90f * direction.y);
    }

}