using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManagerBehaviour : MonoBehaviour
{
    public static Vector3 POOL_POSITION = Vector3.one * 2000;

    [SerializeField]
    private GameObjectInitialPool initialPool;
    public int InitialPoolCount => initialPool?.GameObjectList.Count ?? 0;

    Dictionary<int, Queue<GameObject>> objectPools = new Dictionary<int, Queue<GameObject>>();

    public event Action OnPoolLoadUpdate;
    public event Action OnPoolFinished;

    private void OnEnable()
    {
        ManagerLocator.RegisterManager(this);
    }

    private void OnDisable()
    {
        ManagerLocator.UnRegisterManager<PoolManagerBehaviour>();
    }

    public IEnumerator CreatePool()
    {
        if(initialPool != null)
        {
            foreach (GameObjectItem obj in initialPool.GameObjectList)
            {
                int nameHashCode = obj.gameObjectPrefab.name.GetHashCode();

                if (objectPools.ContainsKey(nameHashCode) == false)
                    objectPools.Add(nameHashCode, new Queue<GameObject>());

                for (int i = 0; i < obj.initialAmount; i++)
                {
                    GameObject newObj = GameObject.Instantiate(obj.gameObjectPrefab, POOL_POSITION, Quaternion.identity);
                    newObj.name = obj.gameObjectPrefab.name;
                    newObj.SetActive(false);
                    objectPools[nameHashCode].Enqueue(newObj);

                    yield return new WaitForEndOfFrame();
                }

                OnPoolLoadUpdate?.Invoke();
            }
        }

        OnPoolFinished?.Invoke();
    }

    public GameObject Spawn (string name)
    {
        int nameHashCode = name.GetHashCode();
        if (objectPools.ContainsKey(nameHashCode) == false || objectPools[nameHashCode].Count <= 0)
        {
            Debug.LogError($"Pool is empty {name}");
            return null;
        }
        else
        {
            return objectPools[nameHashCode].Dequeue();
        }
    }

    public void Despawn (GameObject obj)
    {
        int nameHashCode = obj.name.GetHashCode();

        obj.transform.SetParent(null);
        obj.SetActive(false);
        obj.transform.position = POOL_POSITION;
        obj.transform.rotation = Quaternion.identity;
        objectPools[nameHashCode].Enqueue(obj);
    }
}
