using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int PlayerId { get; protected set; }
    public Snake Snake { get; private set; }
    public Color Color { get; private set; }

    public virtual void SetPlayer(int playerId, Color color)
    {
        PlayerId = playerId;
        Color = color;
    }

    public virtual void SetSnake(Snake snake, InitialSnakeValues initialSnakeValues, int initialSnakeSize)
    {
        Snake = snake;
        Snake.Setup(this, initialSnakeValues, initialSnakeSize);
    }

    public virtual void Remove()
    {
        Snake.Remove();
    }
}


