using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class BaysProb
{

    //0,1,2,3,4,5,6,7,8,9,skip,reverse,draw2
    private static int[] RedCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static int[] BlueCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static int[] YellowCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static int[] GreenCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    //RedCards.CopyTo(BlueCards, 0);
    //RedCards.CopyTo(YellowCards, 0);
    //RedCards.CopyTo(GreenCards, 0);

    //color change, draw4
    private static int[] special = new int[] { 4, 4 };

    //colors should always be red,blue,yellow,green
    int[][] AllCards = new int[][] { RedCards, BlueCards, YellowCards, GreenCards, special };

    List<int[]> EnemyDeck = new List<int[]>();

    double checkOnlyLeft(double total, double personal)
    {
        if (total - personal <= 0)
        {
            return (1);
        }
        return (personal / total);
    }

    double chanceOfDrawingColor(int colorNum)
    {
        double total = 0;
        double personal = AllCards[colorNum].Sum();
        foreach (int[] card in AllCards) { total += card.Sum(); }

        return checkOnlyLeft(total, personal);
    }

    double chanceOfDrawingNum()
    {
        double total = 0;
        double personal = 0;
        foreach (int[] card in AllCards) { total += card.Sum(); }
        for (int i = 0; i < AllCards.Length - 1; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                personal += AllCards[i][j];
            }
        }

        return checkOnlyLeft(total, personal);
    }
    double chanceOfDrawingSpecificCardInGroup(int group, int card)
    {
        return (double) AllCards[group][card]/ (double) AllCards[group].Sum();
    }

    double chanceOfDrawingSpecial()
    {
        double total = 0;
        double personal = 0;
        foreach (int[] card in AllCards) { total += card.Sum(); }
        for (int i = 0; i < AllCards.Length - 1; i++)
        {
            for (int j = 10; j < 13; j++)
            {
                personal += AllCards[i][j];
            }
        }
        personal += AllCards[4].Sum();

        return checkOnlyLeft(total, personal);
    }

    double chanceOfDrawingSpecialColor(int i)
    {
        double total = AllCards[i].Sum();
        double personal = 0;
        for (int j = 10; j < 13; j++)
        {
            personal += AllCards[i][j];
        }
        personal += AllCards[4].Sum();

        return checkOnlyLeft(total, personal);
    }

    double chanceOfDrawingNumColor(int i)
    {
        double total = AllCards[i].Sum();
        double personal = 0;
        for (int j = 0; j < 10; j++)
        {
            personal += AllCards[i][j];
        }

        return checkOnlyLeft(total, personal);
    }

    void predictACard()
    {
        int cardColor = 0;
        double cardColorChance = 0;

        int cardNum = 0;
        double cardNumChance = 0;

        if (chanceOfDrawingSpecial() > chanceOfDrawingNum())//Special cards
        {
            for (int i = 0; i < AllCards.Length; i++)
            {
                if (cardColorChance < chanceOfDrawingColor(i) * chanceOfDrawingSpecialColor(i))
                {
                    cardColor = i;
                    cardColorChance = chanceOfDrawingColor(i) * chanceOfDrawingSpecialColor(i);
                }
            }
            if (cardColor < 4)
            {
                for (int i = 10; i < 12; i++)
                {
                    if (cardNumChance < chanceOfDrawingSpecificCardInGroup(cardColor, i))
                    {
                        cardNum = i;
                        cardNumChance = chanceOfDrawingSpecificCardInGroup(cardColor, i);
                    }

                }
            }
            else
            {
                if (chanceOfDrawingSpecificCardInGroup(cardColor, 0) < chanceOfDrawingSpecificCardInGroup(cardColor, 1))
                {
                    cardNum = 1;
                    cardNumChance = chanceOfDrawingSpecificCardInGroup(cardColor, 1);
                }
                else
                {
                    cardNum = 0;
                    cardNumChance = chanceOfDrawingSpecificCardInGroup(cardColor, 0);
                }
            }

        }
        else//Number cards
        {

            for (int i = 0; i < AllCards.Length - 1; i++)
            {
                if (cardColorChance < chanceOfDrawingColor(i) * chanceOfDrawingNumColor(i))
                {
                    cardColor = i;
                    cardColorChance = chanceOfDrawingColor(i) * chanceOfDrawingNumColor(i);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (cardNumChance < chanceOfDrawingSpecificCardInGroup(cardColor, i))
                {
                    cardNum = i;
                    cardNumChance = chanceOfDrawingSpecificCardInGroup(cardColor, i);
                }
            }
        }
        Debug.Log("Its gonna be color " + cardColor + " chance: " + cardColorChance);
        Debug.Log("Its gonna be num " + cardNum + " chance: " + cardNumChance);
        AllCards[cardColor][cardNum]--;

    }

    public void makeInitalGuess(List<GameObject> myCards)
    {
        Debug.Log("Bays initial idea");
        arrangeMyCards(myCards);

        for(int i = 0; i < 7; i++)
        {
            predictACard();
        }
    }

    private void arrangeMyCards(List<GameObject> myCards)
    {
        foreach (GameObject card in myCards)
        {
            CardValueSaver cardValues = card.GetComponent<CardValueSaver>();

            //Figure out if it is a special
            if (cardValues.cardType.Equals(CardValueSaver.CardType.wild))
            {
                if (cardValues.wildType.Equals(CardValueSaver.WildType.colorChange))
                {
                    AllCards[4][1]--;
                }
                else
                {
                    AllCards[4][0]--;
                }
                continue;
            }

            //Figure out the color
            if (cardValues.color.Equals(CardValueSaver.Color.red))
            {
                removeCardFromGroup(0, cardValues);
            }
            if (cardValues.color.Equals(CardValueSaver.Color.blue))
            {
                removeCardFromGroup(1, cardValues);
            }
            if (cardValues.color.Equals(CardValueSaver.Color.yellow))
            {
                removeCardFromGroup(2, cardValues);
            }
            if (cardValues.color.Equals(CardValueSaver.Color.green))
            {
                removeCardFromGroup(3, cardValues);
            }
        }
    }

    private void removeCardFromGroup(int group, CardValueSaver cardValues)
    {
        if (cardValues.cardType.Equals(CardValueSaver.CardType.number))
        {
            AllCards[group][cardValues.cardNumber]--;
        }
        if (cardValues.cardType.Equals(CardValueSaver.CardType.action))
        {
            if (cardValues.actionType.Equals(CardValueSaver.ActionType.skip))
            {
                AllCards[group][10]--;
            }
            if (cardValues.actionType.Equals(CardValueSaver.ActionType.reverse))
            {
                AllCards[group][11]--;
            }
            if (cardValues.actionType.Equals(CardValueSaver.ActionType.draw2))
            {
                AllCards[group][12]--;
            }
        }
    }

}

