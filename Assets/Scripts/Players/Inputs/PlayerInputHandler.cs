using System;
using UnityEngine;

public interface IInputHandler
{
    public Vector2 ReadInput();
    public void SetInput(Vector2 direction);
}

public class PlayerInputHandler : MonoBehaviour, IInputHandler
{
    public string LeftInput { get; private set; } = string.Empty;
    public string RightInput { get; private set; } = string.Empty;

    bool wasLastInputRead = true;
    Vector2 lastInput = Vector2.down;

    private void Update()
    {
        if(LeftInput == RightInput || wasLastInputRead == false)
        {
            return;
        }

        Vector2 input = lastInput;
        if (lastInput.x != 0f)
        {
            if (Input.GetKeyDown(RightInput))
            {
                input = lastInput.x > 0 ? Vector2.down : Vector2.up;
                wasLastInputRead = false;
            }
            else if (Input.GetKeyDown(LeftInput))
            {
                input = lastInput.x > 0 ? Vector2.up : Vector2.down; 
                wasLastInputRead = false;
            }
        }
        else if (lastInput.y != 0f)
        {
            if (Input.GetKeyDown(RightInput))
            {
                input = Vector2.right;
                wasLastInputRead = false;
            }
            else if (Input.GetKeyDown(LeftInput))
            {
                input = Vector2.left;
                wasLastInputRead = false;
            }
        }

        lastInput = input;

    }

    public void SetInputs(string left, string right)
    {
        LeftInput = left;
        RightInput = right;
    }

    public void Remove()
    {
        LeftInput = string.Empty;
        RightInput = string.Empty;
    }

    public Vector2 ReadInput() 
    {
        wasLastInputRead = true;
        return lastInput;
    }
    public void SetInput(Vector2 direction)
    {
        lastInput = direction;
    }
}


