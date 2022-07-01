using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] GameObject UICanvas;

    [Header("UI Images")]
    [SerializeField] Sprite numberCardBG;
                     
    [SerializeField] Sprite draw2Card;
    [SerializeField] Sprite reverseCard;
    [SerializeField] Sprite skipCard;
                     
    [SerializeField] Sprite colorChangeCard;
    [SerializeField] Sprite draw4Card;

    [Header("Card Display Prefabs")]
    public GameObject numberCardPrefab;
    public GameObject actionCardPrefab;
    public GameObject wildCardPrefab;

    [Header("Card Arrays")]
    [SerializeField] private NumberCard[] numberCards;
    [SerializeField] private ActionCard[] actionCards;
    [SerializeField] private WildCard[] wildCards;

    [SerializeField] private List<GameObject> gameCards;

    private void Awake()
    {
        for (int i = 0; i < numberCards.Length; i++)
        {
            GameObject card = Instantiate(numberCardPrefab, UICanvas.transform);
            gameCards.Add(card);
            card.name = "NumberCard" + numberCards[i].index;
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
        for (int i = 0; i < actionCards.Length; i++)
        {
            GameObject card = Instantiate(actionCardPrefab, UICanvas.transform);
            gameCards.Add(card);
            card.name = "ActionCard" + actionCards[i].index;
            card.GetComponentInChildren<TextMeshProUGUI>().text = numberCards[i].cardNumber.ToString();
            switch (actionCards[i].color)
            {
                case ActionCard.Color.red:
                    card.GetComponent<Image>().color = Color.red;
                    break;
                case ActionCard.Color.green:
                    card.GetComponent<Image>().color = Color.green;
                    break;
                case ActionCard.Color.blue:
                    card.GetComponent<Image>().color = Color.blue;
                    break;
                case ActionCard.Color.yellow:
                    card.GetComponent<Image>().color = Color.yellow;
                    break;
                default:
                    break;
            }
            switch (actionCards[i].type)
            {
                case ActionCard.Type.draw2:
                    card.GetComponent<Image>().sprite = draw2Card;
                    break;
                case ActionCard.Type.reverse:
                    card.GetComponent<Image>().sprite = reverseCard;
                    break;
                case ActionCard.Type.skip:
                    card.GetComponent<Image>().sprite = skipCard;
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < wildCards.Length; i++)
        {
            GameObject card = Instantiate(wildCardPrefab, UICanvas.transform);
            gameCards.Add(card);
            card.name = "WildCard" + wildCards[i].index;
            switch (wildCards[i].type)
            {
                case WildCard.Type.colorChange:
                    card.GetComponent<Image>().sprite = colorChangeCard;
                    break;
                case WildCard.Type.draw4:
                    card.GetComponent<Image>().sprite = draw4Card;
                    break;
                default:
                    break;
            }
        }

        Shuffle(gameCards);
    }

    public void Shuffle(List<GameObject> gos)
    {
        for (int i = 0; i < gos.Count; i++)
        {
            GameObject temp = gos[i];
            int randomIndex = Random.Range(i, gos.Count);
            gos[i] = gos[randomIndex];
            gos[randomIndex] = temp;
        }
    }
}
