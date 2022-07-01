using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardValueSaver : MonoBehaviour
{
    public enum CardType
    {
        number,
        action,
        wild
    }
    public CardType cardType;

    public int cardNumber;

    public enum Color
    {
        red,
        green,
        blue,
        yellow
    }
    public Color color;

    public enum ActionType
    {
        draw2,
        reverse,
        skip
    }
    public ActionType actionType;

    public enum Type
    {
        colorChange,
        draw4
    }
    public Type type;
}
