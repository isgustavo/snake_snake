using UnityEngine;

public class NPCPlayer : Player
{
    IABrain brain;

    private void OnEnable()
    {
        brain = GetComponent<IABrain>();
    }

    public override void SetSnake(Snake snake, InitialSnakeValues initialSnakeValues, int initialSnakeSize)
    {
        base.SetSnake(snake, initialSnakeValues, initialSnakeSize);
        brain.SetupBrain(snake);
        SetInitialDirection();
    }

    public override void Remove()
    {
        brain.Remove();
        base.Remove();
    }

    private void SetInitialDirection()
    {
        if (Snake.Head().position.x > Snake.Head().position.z)
        {
            brain.SetInput(new Vector2(Snake.Head().position.x > 0 ? -1 : 1, 0));
        }
        else
        {
            brain.SetInput(new Vector2(0, Snake.Head().position.z > 0 ? -1 : 1));
        }
    }
}


