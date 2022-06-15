using System;
using UnityEngine;

public class FoodSpawnerManager : MonoBehaviour
{
    public IWorldObject CurrentWorldObject { get; private set; }

    public event Action<IWorldObject> OnFoodChanged;

    private void OnEnable()
    {
        ManagerLocator.RegisterManager(this);
    }

    private void Start()
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnGameStarted += OnGameStarted;
            ingameManager.OnGameEnded += OnGameEnded;
        }
    }

    void OnGameStarted()
    {
        SpawnObject();
    }

    void OnGameEnded(int winnerPlayerId)
    {
        CurrentWorldObject.RemoveHitListener(OnObjectHit);
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            poolManager.Despawn(((MonoBehaviour)CurrentWorldObject).gameObject);
        }
    }

    private void SpawnObject()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager) &&
            ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            IngameFood food = ingameManager.GetNewFood();
            GameObject newObj = poolManager.Spawn(food.FoodPrefab.name);
            if (newObj == null) return;
            CurrentWorldObject = newObj.GetComponent<IWorldObject>();
            CurrentWorldObject.AddHitListener(OnObjectHit);
            newObj.transform.position = ingameManager.GetSpawnPoint();
            newObj.SetActive(true);

            OnFoodChanged?.Invoke(CurrentWorldObject);
        }
    }

    private void OnObjectHit()
    {
        CurrentWorldObject.RemoveHitListener(OnObjectHit);
        SpawnObject();
    }
}
