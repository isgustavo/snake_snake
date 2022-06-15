using System.Collections.Generic;

public abstract class State<T>
{
    virtual public T Context { get; private set; }

    public State(T context) 
    {
        Context = context;
    }

    virtual public void EnterState(Dictionary<string, object> values = null) { }
    virtual public void UpdateState() { }
    virtual public void FixedState() { }
    virtual public void LateUpdateState() { }
    virtual public void AnimationEvent(string value) { }
    virtual public void ExitState() { }
}
