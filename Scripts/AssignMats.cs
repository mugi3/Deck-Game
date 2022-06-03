using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class AssignMats : MonoBehaviour
{
    [Header("Lists")]
    public List<GameObject> playingCards;
    public List<GameObject> shownCards;
    public List<string> removableCards;
    public List<Material> cardMats;
    public List<string> cardsOptions;
    public List<GameObject> availableCards;

    [Space(3f)]
    [Header("Gameobjects")]
    public GameObject cardPrefab;
    public GameObject cardholder;
    object[] matArray;

    [Space(3f)]
    [Header("UI Elements")]
    public TMP_Dropdown add_DropDown;
    public TMP_Dropdown remove_dropDown;
    public CanvasGroup buttonsPanel;
    public TMP_Text messageDisplay;
    public GameObject messageCanvas;

    private float offset = 0.1f;
    private bool executeOperation = false;

    private void Start()
    {
   
        AssignCardsAndMats();
       // CardRemoved(remove_dropDown);
        remove_dropDown.onValueChanged.AddListener(delegate { CardRemoved(remove_dropDown); });
       // CardAdded(add_DropDown);
        add_DropDown.onValueChanged.AddListener(delegate { CardAdded(add_DropDown); });

      
        

    }

    void AssignCardsAndMats()
    {

        cardsOptions.Add(null);
        matArray = Resources.LoadAll("Materials", typeof(Material));

        foreach (Object obj in matArray)
        {
            cardMats.Add(obj as Material);
        }


        GameObject currentCard;
        float offsetNew = 0;
        foreach (Material mat in cardMats)
        {
            currentCard = Instantiate(cardPrefab, new Vector3(0, 0, transform.position.z + offsetNew), transform.rotation);

            currentCard.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = mat;
            currentCard.name = mat.name;
            playingCards.Add(currentCard);
            currentCard.transform.parent = cardholder.transform;
            offsetNew += 0.02f;
        }

        removableCards.Add(null);
        foreach (GameObject item in playingCards)
        {
            removableCards.Add(item.name);
        }
        UpdateRemoveOptions();
    }

    public void ShuffleCards()
    {
        playingCards.Shuffle();
    }
    
    public void CardAdded(TMP_Dropdown dropD)
    {
     
        if (availableCards.Any())
        {
            if (shownCards.Any())
            {
                foreach (GameObject cardItem in shownCards)
                {
                    if(availableCards[dropD.value-1] == cardItem)                                   
                    {
                        availableCards[dropD.value - 1].GetComponent<Animator>().Play("addToDeck");
                        messageCanvas.GetComponent<Animator>().Play("fadeCanvas");
                        messageDisplay.text = "Card added to deck";
                        shownCards.Remove(availableCards[dropD.value - 1]);
                        break;
                    }
                    else
                    {
                        availableCards[dropD.value - 1].GetComponent<Animator>().Play("addToDeckFromRemoved");
                        messageCanvas.GetComponent<Animator>().Play("fadeCanvas");
                        messageDisplay.text = "Card added to deck";
                    }
                    
                }
            }
           else
            {
                availableCards[dropD.value - 1].GetComponent<Animator>().Play("addToDeckFromRemoved");
                messageCanvas.GetComponent<Animator>().Play("fadeCanvas");
                messageDisplay.text = "Card added to deck";
            }
            buttonsPanel.interactable = false;
            StartCoroutine(ReActivateFunctionsOnAdd());
            playingCards.Add(availableCards[dropD.value-1]);
            removableCards.Add(availableCards[dropD.value-1].name);

            availableCards.Remove(availableCards[dropD.value-1]);
            cardsOptions.Remove(cardsOptions[dropD.value]);
            Debug.Log("Card added");
            UpdateRemoveOptions();
            UpdateCardOptions();
        }

    }

    public void CardRemoved(TMP_Dropdown dropD)
    {
    
        if (dropD.value > 0)
        {

            playingCards[dropD.value - 1].GetComponent<Animator>().Play("removeFromDeck");
            messageCanvas.GetComponent<Animator>().Play("fadeCanvas");
            messageDisplay.text = "Card removed from deck";
            buttonsPanel.interactable = false;
            StartCoroutine(ReActivateFunctions());
            cardsOptions.Add(playingCards[dropD.value - 1].name);
            availableCards.Add(playingCards[dropD.value - 1]);
            playingCards.Remove(playingCards[dropD.value - 1]);
            removableCards.Remove(removableCards[dropD.value]);
            
            UpdateCardOptions();
            UpdateRemoveOptions();
        }

    }

    public void ShowCard()
    {
        buttonsPanel.interactable = false;
        StartCoroutine(ReActivateFunctions());
        GameObject tempCard = playingCards.Last();
        shownCards.Add(tempCard);
        availableCards.Add(tempCard);
        cardsOptions.Add(shownCards.Last().name);
        removableCards.Remove(shownCards.Last().name);
        UpdateCardOptions();
        UpdateRemoveOptions();

        playingCards.Last().GetComponent<Animator>().Play("Flip");
        messageCanvas.GetComponent<Animator>().Play("fadeCanvas");
        messageDisplay.text = "Card shown from deck";
        playingCards.Remove(playingCards.Last());
    }

    void UpdateCardOptions()
    {
       
        add_DropDown.ClearOptions();
        add_DropDown.AddOptions(cardsOptions);
        
        
    }

    void UpdateRemoveOptions()
    {
        remove_dropDown.ClearOptions();
        remove_dropDown.AddOptions(removableCards);
    }


    IEnumerator ReActivateFunctions()
    {
        
        yield return new WaitForSeconds(0.7f);
        if (availableCards.Any())
        {
            availableCards.Last().transform.position = new Vector3(availableCards.Last().transform.position.x,
                                                  availableCards.Last().transform.position.y,
                                                  0 + offset);
            offset -= 0.01f;
            Debug.Log("moved the card");
        }

        if (!availableCards.Any())
        {
            foreach(GameObject obj in playingCards)
            {
                obj.transform.position = new Vector3(0, transform.position.y, transform.position.z);
            }
        }

        buttonsPanel.interactable = true;

    
    }
    IEnumerator ReActivateFunctionsOnAdd()
    {
        yield return new WaitForSeconds(0.4f);
        offset += 0.01f;
        if (availableCards.Any())
        {
            availableCards.Last().transform.position = new Vector3(0,
                                                  availableCards.Last().transform.position.y,
                                                  0);
            offset -= 0.01f;
            Debug.Log("moved the card");
        }
        if (!availableCards.Any())
        {
            foreach (GameObject obj in playingCards)
            {
                obj.transform.position = new Vector3(0, transform.position.y, transform.position.z);
            }
        }
        buttonsPanel.interactable = true;
    }
}
