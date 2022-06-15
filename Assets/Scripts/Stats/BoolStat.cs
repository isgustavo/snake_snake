public abstract class BoolStat : ValueBaseStat<bool>
{
    public BoolStat(bool baseValue) : base(baseValue) { }

    public override void AddValue(bool value)
    {
        Value = value;
    }
}




