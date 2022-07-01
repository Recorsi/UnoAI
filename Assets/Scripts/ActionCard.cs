using UnityEngine;

[CreateAssetMenu(fileName = "New Action Card", menuName = "Cards/Action Card")]
public class ActionCard : ScriptableObject
{
    public int index;

    public enum Color
    {
        red,
        green,
        blue,
        yellow
    }
    public Color color;

    public enum Type
    {
        draw2,
        reverse,
        skip
    }
    public Type type;
}
