using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CardProperty : MonoBehaviour
{
    public int CardValue = 0;

    public bool IsMatched = false;
    public bool IsFlipped = false;

    public TextMeshProUGUI valueText;

    private Animator animator;

    private Button button;

    public static event Action<CardProperty> OnCardFlipped;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //Set card value
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);

        valueText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
       
    }

    private void Start()
    {
        valueText.text = CardValue.ToString();
        DebugManager.Instance.Log($"card value : {CardValue}");
    }


    private void OnCardClicked()
    {
        //Flip the card
        DebugManager.Instance.Log($"--- Flipping the card ---{CardValue}");
        animator.SetTrigger("flip");


        IsFlipped = true;

        //Broadcast on Card Flipped
        OnCardFlipped?.Invoke(GetComponent<CardProperty>());
    }

    public void OnCardMatched()
    {
        IsMatched = true;
        Destroy(gameObject);

        //StartCoroutine(DiableAndDestroyCard());
    }

    private IEnumerator DiableAndDestroyCard()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    public void FlipBack()
    {
        IsFlipped = false;
        animator.SetTrigger("flipback");
    }
}
