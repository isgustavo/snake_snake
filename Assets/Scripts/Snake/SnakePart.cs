using System.Collections;
using UnityEngine;

public class SnakePart : MonoBehaviour
{
    [SerializeField]
    private int value;

    Renderer partRenderer;
    public int SnakeHeadId { get; private set; }
    
    private void Awake()
    {
        partRenderer = GetComponent<Renderer>();
    }

    public void SetHead(int headId, Color color)
    {
        SnakeHeadId = headId;
        partRenderer.material.color = color;
    }

    public void HitBy(Snake snake)
    {
        LifeStat lifeStat = snake.Stats.GetStats<LifeStat>();
        lifeStat.AddValue(value);
    }

    public void Remove()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            poolManager.Despawn(this.gameObject);
        }
    }
}