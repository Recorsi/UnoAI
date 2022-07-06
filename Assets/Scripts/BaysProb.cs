using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class BaysProb
{

    //colors should always be red,blue,yellow,green
    private static int[] RedCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static int[] BlueCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static int[] YellowCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static int[] GreenCards = new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    //RedCards.CopyTo(BlueCards, 0);
    //RedCards.CopyTo(YellowCards, 0);
    //RedCards.CopyTo(GreenCards, 0);
    private static int[] special = new int[] { 4, 4 };
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

        if (chanceOfDrawingSpecial() > chanceOfDrawingNum())
        {
            Debug.Log("Its gonna be a special chance: " + chanceOfDrawingSpecial());

            for (int i = 0; i < AllCards.Length; i++)
            {
                if (cardColorChance < chanceOfDrawingColor(i) * chanceOfDrawingSpecialColor(i))
                {
                    cardColor = i;
                    cardColorChance = chanceOfDrawingColor(i) * chanceOfDrawingSpecialColor(i);
                }
            }
        }
        else
        {
            Debug.Log("Its gonna be a number chance: " + chanceOfDrawingNum());

            for (int i = 0; i < AllCards.Length - 1; i++)
            {
                if (cardColorChance < chanceOfDrawingColor(i) * chanceOfDrawingNumColor(i))
                {
                    cardColor = i;
                    cardColorChance = chanceOfDrawingColor(i) * chanceOfDrawingNumColor(i);
                }
            }
        }

        Debug.Log("Its gonna be color " + cardColor + " chance: " + cardColorChance);

    }

}
