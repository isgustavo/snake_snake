using UnityEngine;

public abstract class SnakeState : State<Snake>
{
    public SnakeState(Snake context) : base(context) {  }
}
