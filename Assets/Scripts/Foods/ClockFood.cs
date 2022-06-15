public class ClockFood : WorldObject<bool>
{
    public override void HitBy(Snake snake)
    {
        ClockTimeStat clockTimeStat = snake.Stats.GetStats<ClockTimeStat>();
        clockTimeStat.AddValue(hitValue);

        base.HitBy(snake);
    }
}