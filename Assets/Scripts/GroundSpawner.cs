using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [SerializeField]
    List<GameObject> walls = new List<GameObject>();

    List<GameObject> ground = new List<GameObject>();

    private void OnEnable()
    {
        if(ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager) &&
           ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            for (int i = -ingameManager.IngameStats.SquareArenaSize; i < ingameManager.IngameStats.SquareArenaSize; i++)
            {
                for (int j = -ingameManager.IngameStats.SquareArenaSize; j < ingameManager.IngameStats.SquareArenaSize; j++)
                {
                    GameObject newObj = poolManager.Spawn("GroundCube");
                    if (newObj == null) return;
                    newObj.transform.position = new Vector3(i, Random.Range(-.15f, .15f), j);
                    //newObj.transform.SetParent(this.transform);
                    newObj.SetActive(true);
                    ground.Add(newObj);
                }
            }

            SetupWallColliders(ingameManager.IngameStats.SquareArenaSize);
        }
    }

    private void OnDisable()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            foreach (GameObject g in ground)
            {
                if(g != null)
                {
                    poolManager.Despawn(g);
                }
            }
        }
    }

    void SetupWallColliders(int arenaSize)
    {
        walls[0].transform.position = new Vector3(0f, 1f, arenaSize);
        walls[0].transform.localScale = new Vector3(arenaSize * 2.5f, 1f, 1f);

        walls[1].transform.position = new Vector3(0f, 1f, -arenaSize - 1);
        walls[1].transform.localScale = new Vector3(arenaSize * 2.5f, 1f, 1f);

        walls[2].transform.position = new Vector3(arenaSize, 1f, 0f);
        walls[2].transform.localScale = new Vector3(1f, 1f, arenaSize * 2.5f);

        walls[3].transform.position = new Vector3(-arenaSize - 1, 1f, 0f);
        walls[3].transform.localScale = new Vector3(1f, 1f, arenaSize * 2.5f);
    }
}
