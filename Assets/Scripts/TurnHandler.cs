using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnHandler : MonoBehaviour
{
    public Transform discardPilePos;

    public List<Player> playerList = new List<Player>();

    public List<GameObject> discardPile = new List<GameObject>();

    CardSpawner cardSpawner;

    int activePlayer = 0;
    void Start()
    {
        cardSpawner = FindObjectOfType<CardSpawner>();

        int playerCount = 2;

        for (int i = 0; i < playerCount; i++)
        {
            Player player = new Player();
            player.playerCards = new List<GameObject>();
            playerList.Add(player);

            //Give starting cards to player from deck
            for (int j = 0; j < 7; j++)
            {
                player.playerCards.Add(cardSpawner.gameCards[j]);
                cardSpawner.gameCards.RemoveAt(j);
            }
        }

        //put first card on discard pile
        discardPile.Add(cardSpawner.gameCards[0]);
        cardSpawner.gameCards[0].transform.position = discardPilePos.position;

        //begin first turn for player 0
        setActivePlayer(0);  
    }

    void setActivePlayer(int index)
    {
        switch (index)
        {
            case 0:
                playerList[0].activeTurn = true;
                playerList[1].activeTurn = false;

                //disable player 1s cards
                foreach (var card in playerList[0].playerCards)
                    card.GetComponent<Button>().interactable = true;
                foreach (var card in playerList[1].playerCards)
                    card.GetComponent<Button>().interactable = false;

                activePlayer = 0;
                break;
            case 1:
                playerList[0].activeTurn = false;
                playerList[1].activeTurn = true;

                //disable player 0s cards
                foreach (var card in playerList[0].playerCards)
                    card.GetComponent<Button>().interactable = true;
                foreach (var card in playerList[1].playerCards)
                    card.GetComponent<Button>().interactable = false;

                activePlayer = 1;
                break;
            default:
                break;
        }

        print("Player " + (activePlayer + 1) + "'s turn");
    }

    //On card button
    public void PlayCard(GameObject playedCard)
    {
        //find card array index in player array
        int currentIndex = 0;
        for (int i = 0; i < playerList[activePlayer].playerCards.Count; i++)
        {
            if (playerList[activePlayer].playerCards[i] == playedCard)
            {
                currentIndex = i;
            }
        }

        if (CheckPlayable(currentIndex))
        {
            print("played card");

            //Give turn to other player (and disable the other players cards)
            switch (activePlayer)
            {
                case 0:
                    setActivePlayer(1);               
                    break;
                case 1:
                    setActivePlayer(0);
                    break;
                default:
                    break;
            }
        }
        else
            print("Card not playable");
    }
    
    private bool CheckPlayable(int index)
    {
        GameObject discardTemp = discardPile[discardPile.Count - 1];

        GameObject card = playerList[activePlayer].playerCards[index];

        //if card is number card
        if (card.GetComponent<NumberCard>() != null)
        {
            //if discardpile contains numbercard
            if (discardTemp.GetComponent<NumberCard>() != null)
            {
                //compare cardnumber and color
                if (card.GetComponent<NumberCard>().cardNumber == discardTemp.GetComponent<NumberCard>().cardNumber || card.GetComponent<NumberCard>().color == discardTemp.GetComponent<NumberCard>().color)
                {
                    return true;
                }
            }
            //if discardpile contains actioncard
            else if (discardTemp.GetComponent<ActionCard>() != null)
            {
                //compare color
                if (card.GetComponent<NumberCard>().color.ToString() == discardTemp.GetComponent<ActionCard>().color.ToString())
                {
                    return true;
                }
            }
            //if discardpile contains wildcard
            else if (discardTemp.GetComponent<WildCard>() != null)
            {
                //TODO: Wildcard color pick values
            }
        }
        //if card is action card
        else if (card.GetComponent<ActionCard>() != null)
        {
            //if discardpile contains numbercard
            if (discardTemp.GetComponent<NumberCard>() != null)
            {
                //compare color
                if (card.GetComponent<ActionCard>().color.ToString() == discardTemp.GetComponent<NumberCard>().color.ToString())
                {
                    return true;
                }
            }
            //if discardpile contains actioncard
            if (discardTemp.GetComponent<ActionCard>() != null)
            {
                //compare color
                if (card.GetComponent<ActionCard>().color.ToString() == discardTemp.GetComponent<ActionCard>().color.ToString())
                {
                    return true;
                }
                //compare action
                if (card.GetComponent<ActionCard>().type.ToString() == discardTemp.GetComponent<ActionCard>().type.ToString())
                {
                    return true;
                }
            }
            //if discardpile contains wildcard
            else if (discardTemp.GetComponent<WildCard>() != null)
            {
                //TODO: Wildcard color pick values
            }
        }
        //if card is wild card
        else if (card.GetComponent<WildCard>() != null)
        {
            //no check needed
            return true;
        }
        return false;
    }
}