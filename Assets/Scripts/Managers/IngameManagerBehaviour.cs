using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameManagerBehaviour : MonoBehaviour
{
    [SerializeField]
    private InitialSnakeValues initialSnakeValues;
    [SerializeField]
    private IngameStats ingameStats;

    public Dictionary<int, Player> Players { get; private set; } = new();

    public event Action<Player> OnPlayerAdded;
    public event Action<Player> OnPlayerRemoved;
    public event Action<Snake> OnSnakeAdded;
    public event Action<Snake> OnSnakeRemoved;
    public event Action OnGameStarted;
    public event Action<int> OnGameEnded;

    private void OnEnable()
    {
        ManagerLocator.RegisterManager(this);
    }

    private void Start()
    {
        if(ManagerLocator.TryGetManager(out InitializeManagerBehaviour initializeManager))
        {
            if(initializeManager.IsGameReady == false)
            {
                initializeManager.OnGameIsReady += OnGameIsReady;
            } else
            {
                OpenMainMenu();
            }
        }
    }

    private void OnGameIsReady()
    {
        if (ManagerLocator.TryGetManager(out InitializeManagerBehaviour initializeManager))
        {
            initializeManager.OnGameIsReady -= OnGameIsReady;
            OpenMainMenu();
        }
    }

    private void OpenMainMenu()
    {
        SceneManager.LoadSceneAsync(SceneEnum.MainMenuScene.ToString(), LoadSceneMode.Additive);
    }

    private void AddPlayer(Player newPlayer)
    {
        if (Players.ContainsKey(newPlayer.PlayerId) == false)
        {
            Players.Add(newPlayer.PlayerId, newPlayer);
        }
    }

    private void CreateNPCPlayer()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            GameObject newNPCObj = poolManager.Spawn("NPCPlayer");
            NPCPlayer npcPlayer = newNPCObj.GetComponent<NPCPlayer>();
            npcPlayer.SetPlayer(Players.Count, UnityEngine.Random.ColorHSV());
            AddPlayer(npcPlayer);
            npcPlayer.gameObject.SetActive(true);
        }
    }

    private void CreatePlayersSnake()
    {
        foreach (int playerId in Players.Keys)
        {
            Players[playerId].SetSnake(SpawnSnake(), initialSnakeValues, IngameStats.InitialSnakeSize);
            OnSnakeAdded?.Invoke(Players[playerId].Snake);
            Players[playerId].Snake.transform.position = GetSpawnPoint();

            Players[playerId].Snake.OnSnakeStatsChanged += OnSnakeStatsChanged;
            Players[playerId].Snake.gameObject.SetActive(true);
        }
    }

    private Snake SpawnSnake()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            GameObject newObj = poolManager.Spawn("SnakeHead");
            return newObj.GetComponent<Snake>();
        }

        return null;
    }

    private int GetRandomRangePoint(int range)
    {
        return UnityEngine.Random.Range(-range, range);
    }

    public Vector3 GetSpawnPoint()
    {
        Vector3 point = new Vector3(GetRandomRangePoint(IngameStats.SquareArenaSize), 1, GetRandomRangePoint(IngameStats.SquareArenaSize));
        if (Physics.Raycast(point + Vector3.up * 10f, Vector3.down, out RaycastHit hitInfo, 15f, CollisionLayers.SNAKE | CollisionLayers.FOOD))
        {
            return GetSpawnPoint();
        }
        return point;
    }

    public IngameFood GetNewFood()
    {
        IngameFood food = IngameStats.GetFood();
        if (food.FoodPrefab.GetComponent<IWorldObject>() is ClockFood)
        {
            if (ManagerLocator.TryGetManager(out MementoManagerBehaviour mementoManager))
            {
                if (mementoManager.IsActive)
                {
                    return GetNewFood();
                }
            }
        }

        return food;
    }

    public void CreatePlayer(PlayerEnterInput playerEnterInput)
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            GameObject newPlayerObj = poolManager.Spawn("Player");
            ControllablePlayer newPlayer = newPlayerObj.GetComponent<ControllablePlayer>();
            newPlayer.SetPlayer(Players.Count, playerEnterInput, UnityEngine.Random.ColorHSV());
            newPlayer.gameObject.SetActive(true);
            OnPlayerAdded?.Invoke(newPlayer);
            AddPlayer(newPlayer);
        }
    }

    public void StartGame()
    {
        CreateNPCPlayer();
        CreatePlayersSnake();

        SceneManager.UnloadSceneAsync(SceneEnum.MainMenuScene.ToString());
        SceneManager.LoadSceneAsync(SceneEnum.EnvironmentScene.ToString(), LoadSceneMode.Additive);
        OnGameStarted?.Invoke();
    }

    public void RemovePlayer(int playerId)
    {
        if (Players.ContainsKey(playerId))
        {
            if(ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
            {
                Players[playerId].Snake.OnSnakeStatsChanged -= OnSnakeStatsChanged;
                OnPlayerRemoved?.Invoke(Players[playerId]);
                OnSnakeRemoved?.Invoke(Players[playerId].Snake);
                Players[playerId].Remove();
                poolManager.Despawn(Players[playerId].Snake.gameObject);
                poolManager.Despawn(Players[playerId].gameObject);
            }

            Players.Remove(playerId);
        }
    }

    private void TryEndGame()
    {
        if(Players.Keys.Count <= 1)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        int winnerPlayerId = GetLastPlayer();
        RemovePlayer(winnerPlayerId);

        OnGameEnded?.Invoke(winnerPlayerId);
    }

    private int GetLastPlayer()
    {
        foreach (int playerId in Players.Keys)
        {
            return playerId;
        }

        return 0;
    }

    private void OnSnakeStatsChanged(int playerId, IStat stats)
    {
        if(stats is LifeStat)
        {
            if(IsPlayerSnakeDead(playerId))
            {
                if(IsPlayerSnakeWithClock(playerId))
                {
                    RemovePlayerSnakeClock(playerId);
                    RewindPlayers();
                } else
                {
                    Players[playerId].Snake.StateMachine.ChangetState<DeathState>();
                    StartCoroutine(RemoveDelayCoroutine(playerId));
                }
            }
        }
    }

    void RewindPlayers()
    {
        foreach (int playerId in Players.Keys)
        {
            Players[playerId].Snake.StateMachine.ChangetState<RewindState>();
        }
    }

    IEnumerator RemoveDelayCoroutine(int playerId)
    {
        yield return new WaitForSeconds(.5f);
        RemovePlayer(playerId);

        TryEndGame();
    }

    private void RemovePlayerSnakeClock(int playerId)
    {
        Players[playerId].Snake.Stats.GetStats<ClockTimeStat>().OverrideValue(false);
    }

    private bool IsPlayerSnakeWithClock(int playerId)
    {
        return Players[playerId].Snake.Stats.GetStats<ClockTimeStat>().Value;
    }

    private bool IsPlayerSnakeDead(int playerId)
    {
        return Players[playerId].Snake.Stats.GetStats<LifeStat>().Value <= 0;
    }

    public bool CanPressKey(string key)
    {
        foreach (int playerId in Players.Keys)
        {
            PlayerInputHandler inputHandler = (Players[playerId] as ControllablePlayer).InputHandler;
            if(inputHandler.LeftInput == key || inputHandler.RightInput == key)
            {
                return false;
            }    
        }

        return true;
    }

    public bool CanStartGame()
    {
        return Players.Keys.Count > 0;
    }

    public IngameStats IngameStats => ingameStats;
}
