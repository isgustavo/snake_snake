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

    void SetFreeDirection(Transform head, Vector3 lastFoodSpawned)
    {
        Vector2 input = lastInput;
        if (lastInput.x != 0f)
        {
            if (lastFoodSpawned.x > head.position.x) 
            {
                if (lastFoodSpawned.z > head.position.z) 
                {
                    input = lastInput.x > 0 ? Vector2.right : Vector2.up;
                }
                else if (lastFoodSpawned.z < head.position.z) 
                {
                    input = lastInput.x > 0 ? Vector2.right : Vector2.down;
                }
                else
                {
                    input = lastInput.x > 0 ? Vector2.right : Vector2.up;
                }
            }
            else if (lastFoodSpawned.x < head.position.x) 
            {
                if (lastFoodSpawned.z > head.position.z)
                {
                    input = lastInput.x > 0 ? Vector2.up : Vector2.left;
                }
                else if (lastFoodSpawned.z < head.position.z)
                {
                    input = lastInput.x > 0 ? Vector2.down : Vector2.left;
                }
                else
                {
                    input = lastInput.x > 0 ? Vector2.up : Vector2.left;
                }
            }
            else
            {
                input = lastFoodSpawned.z > head.position.z ? Vector2.up : Vector2.down;
            }
        }
        else if (lastInput.y != 0f)
        {
            if (lastFoodSpawned.z > head.position.z) 
            {
                if (lastFoodSpawned.x > head.position.x) 
                {
                    input = lastInput.y > 0 ? Vector2.up : Vector2.right;
                }
                else if (lastFoodSpawned.x < head.position.x)
                {
                    input = lastInput.y > 0 ? Vector2.up : Vector2.left; 
                }
                else
                {
                    input = lastInput.y > 0 ? Vector2.up : Vector2.right;
                }
            }
            else if (lastFoodSpawned.z < head.position.z)
            {
                if (lastFoodSpawned.x > head.position.x)
                {
                    input = lastInput.y > 0 ? Vector2.right : Vector2.down; 
                }
                else if (lastFoodSpawned.x < head.position.x)
                {
                    input = lastInput.y > 0 ? Vector2.left : Vector2.down;
                }
                else
                {
                    input = lastInput.y > 0 ? Vector2.right : Vector2.down;
                }
            }
            else
            {
                input = lastFoodSpawned.x > head.position.x ? Vector2.right : Vector2.left;
            }
        }

        lastInput = input;      
        }
}


