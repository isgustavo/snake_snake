using System.Collections.Generic;
using UnityEngine;

public class RewindState : SnakeState
{
    float updateDeltaTime;
    float rewindDeltaTime;

    public RewindState(Snake context) : base(context) { }

    public override void EnterState(Dictionary<string, object> values = null)
    {
        base.EnterState(values);

        updateDeltaTime = 0f;
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            rewindDeltaTime = ingameManager.IngameStats.RewindSnakeInSecond;
        }
    }

    public override void UpdateState()
    {
        updateDeltaTime += Time.deltaTime;
        if (updateDeltaTime <= rewindDeltaTime)
        {
            return;
        }

        updateDeltaTime = 0f;

        base.UpdateState();

        if (ManagerLocator.TryGetManager(out MementoManagerBehaviour mementoBehaviour))
        {
            MementoState memento = mementoBehaviour.TryGetState(Context.PlayerId);
            if (memento == null)
            {
                Context.StateMachine.ChangetState<MoveState>();
            } else
            {
                ApplyState(memento);
            }
        }
    }

    public void ApplyState(MementoState memento)
    {
        Context.Stats.GetStats<LifeStat>().OverrideValue(memento.Life);
        Context.Stats.GetStats<SpeedStat>().OverrideValue(memento.Speed);

        Context.InputHandler.SetInput(memento.Direction);

        if (Context.BodyParts.Count > memento.BodyPosition.Count)
        {
            RemoveBodyPart();
        }

        Context.Head().transform.position = new Vector3(memento.HeadPosition.x, 1, memento.HeadPosition.y);
        for (int i = 0; i < Context.BodyParts.Count; i++)
        {
            Context.BodyParts[i].transform.position = new Vector3(memento.BodyPosition[i].x, 1, memento.BodyPosition[i].y);
        }
    }

    void RemoveBodyPart()
    {
        if(ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            poolManager.Despawn(Context.BodyParts[0].gameObject);
            Context.BodyParts.RemoveAt(0);
        }
    }
}
