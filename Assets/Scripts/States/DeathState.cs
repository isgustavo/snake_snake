using System.Collections.Generic;

public class DeathState : SnakeState
{
    public DeathState(Snake context) : base(context) { }

    public override void EnterState(Dictionary<string, object> values = null)
    {
        base.EnterState(values);

        for(int i = 0; i < Context.BodyParts.Count; i++)
        {
            SnakePart part = Context.BodyParts[i];
            part.Remove();
        }

        Context.BodyParts.Clear();
    }
}
