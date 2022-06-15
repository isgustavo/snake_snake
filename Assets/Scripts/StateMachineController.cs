using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineController<T> : MonoBehaviour
{
    [ReadOnly]
    [SerializeField]
    private string previousStateName;
    [ReadOnly]
    [SerializeField]
    private string currentStateName;

    public State<T> PreviousState { get; private set; }
    public State<T> CurrentState { get; private set; }

    public Dictionary<Type, State<T>> States { get; protected set; } = new();

    public event Action<State<T>> OnStateChanged;

    virtual public void Updater()
    {
        CurrentState?.UpdateState();
    }

    virtual public void FixedUpdater()
    {
        CurrentState?.FixedState();
    }

    virtual public void LateUpdater()
    {
        CurrentState?.LateUpdateState();
    }

    virtual public void AddState<E>(State<T> state, bool isInitial = false) where E : State<T>
    {
        States.Add(typeof(E), state);

        if (isInitial)
        {
            ChangetState<E>();
        }
    }

    virtual public void ChangetState<E>(Dictionary<string, object> values = null) where E : State<T>
    {
        if (States.ContainsKey(typeof(E)) == false)
        {
            Debug.LogError($"State not Found {typeof(E).ToString()}");
            return;
        }
           
        CurrentState?.ExitState();
        PreviousState = CurrentState;
        previousStateName = PreviousState?.ToString();
        CurrentState = States[typeof(E)];
        CurrentState?.EnterState(values);
        currentStateName = CurrentState?.ToString();
        OnStateChanged?.Invoke(CurrentState);
    }

    public virtual void Clear()
    {
        PreviousState = null;
        CurrentState = null;
        States?.Clear();
    }

    virtual public void AnimationEvent(string value)
    {
        CurrentState?.AnimationEvent(value);
    }
}
