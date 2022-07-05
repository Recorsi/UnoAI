using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardsDisplay : MonoBehaviour
{
    TurnHandler turnHandler;

    [SerializeField] Transform player0InitialPos;
    [SerializeField] Transform player1InitialPos;

    private void Start()
    {
        turnHandler = FindObjectOfType<TurnHandler>();
    }

    public void DisplayPlayerCards()
    {
        int tempPos = 0;
        for (int i = 0; i < turnHandler.playerList[0].playerCards.Count; i++)
        {
            GameObject card = turnHandler.playerList[0].playerCards[i];
            card.transform.position = player0InitialPos.position + new Vector3(tempPos, 0, 0);
            card.transform.parent = player0InitialPos;
            card.transform.SetSiblingIndex(i);
            tempPos += 60;
        }
        tempPos = 0;
        for (int i = 0; i < turnHandler.playerList[1].playerCards.Count; i++)
        {
            GameObject card = turnHandler.playerList[1].playerCards[i];
            card.transform.position = player1InitialPos.position + new Vector3(tempPos, 0, 0);
            card.transform.parent = player1InitialPos;
            card.transform.SetSiblingIndex(i);
            tempPos += 60;
        }
    }
}
