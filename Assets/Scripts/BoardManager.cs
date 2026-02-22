using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [SerializeField] private Queue<CardProperty> selectedCards = new Queue<CardProperty>();

    [SerializeField] private List<CardProperty> selectedCardsList = new List<CardProperty>();

    [SerializeField] private Transform gridParent;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private bool isCheckingMatch = false;

    private CardProperty firstSelectedCard;
    private CardProperty secondSelectedCard;

    public event Action<string> PlaySfxAudio;
    

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        Instance = this;

        BindEvents();
    }

    private void BindEvents()
    {
        GameManager.Instance.GenerateGrid += InitializeBoardLayout;
        UIManager.Instance.OnHomeClick += ClearCards;
        CardProperty.OnCardFlipped += OnCardSelected;
    }

    private void InitializeBoardLayout(int totalcards)
    {
        gridParent.GetComponent<GridLayoutGroup>().enabled = true;
        gridParent.GetComponent<ContentSizeFitter>().enabled = true;

        //totalcards = 20;
        DebugManager.Instance.Log($"Started Generating the Grid --- Total Grid Elements {totalcards}");

        //int paircount = totalcards / 2;

        //Create the layout
        //for(int i = 0; i < paircount; i++)
        for(int i = 0; i < totalcards; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                GameObject go = Instantiate(cardPrefab, gridParent, false);
                go.name = i.ToString();

                //Set the card Value
                go.GetComponent<CardProperty>().CardValue = i;
                //go.GetComponent<CardProperty>().valueText.text = i.ToString();
            }
        }

        ShuffleGrid();
    }

    private void ShuffleGrid()
    {        
        DebugManager.Instance.Log("--- Shuffling the board : BoardManager --- ");

        int count = gridParent.childCount;

        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(i, count);

            // Swap sibling order
            gridParent.GetChild(i).SetSiblingIndex(rand);
        }

        //ShowPreview?.Invoke();
        StartCoroutine(PreviewGrid());
        DebugManager.Instance.Log("Previewing the grid : Boradmanager");
    }

    private void ClearCards()
    {
        if (gridParent.childCount > 0)
        {
            DebugManager.Instance.Log("--- Cleaning all cards : BoardManager --- ");

            //Destroy All Cards
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void OnCardSelected(CardProperty cdproperty)
    {
        PlaySfxAudio?.Invoke("cardflipped");

        DebugManager.Instance.Log($"---Checking match --- : Boradmanager | {cdproperty.CardValue}");
        selectedCards.Enqueue(cdproperty);

        if (selectedCards.Count % 2 == 0 && !isCheckingMatch)
        {
            StartCoroutine(CheckForMatch());
        }
    }

    private IEnumerator CheckForMatch()
    {
        isCheckingMatch = true;
        //yield return new WaitForSeconds(1f);
        DebugManager.Instance.Log($"---Checking match --- : Boradmanager");

        while (selectedCards.Count >= 2)
        {
            GameManager.Instance.IncrementTurns();

            firstSelectedCard = selectedCards.Dequeue();
            secondSelectedCard = selectedCards.Dequeue();

            yield return new WaitForSeconds(0.5f);  // Delay for the user to see the flipped cards
            

            if (firstSelectedCard.CardValue == secondSelectedCard.CardValue)
            {
                PlaySfxAudio?.Invoke("correctmatch");

                firstSelectedCard.OnCardMatched();
                secondSelectedCard.OnCardMatched();
                
                yield return new WaitForSeconds(0.25f);

                GameManager.Instance.IncrementMatches();
                //SoundManager.Instance.PlaySound(SoundType.Match);

            }
            else
            {
                Debug.Log("control is in the else" + firstSelectedCard + " : " + secondSelectedCard);
                firstSelectedCard.FlipBack();
                secondSelectedCard.FlipBack();

                PlaySfxAudio?.Invoke("incorrectmatch");
                //SoundManager.Instance.PlaySound(SoundType.Mismatch);
            }

            firstSelectedCard = null;
            secondSelectedCard = null;

        }

        isCheckingMatch = false;
    }

    private IEnumerator PreviewGrid()
    {
        yield return new WaitForSeconds(1f);

        int count = gridParent.childCount;

        //Flip
        for (int i = 0; i < count; i++)
        {
            gridParent.GetChild(i).GetComponent<Animator>().SetTrigger("flip");
        }

        yield return new WaitForSeconds(1f);

        //Flip Back
        for (int i = 0; i < count; i++)
        {
            gridParent.GetChild(i).GetComponent<Animator>().SetTrigger("flipback");
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < count; i++)
        {
            gridParent.GetChild(i).GetComponent<Button>().interactable = true;
        }

        gridParent.GetComponent<GridLayoutGroup>().enabled = false;
        gridParent.GetComponent<ContentSizeFitter>().enabled = false;
    }

    private void Reset()
    {

    }

    private void OnApplicationQuit()
    {
        GameManager.Instance.GenerateGrid -= InitializeBoardLayout;
        UIManager.Instance.OnHomeClick -= ClearCards;
        CardProperty.OnCardFlipped -= OnCardSelected;
    }

}
