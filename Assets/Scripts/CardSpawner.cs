using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    //Card Display Prefabs
    public GameObject numberCardPrefab;
    public GameObject actionCardPrefab;
    public GameObject wildCardPrefab;

    [SerializeField] private NumberCard[] numberCards;
    private void Awake()
    {
        for (int i = 0; i < numberCards.Length; i++)
        {
            GameObject card = Instantiate(numberCardPrefab);
            card.GetComponentInChildren<TextMeshProUGUI>().text = numberCards[i].cardNumber.ToString();
            switch (numberCards[i].color)
            {
                case NumberCard.Color.red:
                    card.GetComponent<Image>().color = Color.red;
                    break;
                case NumberCard.Color.green:
                    card.GetComponent<Image>().color = Color.green;
                    break;
                case NumberCard.Color.blue:
                    card.GetComponent<Image>().color = Color.blue;
                    break;
                case NumberCard.Color.yellow:
                    card.GetComponent<Image>().color = Color.yellow;
                    break;
                default:
                    break;
            }
        }
    }
}
