using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectInitialPool", menuName = "Custom/GameObjectInitialPool")]
public class GameObjectInitialPool : ScriptableObject
{
    [SerializeField]
    private List<GameObjectItem> gameObjectList;
    public List<GameObjectItem> GameObjectList => gameObjectList;
}

[Serializable]
public class GameObjectItem
{
    public GameObject gameObjectPrefab;
    public int initialAmount;
}
