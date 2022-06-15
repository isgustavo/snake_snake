using UnityEngine;

public class WallPart : WorldObject<int>
{
    public override void HitBy(Snake snake)
    {
        LifeStat lifeStat = snake.Stats.GetStats<LifeStat>();
        lifeStat.AddValue(hitValue);
    }  
}