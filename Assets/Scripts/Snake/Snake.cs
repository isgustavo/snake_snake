using System;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private UISnakeController snakeView;

    //private Player player;
    private Color playerColor;
    public int PlayerId { get; private set; }
    public IInputHandler InputHandler { get; private set; }
    public SnakeStateMachineController StateMachine { get; private set; }
    public StatsController Stats { get; private set; } = new StatsController();
    public List<SnakePart> BodyParts { get; private set; } = new List<SnakePart>();

    Renderer headRenderer;

    public event Action<int, IStat> OnSnakeStatsChanged;

    private void Awake()
    {
        headRenderer = GetComponent<Renderer>();
        StateMachine = GetComponent<SnakeStateMachineController>();
    }

    private void Update()
    {
        if(StateMachine.CurrentState != null)
        {
            StateMachine.Updater();
        }
    }

    public void Setup(Player player, InitialSnakeValues initialValues, int size)
    {
        PlayerId = player.PlayerId;
        playerColor = player.Color;
        InputHandler = player.GetComponent<IInputHandler>();

        Stats.AddStat(new LifeStat(initialValues.InitialLife));
        Stats.AddStat(new SpeedStat(initialValues.InitialSpeed, initialValues.MinSpeed, initialValues.MaxSpeed));
        Stats.AddStat(new ClockTimeStat(false));
        Stats.OnStatChanged += OnStatsChanged;

        snakeView.Setup(player);
   
        headRenderer.material.color = player.Color;
        for (int i = 0; i < size; i++)
        {
            Grow();
        }

        StateMachine.AddState<DeathState>(new DeathState(this));
        StateMachine.AddState<RewindState>(new RewindState(this));
        StateMachine.AddState<MoveState>(new MoveState(this), true);
    }

    public void Remove()
    {
        snakeView.Remove();

        Stats.OnStatChanged -= OnStatsChanged;
        Stats.RemoveAll();

        for (int i = BodyParts.Count - 1; i >= 0; i--)
        {
            BodyParts[i].Remove();
        }

        BodyParts.Clear();

        StateMachine.Clear();
        PlayerId = -1;
    }

    private void OnStatsChanged(IStat stat)
    {
        if (stat is SpeedStat)
        {
            Grow();
        }

        OnSnakeStatsChanged?.Invoke(PlayerId, stat);
    }

    protected void Grow()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManagerBehaviour))
        {
            GameObject body = poolManagerBehaviour.Spawn("SnakeBody");
            body.transform.position = Head().position;
            SnakePart snakePart = body.GetComponent<SnakePart>();
            snakePart.SetHead(PlayerId, playerColor);
            snakePart.gameObject.SetActive(true);
            BodyParts.Add(snakePart);
        }
    }

    //public Player Player() => player;
    //public Vector2 ReadInput() => inputHandler.ReadInput();
    //public StatsController Stats() => stats;
    public Transform Head() => this.transform;
    //public List<SnakePart> Body() => bodyParts;
    //public SnakeStateMachineController StateMachine() => stateMachine;
    public UISnakeController SnakeView() => snakeView;
}
/*public interface ISnakeContext : IContext
{
    //public Player Player();
    public StateMachineController StateMachine();
    public StatsController Stats();
    public Vector2 ReadInput();
    public Transform Head();
    public List<SnakePart> Body();
    public UISnakeController SnakeView();
}*/