using System;
using UnityEngine;

public interface IWorldObject
{
    public Vector3 GetPosition();
    public void HitBy(Snake snake);
    public void AddHitListener(Action action);
    public void RemoveHitListener(Action action);
}

public abstract class WorldObject<T> : MonoBehaviour, IWorldObject
{
    [SerializeField]
    protected T hitValue;

    private BoxCollider objCollider;
    public event Action OnHit;

    private void Awake()
    {
        objCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        objCollider.enabled = true;
    }

    public virtual void HitBy(Snake snake)
    {
        objCollider.enabled = false;
        OnHit?.Invoke();

        if(ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            poolManager.Despawn(gameObject);
        }
    }

    public void AddHitListener(Action action)
    {
        OnHit += action;
    }

    public void RemoveHitListener(Action action)
    {
        OnHit -= action;
    }

    public Vector3 GetPosition() => this.transform.position;
}




