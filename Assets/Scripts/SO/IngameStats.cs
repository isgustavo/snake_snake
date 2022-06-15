using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngameStats", menuName = "Custom/IngameStats")]
public class IngameStats : ScriptableObject
{
    [SerializeField]
    private int squareArenaSize;
    [SerializeField]
    private int initialSnakeSize;
    [SerializeField]
    private float rewindSnakeInSecond;
    [SerializeField]
    private List<IngameFood> ingameFoods;
    [ReadOnly]
    [SerializeField]
    private float foodDropTotalPercentage;

    public int SquareArenaSize => squareArenaSize;
    public int InitialSnakeSize => initialSnakeSize;
    public float RewindSnakeInSecond => rewindSnakeInSecond;

    public IngameFood GetFood()
    {
        float randomValue = UnityEngine.Random.Range(0, 101);
        float percentage = 0f;
        foreach(IngameFood food in ingameFoods)
        {
            percentage += food.DropPercentage;
            if(randomValue <= percentage)
            {
                return food;
            }
        }

        return null;
    }

    private void OnValidate()
    {
        float value = 0f;
        foreach(IngameFood food in ingameFoods)
        {
            value += food.DropPercentage;
        }

        foodDropTotalPercentage = value;
    }
}

[System.Serializable]
public class IngameFood
{
    [SerializeField]
    private GameObject foodPrefab;
    [SerializeField]
    private float dropPercentage;

    public GameObject FoodPrefab => foodPrefab;
    public float DropPercentage => dropPercentage;
}
