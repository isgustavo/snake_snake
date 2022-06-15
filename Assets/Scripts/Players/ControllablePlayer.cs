using UnityEngine;

public class ControllablePlayer : Player
{
    public PlayerInputHandler InputHandler { get; private set; }

    private void Awake()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
    }

    public void SetPlayer(int playerId, PlayerEnterInput enterInput, Color color)
    {
        base.SetPlayer(playerId, color);

        InputHandler.SetInputs(enterInput.leftInputKeyName, enterInput.rightInputKeyName);
    }

    public override void SetSnake(Snake snake, InitialSnakeValues initialSnakeValues, int initialSnakeSize)
    {
        base.SetSnake(snake, initialSnakeValues, initialSnakeSize);
        SetInitialDirection();
    }

    public override void Remove()
    {
        InputHandler?.Remove();
        base.Remove();
    }

    private void SetInitialDirection()
    {
        if (Snake.Head().position.x > Snake.Head().position.z)
        {
            InputHandler.SetInput(new Vector2(Snake.Head().position.x > 0 ? -1 : 1, 0));
        }
        else
        {
            InputHandler.SetInput(new Vector2(0, Snake.Head().position.z > 0 ? -1 : 1));
        }
    }
}


