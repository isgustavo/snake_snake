public abstract class FloatStat : ValueBaseStat<float>
{
    public FloatStat(float baseValue) : base(baseValue) { }

    public override void AddValue(float value)
    {
        Value += value;
    }
}




