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
        BindEvents();

        //Set card value
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);

        valueText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        valueText.text = CardValue.ToString();
    }   
    
    private void BindEvents()
    {
        
    }

    private void OnCardClicked()
    {
        //Flip the card
        DebugManager.Instance.Log($"--- Flipping the card ---{CardValue}");
        animator.SetTrigger("flip");

        //Broadcast on Card Flipped
        OnCardFlipped?.Invoke(GetComponent<CardProperty>());
    }
}
