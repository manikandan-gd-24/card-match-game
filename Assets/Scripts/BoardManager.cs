using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    private Queue<CardProperty> selectedCards = new Queue<CardProperty>();

    [SerializeField] private Transform gridParent;

    [SerializeField] private GameObject cardPrefab;
   

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
        CardProperty.OnCardFlipped += CheckMatch;
    }

    private void InitializeBoardLayout(int totalcards)
    {
        //totalcards = 20;
        DebugManager.Instance.Log($"Started Generating the Grid --- Total Grid Elements {totalcards}");

        int paircount = totalcards / 2;

        //Create the layout
        for(int i = 0; i < paircount; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                GameObject go = Instantiate(cardPrefab, gridParent, false);
                go.name = i.ToString();

                //Set the card Value
                go.GetComponent<CardProperty>().CardValue = i;
                //go.GetComponent<CardProperty>().valueText.text = j.ToString();
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
        DebugManager.Instance.Log("--- Cleaning all cards : BoardManager --- ");

        //Destroy All Cards
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CheckMatch(CardProperty cdprperty)
    {
        DebugManager.Instance.Log($"---Checking match --- : Boradmanager | {cdprperty.CardValue}");
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
    }
    
}
