using UnityEngine;

[CreateAssetMenu(fileName = "New Wild Card", menuName = "Cards/Wild Card")]
public class WildCard : ScriptableObject
{
    public int index;

    public enum Type
    {
        colorChange,
        draw4
    }
    public Type type;
}
