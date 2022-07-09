using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class BaysCards
{
    public int color { get; private set; }
    public double colorProb { get; private set; }
    public int num { get; private set; }
    public double numProb { get; private set; }

    public BaysCards(int color, double colorProb, int num, double numProb)
    {
        this.color = color;
        this.colorProb = colorProb;
        this.num = num;
        this.numProb = numProb;
    }
}

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

    List<BaysCards> EnemyDeck = new List<BaysCards>();

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
    double chanceOfDrawingSpecialGivenColor(int i)
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
    double chanceOfDrawingNumGivenColor(int i)
    {
        double total = AllCards[i].Sum();
        double personal = 0;
        for (int j = 0; j < 10; j++)
        {
            personal += AllCards[i][j];
        }

        return checkOnlyLeft(total, personal);
    }
    int mostProbabolNumGivenColorAndNOTNum(int color, int num)
    {
        double mostLikely = 0;
        double thatSum = AllCards[color].Sum();
        int res = 0;
        for (int i = 0; i < AllCards[color].Length; i++)
        {
            if (i != num && AllCards[color][i] / thatSum > mostLikely)
            {
                mostLikely = AllCards[color][i] / AllCards[color].Sum();
                res = i;
            }
        }
        return res;
    }
    int mostProbebolColorGivenNOTColor(int color)
    {
        double mostLikely = 0;
        int res = 0;
        for(int i = 0; i < AllCards.Length-1; i++)//-1 cus wild will always work
        {
            if (i != color && chanceOfDrawingColor(i) > mostLikely)
            {
                mostLikely = chanceOfDrawingColor(i);
                res = i;
            }
        }
        return res;
    }
    public void enemyDrewCardInsteadOfPlay(GameObject cardOnTable)
    {
        CardValueSaver cardOnTableValues = cardOnTable.GetComponent<CardValueSaver>();
        int color = mostProbebolColorGivenNOTColor(giveColorIntCode(cardOnTableValues));
        int num = 0;
        if (cardOnTableValues.cardType.Equals(CardValueSaver.CardType.wild))//special case when card was wild
        {
            
        }
        else
        {
            if (cardOnTableValues.cardType.Equals(CardValueSaver.CardType.number))
            {
                num = mostProbabolNumGivenColorAndNOTNum(color, cardOnTableValues.cardNumber);
            }
            else
            {
                if (cardOnTableValues.actionType.Equals(CardValueSaver.ActionType.skip))
                {
                    num = mostProbabolNumGivenColorAndNOTNum(color, 10);
                }
                if (cardOnTableValues.actionType.Equals(CardValueSaver.ActionType.reverse))
                {
                    num = mostProbabolNumGivenColorAndNOTNum(color, 11);
                }
                if (cardOnTableValues.actionType.Equals(CardValueSaver.ActionType.draw2))
                {
                    num = mostProbabolNumGivenColorAndNOTNum(color, 12);
                }
            }
        }

        EnemyDeck.Add(new BaysCards(color, chanceOfDrawingColor(color), num, chanceOfDrawingSpecificCardInGroup(color,num)));
        if (AllCards[color][num] > 0)
        {
            AllCards[color][num]--;
        }
        else
        {
            Debug.Log("Critical problem lost track of cards");
        }

        Debug.Log("its color: " + color);
        Debug.Log("its num: " + num);
    }
    public void predictACard()
    {
        int cardColor = 0;
        double cardColorChance = 0;

        int cardNum = 0;
        double cardNumChance = 0;

        if (chanceOfDrawingSpecial() > chanceOfDrawingNum())//Special cards
        {
            for (int i = 0; i < AllCards.Length; i++)
            {
                if (cardColorChance < chanceOfDrawingColor(i) * chanceOfDrawingSpecialGivenColor(i))
                {
                    cardColor = i;
                    cardColorChance = chanceOfDrawingColor(i) * chanceOfDrawingSpecialGivenColor(i);
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
                if (cardColorChance < chanceOfDrawingColor(i) * chanceOfDrawingNumGivenColor(i))
                {
                    cardColor = i;
                    cardColorChance = chanceOfDrawingColor(i) * chanceOfDrawingNumGivenColor(i);
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
        //Debug.Log("Its gonna be color " + cardColor + " chance: " + cardColorChance);
        //Debug.Log("Its gonna be num " + cardNum + " chance: " + cardNumChance);

        EnemyDeck.Add(new BaysCards(cardColor, cardColorChance, cardNum, cardNumChance));
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
            addACardToAIDeck(card, false);
        }
    }
    private void removeCardFromGroup(int group, CardValueSaver cardValues, bool otherPlaydIt)
    {
        int index = -1;

        if (cardValues.cardType.Equals(CardValueSaver.CardType.number))
        {
            index = cardValues.cardNumber;
        }
        if (cardValues.cardType.Equals(CardValueSaver.CardType.action))
        {
            if (cardValues.actionType.Equals(CardValueSaver.ActionType.skip))
            {
                index = 10;
            }
            if (cardValues.actionType.Equals(CardValueSaver.ActionType.reverse))
            {
                index = 11;
            }
            if (cardValues.actionType.Equals(CardValueSaver.ActionType.draw2))
            {
                index = 12;
            }

        }

        if (index != -1)
        {
            if (AllCards[group][index] == 0)
            {
                updateBelif(new int[] { group, index }, otherPlaydIt);
            }
            else
            {
                AllCards[group][index]--;
                if (otherPlaydIt)
                {
                    updateBelif(new int[] { 4, index }, otherPlaydIt);
                }
            }
        }
        else
        {
            Debug.Log("Card does not exist in group");
        }
        
    }
    public GameObject takeAturn(List<GameObject> myCards, GameObject cardOnTable)
    {
        CardValueSaver tableCardValues = cardOnTable.GetComponent<CardValueSaver>();

        List<GameObject> playbleCards = new List<GameObject>();//lets figure out what cards to consider
        foreach (GameObject card in myCards)
        {
            if (cardIsPlayable(card, tableCardValues))
            {
                playbleCards.Add(card);
            }
        }

        //^special case^ if we cant play any one card return null
        if (playbleCards.Count == 0)
        {
            return null;
        }
        //^special case^ if we only have one card to play just play that!
        if (playbleCards.Count == 1)
        {
            return playbleCards[0];
        }

        double currentBest = 100;
        GameObject bestCard = playbleCards[0];

        //TODO some logic for wild cards ya know
        foreach (GameObject card in playbleCards)//we should replace this with the nural logic 
        {
            if (colorValueForEnemy(card) + typeValueForEnemy(card) < currentBest)
            {
                bestCard = card;
                currentBest = colorValueForEnemy(card) + typeValueForEnemy(card);
            }
        }

        Debug.Log("best color would be: " + bestCard.GetComponent<CardValueSaver>().color);
        Debug.Log("with type: " + bestCard.GetComponent<CardValueSaver>().cardType);
        Debug.Log("prob of: " + currentBest);

        return bestCard;
    }
    public void addACardToAIDeck(GameObject card, bool otherPlaydIt)
    {
        CardValueSaver cardValues = card.GetComponent<CardValueSaver>();

        //Figure out if it is a special
        if (cardValues.cardType.Equals(CardValueSaver.CardType.wild))
        {
            int index;
            if (cardValues.wildType.Equals(CardValueSaver.WildType.colorChange))
            {
                index = 1;
            }
            else
            {
                index = 0;
            }

            if (AllCards[4][index] == 0)
            {
                updateBelif(new int[] {4,index }, otherPlaydIt);
            }
            else
            {
                AllCards[4][index]--;
                if (otherPlaydIt)
                {
                    updateBelif(new int[] { 4, index }, otherPlaydIt);
                }
            }
            return;
        }

        removeCardFromGroup(giveColorIntCode(cardValues), cardValues, otherPlaydIt);
    }
    public void updateBelif(int[] cardIndex, bool otherPlaydIt)
    {
        bool foundCard = false;
        double leastLikely = Math.Log(100);//we use log because of the classic decimal issue in CS
        int cardR = -1;

        for (int i = 0; i < EnemyDeck.Count; i++)
        {
            if (leastLikely > Math.Log(EnemyDeck[i].colorProb * EnemyDeck[i].numProb))
            {
                leastLikely = Math.Log(EnemyDeck[i].colorProb * EnemyDeck[i].numProb);
                cardR = i;
            }

            if (EnemyDeck[i].color == cardIndex[0] && EnemyDeck[i].num == cardIndex[1])
            {

                foundCard = true;
                if (otherPlaydIt)
                {
                    Debug.Log("Yea i knew you had that");
                }
                else
                {
                    Debug.Log("O i thought you had that");
                }
                EnemyDeck.RemoveAt(i);
            }
        }

        if (foundCard && !otherPlaydIt)
        {
            predictACard();
        }
        else if (otherPlaydIt && cardR != -1)
        {
            EnemyDeck.RemoveAt(cardR);
        }
        else
        {
            Debug.Log("==========================================================================");
            Debug.Log("Fatal error bays has lost track of the cards");
            Debug.Log(cardIndex[0] + " "+ cardIndex[1]);
        }
    }
    private double colorValueForEnemy(GameObject card)
    {
        CardValueSaver cardValues = card.GetComponent<CardValueSaver>();
        double bestEnemeyChance = 0;

        foreach(BaysCards eCard in EnemyDeck)
        {
            if (giveColorIntCode(cardValues) == eCard.color)
            {
                if (bestEnemeyChance < eCard.colorProb)
                {
                    bestEnemeyChance = eCard.colorProb;
                }
            }
        }
        return bestEnemeyChance;

    }
    private double typeValueForEnemy(GameObject card)
    {
        CardValueSaver cardValues = card.GetComponent<CardValueSaver>();
        double bestEnemeyChance = 0;

        if (cardValues.cardType.Equals(CardValueSaver.CardType.number))
        {
            foreach (BaysCards eCard in EnemyDeck)
            {
                if (eCard.num < 10 && eCard.num == cardValues.cardNumber)
                {
                    if (bestEnemeyChance < eCard.colorProb)
                    {
                        bestEnemeyChance = eCard.colorProb;
                    }
                }
            }
        }
        else
        {
            return 0;
        }

        return bestEnemeyChance;
    }
    private bool cardIsPlayable(GameObject card, CardValueSaver tableCardValues)
    {
        CardValueSaver cardValues = card.GetComponent<CardValueSaver>();

        if (tableCardValues.color == cardValues.color)//is it same color?
        {
            return true;
        }
        else
        {
            if (cardValues.cardType.Equals(CardValueSaver.CardType.number) && tableCardValues.cardType.Equals(CardValueSaver.CardType.number))//is it same number
            {
                if (tableCardValues.cardNumber == cardValues.cardNumber)
                {
                    return true;
                }
            }
            if (cardValues.cardType.Equals(CardValueSaver.CardType.action) && tableCardValues.cardType.Equals(CardValueSaver.CardType.action))//is it same type
            {
                if (tableCardValues.actionType == cardValues.actionType)
                {
                    return true;
                }
            }
        }
        if (cardValues.cardType.Equals(CardValueSaver.CardType.wild))
        {
            return true;
        }
        return false;
        
    }
    private int giveColorIntCode(CardValueSaver cardValues)
    {
        if (cardValues.cardType.Equals(CardValueSaver.CardType.wild))
        {
            return 4;
        }
        //Figure out the color
        if (cardValues.color.Equals(CardValueSaver.Color.red))
        {
            return 0;
        }
        if (cardValues.color.Equals(CardValueSaver.Color.blue))
        {
            return 1;
        }
        if (cardValues.color.Equals(CardValueSaver.Color.yellow))
        {
            return 2;
        }
        if (cardValues.color.Equals(CardValueSaver.Color.green))
        {
            return 3;
        }
        throw new Exception("bays could not identify the color of a given card :(");
    }




    //debug functions
    public void showEntireArrayStructure()
    {
        Debug.Log("0,1,2,3,4,5,6,7,8,9,skip,reverse,+1");
        foreach (int[] cards in AllCards)
        {
            if (cards.Length > 2)
            {
                Debug.Log(cards[0] + ", " + cards[1] + ", " + cards[2] + ", " + cards[3] + ", " + cards[4] + ", " + cards[5] + ", " + cards[6] + ", " + cards[7] + ", " + cards[8] + ", " + cards[9] + ", " + cards[10] + ", " + cards[11] + ", " + cards[12]);
                continue;
            }
            Debug.Log(cards[0] + ", " + cards[1]);
        }
    }
}

