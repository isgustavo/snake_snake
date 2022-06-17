using System.Collections.Generic;
using UnityEngine;

public class MoveState : SnakeState
{
    Vector2 direction;
    float updateDeltaTime;

    public MoveState(Snake context) : base(context) {  }

    public override void EnterState(Dictionary<string, object> values = null)
    {
        base.EnterState(values);
        Context.SnakeView()?.gameObject.SetActive(true);
        Context.SnakeView()?.UpdateLifeStat(Context.Stats.GetStats<LifeStat>().Value);
        updateDeltaTime = 0f;
    }


    override public void UpdateState()
    {
        updateDeltaTime += Time.deltaTime;
        if (updateDeltaTime <= Context.Stats.GetStats<SpeedStat>().Value)
        {
            return;
        }

        updateDeltaTime = 0f;

        base.UpdateState();

        direction = Context.InputHandler.ReadInput();

        TryEatFood();
        TryEatOtherSnake();
        IsMoveThroughWall();
        
        Move();

        CreateMemento();
    }

    public override void ExitState()
    {
        base.ExitState();
        Context.SnakeView()?.gameObject.SetActive(false);
    }

    void CreateMemento()
    {
        if (ManagerLocator.TryGetManager(out MementoManagerBehaviour mementoManagerBehaviour))
        {
            List<Vector2> bodyParts = new List<Vector2>();
            foreach (SnakePart snakePart in Context.BodyParts)
            {
                bodyParts.Add(new Vector2(snakePart.transform.position.x, snakePart.transform.position.z));
            }

            mementoManagerBehaviour.SetState(Context.PlayerId,
                new MementoState(Context.Stats.GetStats<LifeStat>().Value,
                                                    Context.Stats.GetStats<SpeedStat>().Value,
                                                    direction,
                                                        new Vector2(Context.Head().position.x, Context.Head().position.z),
                                                            bodyParts));
        }
    }

    void TryEatOtherSnake()
    {
        if (Physics.Raycast(Context.Head().position, new Vector3(direction.x, 0, direction.y), out RaycastHit hitInfo, 1f, CollisionLayers.SNAKE))
        {
            SnakePart snakePart = hitInfo.collider.GetComponent<SnakePart>();
            if(snakePart != null)
            {
                snakePart.HitBy(Context);
            }
        }
    }

    void TryEatFood()
    {
        if(Physics.Raycast(Context.Head().position, new Vector3(direction.x, 0, direction.y), out RaycastHit hitInfo, 1f, CollisionLayers.FOOD))
        {
            IWorldObject wordObject = hitInfo.collider.GetComponent<IWorldObject>();
            wordObject.HitBy(Context);
        }
    }

    void IsMoveThroughWall()
    {
        if (Physics.Raycast(Context.Head().position, new Vector3(direction.x, 1, direction.y), out RaycastHit hitInfo, 1f, CollisionLayers.WALL))
        {
            WallPart wallPart = hitInfo.collider.GetComponent<WallPart>();
            wallPart.HitBy(Context);
        }
    }

    void Move()
    {
        Vector3 lastPosition = Context.Head().position;
        MoveBody();
        Context.Head().position += new Vector3(direction.x, 0, direction.y) * 1f;
        Context.SnakeView()?.UpdateForward((lastPosition - Context.Head().position).normalized);
    }

    void MoveBody()
    {
        for (int i = Context.BodyParts.Count - 1; i > 0; i--)
        {
            Context.BodyParts[i].transform.position = Context.BodyParts[i - 1].transform.position;
        }

        if (Context.BodyParts.Count > 0)
        {
            Context.BodyParts[0].transform.position = Context.Head().position;
        }
    }
}