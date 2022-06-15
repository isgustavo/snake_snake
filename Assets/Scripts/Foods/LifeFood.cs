public class LifeFood : WorldObject<int>
{
    public override void HitBy(Snake snake)
    {
        LifeStat lifeStat = snake.Stats.GetStats<LifeStat>();
        lifeStat.AddValue(hitValue);

        base.HitBy(snake);
    }
}
