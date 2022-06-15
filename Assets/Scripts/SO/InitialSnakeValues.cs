using UnityEngine;

[CreateAssetMenu(fileName = "InitialSnakeValues", menuName = "Custom/InitialSnakeValues")]
public class InitialSnakeValues : ScriptableObject
{
    [SerializeField]
    private int initialLife;
    [SerializeField]
    private float updateInSecond;
    [SerializeField]
    private float minUpdateInSecond;
    [SerializeField]
    private float maxUpdateInSecond;

    public int InitialLife => initialLife;
    public float InitialSpeed => updateInSecond;
    public float MinSpeed => minUpdateInSecond;
    public float MaxSpeed => maxUpdateInSecond;
}