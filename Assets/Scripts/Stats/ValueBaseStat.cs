using System;

public interface IStat 
{
    public void AddChangeStatListener(Action<IStat> action);
    public void RemoveChangeStatListener(Action<IStat> action);
}

public abstract class ValueBaseStat<T> : IStat
{
    public T BaseValue { get; private set; }
    private T currentValue;
    public T Value
    {
        get
        {
            return currentValue;
        }
        set
        {
            currentValue = value;
            ValueChanged?.Invoke(this);
        }
    }

    protected event Action<IStat> ValueChanged;

    public ValueBaseStat(T baseValue)
    {
        BaseValue = baseValue;
        Value = BaseValue;
    }

    public virtual void OverrideValue(T value)
    {
        currentValue = value;
    }

    public abstract void AddValue(T value);

    public void AddChangeStatListener(Action<IStat> action)
    {
        ValueChanged += action;
    }

    public void RemoveChangeStatListener(Action<IStat> action)
    {
        ValueChanged -= action;
    }
}


