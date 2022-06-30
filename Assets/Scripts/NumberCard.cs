using UnityEngine;

[CreateAssetMenu(fileName = "New Number Card", menuName = "Cards/Number Card")]
public class NumberCard : ScriptableObject
{
    public int index;
    public int cardNumber;

    public enum Color
    {
        red,
        green,
        blue,
        yellow
    }
    public Color color;

}
