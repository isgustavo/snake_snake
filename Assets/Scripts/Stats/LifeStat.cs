using UnityEngine;

public class LifeStat : IntStat
{
    public LifeStat(int baseValue) : base(baseValue) {  }

    public override void AddValue(int value)
    {
        Value = Mathf.Clamp(Value + value, 0, 99);
    }
}




