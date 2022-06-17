using System;
using System.Collections.Generic;

public class StatsController
{
    readonly Dictionary<Type, IStat> Stats = new Dictionary<Type, IStat>();

    public event Action<IStat> OnStatChanged;

    public void AddStat<T>(T stat) where T : IStat
    {
        Stats.Add(typeof(T), stat);
        stat.AddChangeStatListener(OnStatListener);
    }

    private void OnStatListener(IStat stat)
    {
        OnStatChanged.Invoke(stat);
    }

    public void RemoveAll()
    {
        foreach(IStat stat in Stats.Values)
        {
            stat.RemoveChangeStatListener(OnStatListener);
        }

        Stats.Clear();
    }

    public void RemoveStat<T>(T stat) where T : IStat
    {
        stat.RemoveChangeStatListener(OnStatListener);
        Stats.Remove(typeof(T));
    }

    public T GetStats<T>()
    {
        if (Stats.ContainsKey(typeof(T)))
        {
            return (T)Stats[typeof(T)];
        }

        return default;
    }
}


