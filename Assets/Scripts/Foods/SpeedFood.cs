public class SpeedFood : WorldObject<float>
{
    public override void HitBy(Snake snake)
    {
        SpeedStat speedStat = snake.Stats.GetStats<SpeedStat>();
        speedStat.AddValue(hitValue);

        base.HitBy(snake);
    }
}
