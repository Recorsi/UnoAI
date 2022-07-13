using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardsDisplay : MonoBehaviour
{
    TurnHandler turnHandler;

    [SerializeField] Transform[] playerInitialPositions;

    private void Start()
    {
        turnHandler = FindObjectOfType<TurnHandler>();
    }

    public void DisplayPlayerCards()
    {
        int tempPos = 0;
        for (int i = 0; i < turnHandler.playerList.Count; i++)
        {
            for (int j = 0; j < turnHandler.playerList[i].playerCards.Count; j++)
            {
                GameObject card = turnHandler.playerList[i].playerCards[j];
                card.transform.position = playerInitialPositions[i].position + new Vector3(tempPos, 0, 0);
                card.transform.parent = playerInitialPositions[i];
                card.transform.SetSiblingIndex(j);
                tempPos += 60;
            }
            tempPos = 0;
        }
    }
}
