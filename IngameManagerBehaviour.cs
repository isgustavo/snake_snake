using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "IngameStats", menuName = "Custom/IngameStats")]
public class IngameStats : ScriptableObject
{
    [SerializeField]
    private int squareArenaSize;
    [SerializeField]
    private int initialSnakeSize;
}

public class IngameManagerBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private InitialSnakeValues initialSnakeValues;
    [SerializeField]
    private IngameStats ingameStats;

    class PlayerSnake
    {
        public Player Player;
        public Snake Snake;
    }

    Dictionary<int, PlayerSnake> players = new();
    public event Action<Player> OnPlayerAdded;

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

    void OpenMainMenu()
    {
        SceneManager.LoadSceneAsync(SceneEnum.MainMenuScene.ToString(), LoadSceneMode.Additive);
    }

    public void StartGame()
    {
        CreatePlayersSnake();

        SceneManager.UnloadSceneAsync(SceneEnum.MainMenuScene.ToString());
        SceneManager.LoadSceneAsync(SceneEnum.EnvironmentScene.ToString(), LoadSceneMode.Additive);
    }

    private void OnDestroy()
    {
        foreach (int playerId in players.Keys)
        {
            if (players[playerId].Snake != null)
            {
                players[playerId].Snake.OnSnakeStatsChanged -= OnSnakeStatsChanged;
            }
        }
    }

    public void CreatePlayer(PlayerEnterInput playerEnterInput)
    {
        if(ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            GameObject newObj = Instantiate(playerPrefab);
            ControllablePlayer newPlayer = newObj.GetComponent<ControllablePlayer>();
            newPlayer.SetPlayer(players.Count, playerEnterInput, UnityEngine.Random.ColorHSV());
            AddPlayer(newPlayer);
        }
    }

    private void AddPlayer(Player newPlayer)
    {
        if (players.ContainsKey(newPlayer.PlayerId) == false)
        {
            PlayerSnake playerSnake = new();
            playerSnake.Player = newPlayer;
            players.Add(newPlayer.PlayerId, playerSnake);
            OnPlayerAdded?.Invoke(newPlayer);
        }
    }

    private void CreatePlayersSnake()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            foreach (int playerId in players.Keys)
            {
                GameObject newObj = poolManager.Spawn("Snake Head");
                Snake snake = newObj.GetComponent<Snake>();
                snake.Setup(players[playerId].Player, initialSnakeValues, 3);
                snake.transform.position = new Vector3(UnityEngine.Random.Range(-squareArenaSize + 1, squareArenaSize - 1), 1, UnityEngine.Random.Range(-squareArenaSize + 1, squareArenaSize - 1));
                snake.gameObject.SetActive(true);
                players[playerId].Snake = snake;
            }
        }
    }

    private void OnSnakeStatsChanged(int playerId, Type type)
    {
        if(type == typeof(ClockTimeStat))
        {
            if(ShouldRecord(playerId))
            {
                TryActiveRecord();
            } else
            {
                TryInactiveRecord();
            }
        }
    }

    private bool ShouldRecord(int playerId)
    {
        return players[playerId].Snake.Stats().GetStats<ClockTimeStat>().Value;
    }

    private void TryActiveRecord()
    {
        if(ManagerLocator.TryGetManager(out MementoManagerBehaviour mementoManagerBehaviour))
        {
            mementoManagerBehaviour.Active();
        }
    }

    private void TryInactiveRecord()
    {
        if (ManagerLocator.TryGetManager(out MementoManagerBehaviour mementoManagerBehaviour))
        {
            mementoManagerBehaviour.Inactive();
        }
    }

    public bool CanPressKey(string key)
    {
        foreach (int playerId in players.Keys)
        {
            PlayerInputHandler inputHandler = (players[playerId].Player as ControllablePlayer).InputHandler;
            if(inputHandler.LeftInput == key || inputHandler.RightInput == key)
            {
                return false;
            }    
        }

        return true;
    }

    public bool CanStartGame()
    {
        return players.Count > 0;
    }
}
