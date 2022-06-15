using UnityEngine;

public abstract class MinMaxStat : FloatStat
{
    private readonly float minValue;
    private readonly float maxValue;

    public MinMaxStat(float baseValue, float minValue, float maxValue) : base(baseValue) 
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public override void AddValue(float value)
    {
        Value = Mathf.Clamp(Value + value, minValue, maxValue);
    }
}




