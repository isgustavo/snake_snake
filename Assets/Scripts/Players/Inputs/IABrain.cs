using System;
using UnityEngine;

public class IABrain : MonoBehaviour, IInputHandler
{
    Vector2 lastInput = Vector2.down;

    Snake snake;
    Vector3 lastFoodPosition;

    public void SetupBrain(Snake snake)
    {
        this.snake = snake;

        if (ManagerLocator.TryGetManager(out FoodSpawnerManager foodSpawner))
        {
            foodSpawner.OnFoodChanged += OnFoodChanged;
        }
    }

    public void Remove()
    {
        if (ManagerLocator.TryGetManager(out FoodSpawnerManager foodSpawner))
        {
            foodSpawner.OnFoodChanged -= OnFoodChanged;
        }

        snake = null;
    }

    private void OnFoodChanged(IWorldObject worldObject)
    {
        lastFoodPosition = worldObject.GetPosition();
    }

    public Vector2 ReadInput()
    {
        SetFreeDirection(snake.Head(), lastFoodPosition);

        if (IsMoveThroughSnake())
        {
            TrySetSafeDirection();
        }

        return lastInput;
    }

    public void SetInput(Vector2 direction)
    {
        lastInput = direction;
    }

    bool IsMoveThroughSnake()
    {
        return Physics.Raycast(snake.Head().position, new Vector3(lastInput.x, 0, lastInput.y), 1f, CollisionLayers.SNAKE);
    }

    void TrySetSafeDirection()
    {
        Vector2 input;
        if (lastInput.x != 0f)
        {
            input = Physics.Raycast(snake.Head().position, Vector3.forward, 1f, CollisionLayers.SNAKE) ? Vector2.down : Vector2.up;
        } else
        {
            input = Physics.Raycast(snake.Head().position, Vector3.right, 1f, CollisionLayers.SNAKE) ? Vector2.left : Vector2.right;
        }

        lastInput = input;
    }

    private void SetFreeDirection(Transform head, Vector3 lastFoodSpawned)
    {
        Vector2 input = lastInput;
        if (lastInput.x != 0f)
        {
            input = GetHorizontalInput(head.transform.position, lastFoodSpawned);
        }
        else if (lastInput.y != 0f)
        {
            input = GetVerticalInput(head.transform.position, lastFoodSpawned);
        }

        lastInput = input;      
     }

    private bool IsFoodOnRight(float food, float head)
    {
        return food > head;
    }

    private bool IsFoodOnLeft(float food, float head)
    {
        return food < head;
    }

    private bool IsFoodAbove(float food, float head)
    {
        return food > head;
    }

    private bool IsFoodBelow(float food, float head)
    {
        return food < head;
    }
    private Vector3 GetHorizontalInput(Vector3 head, Vector3 lastFoodSpawned)
    {
        if (IsFoodOnRight(lastFoodSpawned.x, head.x))
        {
            return GetDirectionOnRight(lastFoodSpawned.z, head.z);
        }
        else if (IsFoodOnLeft(lastFoodSpawned.x, head.x))
        {
            return GetDirectionOnLeft(lastFoodSpawned.z, head.z);
        }
        else
        {
            return lastFoodSpawned.z > head.z ? Vector2.up : Vector2.down;
        }
    }

    private Vector3 GetVerticalInput(Vector3 head, Vector3 lastFoodSpawned)
    {
        if (IsFoodAbove(lastFoodSpawned.z, head.z))
        {
            return GetDirectionAbove(lastFoodSpawned.x, head.x);
        }
        else if (IsFoodBelow(lastFoodSpawned.z, head.z))
        {
            return GetDirectionBelow(lastFoodSpawned.x, head.x);
        }
        else
        {
            return lastFoodSpawned.x > head.x ? Vector2.right : Vector2.left;
        }
    }

    private Vector2 GetDirectionOnRight(float food, float head)
    {
        if (IsFoodAbove(food, head))
        {
            return lastInput.x > 0 ? Vector2.right : Vector2.up;
        }
        else if (IsFoodBelow(food, head))
        {
            return lastInput.x > 0 ? Vector2.right : Vector2.down;
        }
        else
        {
            return lastInput.x > 0 ? Vector2.right : Vector2.up;
        }
    }

    Vector2 GetDirectionOnLeft(float food, float head)
    {
        if (IsFoodAbove(food, head))
        {
            return lastInput.x > 0 ? Vector2.up : Vector2.left;
        }
        else if (IsFoodBelow(food, head))
        {
            return lastInput.x > 0 ? Vector2.down : Vector2.left;
        }
        else
        {
            return lastInput.x > 0 ? Vector2.up : Vector2.left;
        }
    }

    Vector2 GetDirectionAbove(float food, float head)
    {
        if (IsFoodOnRight(food, head))
        {
            return lastInput.y > 0 ? Vector2.up : Vector2.right;
        }
        else if (IsFoodOnLeft(food, head))
        {
            return lastInput.y > 0 ? Vector2.up : Vector2.left;
        }
        else
        {
            return lastInput.y > 0 ? Vector2.up : Vector2.right;
        }
    }

    Vector2 GetDirectionBelow(float food, float head)
    {
        if (IsFoodOnRight(food, head))
        {
            return lastInput.y > 0 ? Vector2.right : Vector2.down;
        }
        else if (IsFoodOnLeft(food, head))
        {
            return lastInput.y > 0 ? Vector2.left : Vector2.down;
        }
        else
        {
            return lastInput.y > 0 ? Vector2.right : Vector2.down;
        }
    }

}


