using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{
    CardSpawner cardSpawner;
    public List<Player> playerList = new List<Player>();
    void Start()
    {
        cardSpawner = FindObjectOfType<CardSpawner>();

        int playerCount = 2;

        for (int i = 0; i < playerCount; i++)
        {
            Player player = new Player();
            playerList.Add(player);
            player.playerCards = new List<GameObject>();

            //Give starting cards to player from deck
            for (int j = 0; j < 7; j++)
            {
                player.playerCards.Add(cardSpawner.gameCards[j]);
                cardSpawner.gameCards.RemoveAt(j);
            }
        }
    }
}
