using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TurnHandler : MonoBehaviour
{
    public TextMeshProUGUI winText;

    public TextMeshProUGUI turnIndicatorText;

    public GameObject[] UNOTexts;
    public BaysProb probabilty = new BaysProb();

    public Button[] colorButtons;
    public Toggle cardHideToggle;

    public GameObject winScreen;
    [SerializeField] Sprite cardBackSprite;
    [SerializeField] Sprite cardBGSprite;

    public Transform discardPilePos;
    public Transform cardDeckPos;

    public List<Player> playerList = new List<Player>();

    public List<GameObject> discardPile = new List<GameObject>();

    CardSpawner cardSpawner;

    bool awaitPlayColorChoice = false;
    int activePlayer = 0;

    private static TurnHandler _instance;

    public static TurnHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TurnHandler>();
            }

            return _instance;
        }
    }

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

        //insertThisLogic
        //playerList[activePlayer].playerCards[index]
        //

        cardSpawner.GetComponent<PlayerCardsDisplay>().DisplayPlayerCards();

        //put last card in deck on discard pile
        int tempIndex = cardSpawner.gameCards.Count - 1;
        ToDiscardPile(cardSpawner.gameCards[tempIndex]);
        cardSpawner.gameCards.RemoveAt(tempIndex);

        if (discardPile[0].GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.action)
        {
            if (discardPile[0].GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.draw2)
                DrawX(2);

            SwitchActivePlayer();
        }
        else if (discardPile[0].GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.wild)
        {
            if (discardPile[0].GetComponent<CardValueSaver>().wildType == CardValueSaver.WildType.draw4)
                DrawX(4);

            StartCoroutine(PickWildcardColor(discardPile[0]));
        }

        probabilty.makeInitalGuess(playerList[1].playerCards, discardPile[0].gameObject);

        winScreen.SetActive(false);

        //begin first turn for player 0
        SetActivePlayer(0);

        ToggleAICardDisplay(); //call at start so toogle can be activaed at game start
    }

    void SetActivePlayer(int index)
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
                    card.GetComponent<Button>().interactable = false;
                foreach (var card in playerList[1].playerCards)
                    card.GetComponent<Button>().interactable = true;

                activePlayer = 1;
                break;
            default:
                break;
        }

        turnIndicatorText.text = "Player " + (activePlayer + 1) + "'s turn";
    }

    void SwitchActivePlayer()
    {
        if (awaitPlayColorChoice) { return; }

        hasPickedCard = false;

        switch (activePlayer)
        {
            case 0:
                SetActivePlayer(1);
                AiTurn();
                break;
            case 1:
                SetActivePlayer(0);
                break;
            default:
                break;
        }
    }


    private void AiTurn()
    {

        if (playerList[1].activeTurn == true)
        {
            //probabilty.showEntireArrayStructure();

            GameObject playCard = probabilty.takeAturn(playerList[activePlayer].playerCards, discardPile[discardPile.Count - 1], playerList[0].playerCards.Count);
            probabilty.EnemyCardTotal = playerList[0].playerCards.Count;
            probabilty.AICardTotal = playerList[1].playerCards.Count;

            if (playCard != null)
            {
                PlayCard(playCard);
                if (playCard.GetComponent<CardValueSaver>().cardType.Equals(CardValueSaver.CardType.action) || playCard.GetComponent<CardValueSaver>().cardType.Equals(CardValueSaver.CardType.wild))//since all actions gives you another turn...
                {
                    AiTurn();
                }
            }
            else
            {
                PickCard();
                probabilty.addACardToAIDeck(playerList[activePlayer].playerCards[playerList[activePlayer].playerCards.Count - 1], false, false);
                AiTurn();
            }
            //logic to decide card
            //PlayCard(playedCard);
        }

    }

    //On card button
    public void PlayCard(GameObject playedCard)
    {
        if (awaitPlayColorChoice) { return; }

        //find card array index in player array
        int currentIndex = 0;
        for (int i = 0; i < playerList[activePlayer].playerCards.Count; i++)
        {
            if (playerList[activePlayer].playerCards[i] == playedCard)
            {
                currentIndex = i;
            }
        }

        (bool playable, bool reverse, bool skip) = CheckPlayable(currentIndex, true);

        if (playable)
        {
            //Did AI assume correct card
            if (activePlayer == 0)
            {
                probabilty.EnemyPlayed = 1;
                probabilty.addACardToAIDeck(playedCard, true, false);
            }

            RevealAICard(playedCard); //reveal card in case its hidden

            hasPickedCard = false;

            ToDiscardPile(playedCard);
            playerList[activePlayer].playerCards.RemoveAt(currentIndex); //remove card from player

            if (reverse) //for 2 players reverse just skips
                skip = true;

            CheckForUNO();
            if (!CheckForWin())
            {
                if (!skip)
                {
                    //Give turn to other player (and disable the other players cards)
                    SwitchActivePlayer();
                }
                else
                    print("Skip");
            }
            else //if activeplayer won
            {
                playerList[0].activeTurn = false;
                playerList[1].activeTurn = false;

                //disable player 1s cards
                foreach (var card in playerList[0].playerCards)
                    card.GetComponent<Button>().interactable = false;

                foreach (var card in playerList[1].playerCards)
                    card.GetComponent<Button>().interactable = false;

            }

        }
        else
            print("Card not playable");
    }

    private void ToDiscardPile(GameObject card)
    {
        discardPile.Add(card);
        card.transform.position = discardPilePos.position;
        card.transform.SetParent(discardPilePos);
        card.transform.SetAsLastSibling();
    }

    private void CheckForUNO()
    {
        if (playerList[activePlayer].playerCards.Count == 1)
            UNOTexts[activePlayer].SetActive(true);
        else
            UNOTexts[activePlayer].SetActive(false);
    }

    private bool CheckForWin()
    {
        if (playerList[activePlayer].playerCards.Count == 0)
        {
            if (activePlayer == 0)
            {
                probabilty.SaveOutcomeForNeural(1,0);
            }
            else
            {
                probabilty.SaveOutcomeForNeural(0,1);
            }

            winText.gameObject.SetActive(true);
            winText.text = "Player " + (activePlayer + 1) + " wins!";

            winScreen.SetActive(true);

            return true;
        }

        return false;
    }

    bool hasPickedCard = false;
    public void PickCard()
    {
        if (!hasPickedCard)
        {
            if (playerList[0].activeTurn == true)//AI wants to guess what card it was then
            {
                probabilty.enemyDrewCardInsteadOfPlay(discardPile[discardPile.Count - 1]);
            }
            if (cardSpawner.gameCards.Count >= 1)
            {
                playerList[activePlayer].playerCards.Add(cardSpawner.gameCards[cardSpawner.gameCards.Count - 1]); //give active player last card on deck
                cardSpawner.gameCards.RemoveAt(cardSpawner.gameCards.Count - 1);
                cardSpawner.GetComponent<PlayerCardsDisplay>().DisplayPlayerCards();

                hasPickedCard = true;


                ToggleAICardDisplay();
                CheckForUNO();
            }
            else if (discardPile.Count > 1)
            {
                for (int i = 0; i < discardPile.Count - 1; i++) //remove all cards from discard pile except at last position and add them to deck
                {
                    cardSpawner.gameCards.Add(discardPile[i].gameObject);
                    discardPile.RemoveAt(i);
                }

                cardSpawner.Shuffle(cardSpawner.gameCards);

                foreach (var card in cardSpawner.gameCards) //move shuffled cards offscreen
                    card.transform.position = cardDeckPos.position;

                //Tell the AI cards where shuffled
                probabilty.shuffleCardsUpdate(playerList[1].playerCards);

                PickCard(); //try to pick a card again
            }
            else
            {
                print("No cards left to take");
            }
        }

        (bool playable, bool reverse, bool skip) = CheckPlayable(playerList[activePlayer].playerCards.Count - 1, false);

        if (!playable) //only skip if not playable
        {
            SwitchActivePlayer();

            hasPickedCard = false;
        }
    }

    private void DrawX(int count)
    {
        if (cardSpawner.gameCards.Count >= count)
        {
            for (int i = 0; i < count; i++)
            {
                switch (activePlayer)
                {
                    case 0:
                        playerList[1].playerCards.Add(cardSpawner.gameCards[cardSpawner.gameCards.Count - 1]); //give active player last card on deck
                        break;
                    case 1:
                        playerList[0].playerCards.Add(cardSpawner.gameCards[cardSpawner.gameCards.Count - 1]); //give active player last card on deck
                        probabilty.predictACard(cardSpawner.gameCards.Count);//let the AI guess what card human got
                        break;
                }

                cardSpawner.gameCards.RemoveAt(cardSpawner.gameCards.Count - 1);
            }
            cardSpawner.GetComponent<PlayerCardsDisplay>().DisplayPlayerCards();
        }
        else if (discardPile.Count > 1)
        {
            for (int i = 0; i < discardPile.Count - 1; i++) //remove all cards from discard pile except at last position and add them to deck
            {
                cardSpawner.gameCards.Add(discardPile[i].gameObject);
                discardPile.RemoveAt(i);
            }

            cardSpawner.Shuffle(cardSpawner.gameCards);

            //Tell the AI cards where shuffled
            probabilty.shuffleCardsUpdate(playerList[1].playerCards);

            foreach (var card in cardSpawner.gameCards) //move shuffled cards offscreen
                card.transform.position = cardDeckPos.position;

            DrawX(count); //try drawing cards again
        }
        else
        {
            print("Not enough cards left to take");
        }

        ToggleAICardDisplay();
    }

    private (bool playable, bool reverse, bool skip) CheckPlayable(int index, bool executeActions)
    {
        GameObject discardTemp = discardPile[discardPile.Count - 1];

        GameObject card = playerList[activePlayer].playerCards[index];

        //if card is number card
        if (card.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.number)
        {
            int cardNumber = card.GetComponent<CardValueSaver>().cardNumber;

            //if discardpile contains numbercard
            if (discardTemp.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.number)
            {
                //compare cardnumber and color
                if (cardNumber == discardTemp.GetComponent<CardValueSaver>().cardNumber || card.GetComponent<CardValueSaver>().color == discardTemp.GetComponent<CardValueSaver>().color)
                {
                    return (true, false, false);
                }
            }
            //if discardpile contains actioncard
            else if (discardTemp.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.action)
            {
                //compare color
                if (card.GetComponent<CardValueSaver>().color == discardTemp.GetComponent<CardValueSaver>().color)
                {
                    return (true, false, false);
                }
            }
            //if discardpile contains wildcard
            else if (discardTemp.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.wild)
            {
                //compare color
                if (card.GetComponent<CardValueSaver>().color == discardTemp.GetComponent<CardValueSaver>().color)
                {
                    return (true, false, false);
                }
            }
        }
        //if card is action card
        else if (card.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.action)
        {
            //if discardpile contains numbercard
            if (discardTemp.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.number)
            {
                //compare color
                if (card.GetComponent<CardValueSaver>().color == discardTemp.GetComponent<CardValueSaver>().color)
                {
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.reverse)
                        return (true, true, false);
                    else if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.skip)
                        return (true, false, true);
                    else if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.draw2)
                    {
                        if (executeActions)
                            DrawX(2);

                        return (true, false, true);
                    }
                }
            }
            //if discardpile contains actioncard
            if (discardTemp.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.action)
            {
                //compare color
                if (card.GetComponent<CardValueSaver>().color == discardTemp.GetComponent<CardValueSaver>().color)
                {
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.reverse)
                        return (true, true, false);
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.skip)
                        return (true, false, true);
                    else if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.draw2)
                    {
                        if (executeActions)
                            DrawX(2);

                        return (true, false, true);
                    }
                }
                //compare action
                if (card.GetComponent<CardValueSaver>().actionType == discardTemp.GetComponent<CardValueSaver>().actionType)
                {
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.reverse)
                        return (true, true, false);
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.skip)
                        return (true, false, true);
                    else if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.draw2)
                    {
                        if (executeActions)
                            DrawX(2);

                        return (true, false, true);
                    }
                }
            }
            //if discardpile contains wildcard
            else if (discardTemp.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.wild)
            {
                //compare color
                if (card.GetComponent<CardValueSaver>().color == discardTemp.GetComponent<CardValueSaver>().color)
                {
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.reverse)
                        return (true, true, false);
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.skip)
                        return (true, false, true);
                    if (card.GetComponent<CardValueSaver>().actionType == CardValueSaver.ActionType.draw2)
                    {
                        if (executeActions)
                            DrawX(2);

                        return (true, false, true);
                    }
                }
            }
        }
        //if card is wild card
        else if (card.GetComponent<CardValueSaver>().cardType == CardValueSaver.CardType.wild)
        {
            if (executeActions)
                if (activePlayer == 0)
                {
                    StartCoroutine(PickWildcardColor(card));
                }

            if (card.GetComponent<CardValueSaver>().wildType == CardValueSaver.WildType.draw4)
            {
                if (executeActions)
                    DrawX(4);

                return (true, false, true);
            }
            else if (card.GetComponent<CardValueSaver>().wildType == CardValueSaver.WildType.colorChange)
                return (true, false, false);

        }
        return (false, false, false);
    }

    int colorID;

    public void SetColorID(int id) //on button
    {
        colorID = id;
    }

    private IEnumerator PickWildcardColor(GameObject card)
    {
        awaitPlayColorChoice = true;

        //show color pick buttons 
        foreach (var button in colorButtons)
        {
            button.gameObject.SetActive(true);
        }

        var waitForButton = new WaitForUIButtons(colorButtons);
        yield return waitForButton.Reset();

        //colorID = 0;

        if (waitForButton.PressedButton == colorButtons[0] || waitForButton.PressedButton == colorButtons[1] || waitForButton.PressedButton == colorButtons[2] || waitForButton.PressedButton == colorButtons[3])
        {
            //hide color pick buttons
            foreach (var button in colorButtons)
            {
                button.gameObject.SetActive(false);
            }

            SetWildCardColor(card, colorID);

            awaitPlayColorChoice = false;

            if (card.GetComponent<CardValueSaver>().wildType.Equals(CardValueSaver.WildType.colorChange))
                SwitchActivePlayer();
        }
    }

    public void SetWildCardColor(GameObject card, int colorID)
    {
        switch (colorID)
        {
            case 1: //red
                card.GetComponent<CardValueSaver>().color = CardValueSaver.Color.red;
                card.GetComponent<Image>().color = cardSpawner.redColor;
                break;
            case 2: //green
                card.GetComponent<CardValueSaver>().color = CardValueSaver.Color.green;
                card.GetComponent<Image>().color = cardSpawner.greenColor;
                break;
            case 3: //blue
                card.GetComponent<CardValueSaver>().color = CardValueSaver.Color.blue;
                card.GetComponent<Image>().color = cardSpawner.blueColor;
                break;
            case 4: //yellow
                card.GetComponent<CardValueSaver>().color = CardValueSaver.Color.yellow;
                card.GetComponent<Image>().color = cardSpawner.yellowColor;
                break;
        }
    }

    public void ToggleAICardDisplay() //on UI Toggle
    {
        foreach (GameObject card in playerList[1].playerCards)
        {
            if (cardHideToggle.isOn) //if card value is supposed to be hidden
            {
                card.GetComponent<Image>().sprite = cardBackSprite;

                card.GetComponent<Image>().color = Color.white;

                card.transform.GetChild(0).gameObject.SetActive(false); //deactivate action & wild card sprite

                foreach (var text in card.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    text.enabled = false; //deactivate text in case of numbercard
                }
            }
            else //if card value was hidden before
            {
                RevealAICard(card);
            }
        }
    }

    private void RevealAICard(GameObject card)
    {
        switch (card.GetComponent<CardValueSaver>().cardType)
        {
            case CardValueSaver.CardType.number:

                card.GetComponent<Image>().sprite = cardBGSprite;

                switch (card.GetComponent<CardValueSaver>().color) //Set Color Again
                {
                    case CardValueSaver.Color.red:
                        card.GetComponent<Image>().color = cardSpawner.redColor;
                        break;
                    case CardValueSaver.Color.green:
                        card.GetComponent<Image>().color = cardSpawner.greenColor;
                        break;
                    case CardValueSaver.Color.blue:
                        card.GetComponent<Image>().color = cardSpawner.blueColor;
                        break;
                    case CardValueSaver.Color.yellow:
                        card.GetComponent<Image>().color = cardSpawner.yellowColor;
                        break;
                }

                card.transform.GetChild(0).gameObject.SetActive(true);

                foreach (var text in card.GetComponentsInChildren<TextMeshProUGUI>()) //show text again
                {
                    text.enabled = true;
                }
                break;
            case CardValueSaver.CardType.action:

                card.GetComponent<Image>().sprite = cardBGSprite;

                card.transform.GetChild(0).gameObject.SetActive(true);

                switch (card.GetComponent<CardValueSaver>().color) //Set Color Again
                {
                    case CardValueSaver.Color.red:
                        card.GetComponent<Image>().color = cardSpawner.redColor;
                        break;
                    case CardValueSaver.Color.green:
                        card.GetComponent<Image>().color = cardSpawner.greenColor;
                        break;
                    case CardValueSaver.Color.blue:
                        card.GetComponent<Image>().color = cardSpawner.blueColor;
                        break;
                    case CardValueSaver.Color.yellow:
                        card.GetComponent<Image>().color = cardSpawner.yellowColor;
                        break;
                }

                break;
            case CardValueSaver.CardType.wild:
                card.GetComponent<Image>().sprite = cardBGSprite;
                if(card.GetComponent<Image>().color == Color.white)
                    card.GetComponent<Image>().color = cardSpawner.blackColor;

                card.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }
    }
}