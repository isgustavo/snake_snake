public abstract class IntStat : ValueBaseStat<int>
{
    public IntStat(int baseValue) : base(baseValue) { }

    public override void AddValue(int value)
    {
        Value += value;
    }
}




