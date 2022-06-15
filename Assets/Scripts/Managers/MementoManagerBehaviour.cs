using System;
using System.Collections.Generic;
using UnityEngine;

public class MementoManagerBehaviour : MonoBehaviour
{
    private Dictionary<int, Stack<MementoState>> states = new();

    public bool IsActive { get; private set; } = false;

    private void OnEnable()
    {
        ManagerLocator.RegisterManager(this);
    }

    private void Start()
    {
        if(ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnSnakeAdded += OnSnakeAdded;
            ingameManager.OnSnakeRemoved += OnSnakeRemoved;
        }
    }

    private void OnDisable()
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnSnakeAdded -= OnSnakeAdded;
            ingameManager.OnSnakeRemoved -= OnSnakeRemoved;
        }
    }

    private void OnSnakeAdded(Snake snake)
    {
        snake.OnSnakeStatsChanged += OnSnakeStatsChanged;
    }

    private void OnSnakeRemoved(Snake snake)
    {
        snake.OnSnakeStatsChanged -= OnSnakeStatsChanged;
    }

    private void OnSnakeStatsChanged(int playerId, IStat stats)
    {
        if (stats is ClockTimeStat)
        {
            if (ShouldRecord(playerId))
            {
                Active();
            }
            else
            {
                Inactive();
            }
        }
    }

    private bool ShouldRecord(int playerId)
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            return ingameManager.Players[playerId].Snake.Stats.GetStats<ClockTimeStat>().Value;
        }

        return false;
    }

    private void Active ()
    {
        states.Clear();
        IsActive = true;
    }

    public void Inactive()
    {
        IsActive = false;
    }

    public void SetState(int playerId, MementoState state)
    {
        if(IsActive)
        {
            if (states.ContainsKey(playerId) == false)
            {
                states.Add(playerId, new());
            }
            states[playerId].Push(state);
        }
    }

    public MementoState TryGetState(int playerId)
    {
        if (states.ContainsKey(playerId))
        {
            if(states[playerId].Count > 0)
            {
                return states[playerId].Pop();
            }
        }
        return null;
    }
}

public class MementoState
{
    public int Life { get; private set; }
    public float Speed { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 HeadPosition { get; private set; }
    public List<Vector2> BodyPosition { get; private set; }

    public MementoState(int life, float speed, Vector2 direction, Vector2 headPosition, List<Vector2> bodyPosition)
    {
        Life = life;
        Speed = speed;
        Direction = direction;
        HeadPosition = headPosition;
        BodyPosition = bodyPosition;
    }
}