using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Colors")]
    public Color blackColor;
    public Color redColor;
    public Color greenColor;
    public Color blueColor;
    public Color yellowColor;
    [Space]

    [SerializeField] GameObject parentObj;

    [Header("UI Images")]
    [SerializeField] Sprite cardBG;
                     
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

    public List<GameObject> gameCards;

    TurnHandler turnHandler;

    private void Awake()
    {
        turnHandler = FindObjectOfType<TurnHandler>();

        for (int i = 0; i < numberCards.Length; i++)
        {
            GameObject card = Instantiate(numberCardPrefab, parentObj.transform);
            gameCards.Add(card);
            card.name = "NumberCard_" + numberCards[i].color + "_" + numberCards[i].cardNumber;
            card.GetComponent<Button>().onClick.AddListener(() => turnHandler.PlayCard(card));
            TextMeshProUGUI[] cardText = card.GetComponentsInChildren<TextMeshProUGUI>();

            //Save card values for later use
            CardValueSaver valueSaver = card.GetComponent<CardValueSaver>();
            valueSaver.cardType = CardValueSaver.CardType.number;
            valueSaver.cardNumber = numberCards[i].cardNumber;
            valueSaver.color = (CardValueSaver.Color)Enum.Parse(typeof(CardValueSaver.Color), numberCards[i].color.ToString());

            foreach (var text in cardText)
                text.text = numberCards[i].cardNumber.ToString();

            switch (numberCards[i].color)
            {
                case NumberCard.Color.red:
                    card.GetComponent<Image>().color = redColor;
                    break;
                case NumberCard.Color.green:
                    card.GetComponent<Image>().color = greenColor;
                    break;
                case NumberCard.Color.blue:
                    card.GetComponent<Image>().color = blueColor;
                    break;
                case NumberCard.Color.yellow:
                    card.GetComponent<Image>().color = yellowColor;
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < actionCards.Length; i++)
        {
            GameObject card = Instantiate(actionCardPrefab, parentObj.transform);
            gameCards.Add(card);
            card.name = "ActionCard_" + actionCards[i].color + "_" + actionCards[i].type;
            card.GetComponent<Button>().onClick.AddListener(() => turnHandler.PlayCard(card));

            //Save card values for later use
            CardValueSaver valueSaver = card.GetComponent<CardValueSaver>();
            valueSaver.cardType = CardValueSaver.CardType.action;
            valueSaver.color = (CardValueSaver.Color)Enum.Parse(typeof(CardValueSaver.Color), actionCards[i].color.ToString());
            valueSaver.actionType = (CardValueSaver.ActionType)Enum.Parse(typeof(CardValueSaver.ActionType), actionCards[i].type.ToString());

            switch (actionCards[i].color)
            {
                case ActionCard.Color.red:
                    card.GetComponent<Image>().color = redColor;
                    break;
                case ActionCard.Color.green:
                    card.GetComponent<Image>().color = greenColor;
                    break;
                case ActionCard.Color.blue:
                    card.GetComponent<Image>().color = blueColor;
                    break;
                case ActionCard.Color.yellow:
                    card.GetComponent<Image>().color = yellowColor;
                    break;
                default:
                    break;
            }

            Transform actionImage = card.transform.GetChild(0); //get action card image which is the first child
            switch (actionCards[i].type)
            {
                case ActionCard.Type.draw2:
                    actionImage.GetComponent<Image>().sprite = draw2Card;
                    break;
                case ActionCard.Type.reverse:
                    actionImage.GetComponent<Image>().sprite = reverseCard;
                    break;
                case ActionCard.Type.skip:
                    actionImage.GetComponent<Image>().sprite = skipCard;
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < wildCards.Length; i++)
        {
            GameObject card = Instantiate(wildCardPrefab, parentObj.transform);
            gameCards.Add(card);
            card.name = "WildCard_" + wildCards[i].type;
            card.GetComponent<Button>().onClick.AddListener(() => turnHandler.PlayCard(card));

            card.GetComponent<Image>().color = blackColor;

            //Save card values for later use
            CardValueSaver valueSaver = card.GetComponent<CardValueSaver>();
            valueSaver.cardType = CardValueSaver.CardType.wild;
            valueSaver.wildType = (CardValueSaver.WildType)Enum.Parse(typeof(CardValueSaver.WildType), wildCards[i].type.ToString());

            Transform wildImage = card.transform.GetChild(0); //get action card image which is the first child
            switch (wildCards[i].type)
            {
                case WildCard.Type.colorChange:
                    wildImage.GetComponent<Image>().sprite = colorChangeCard;
                    break;
                case WildCard.Type.draw4:
                    wildImage.GetComponent<Image>().sprite = draw4Card;
                    break;
                default:
                    break;
            }
        }

        Shuffle(gameCards);

        foreach (var card in gameCards) //move shuffled cards offscreen
            card.transform.position = turnHandler.cardDeckPos.position;
    }

    public void Shuffle(List<GameObject> gos)
    {
        for (int i = 0; i < gos.Count; i++)
        {
            GameObject temp = gos[i];
            int randomIndex = UnityEngine.Random.Range(i, gos.Count);
            gos[i] = gos[randomIndex];
            gos[randomIndex] = temp;
        }
    }
}
