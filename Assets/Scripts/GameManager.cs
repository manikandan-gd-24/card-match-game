using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// INTEGERS ----------
    /// </summary>
    private int currentLevelIndex = 0;
    private int matches = 0;
    private int turns = 0;
    private int currentScore = 0;

    /// <summary>
    /// Others
    /// </summary>
    [SerializeField] private CardLayoutProperty cardLayoutProperty;

    public event Action<int> GenerateGrid;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional (keeps it between scenes)
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Start()
    {
        BindEvents();
        SetDefaultValues();
    }

    private void Init()
    {
        BindEvents();
    }

    private void SetDefaultValues()
    {
        cardLayoutProperty.ValidateLayouts();
    }

    private void BindEvents()
    {
        UIManager.Instance.onNewGame += OnNewGameStart;
        UIManager.Instance.OnHomeClick += ResetLevelData;
    }

    private void OnNewGameStart()
    {
        DebugManager.Instance.Log("Starting a New game --- Gamemanger");

        LoadLevelData();
    }

    private void LoadLevelData()
    {
        //Check if current level is valid || if not return
        if (currentLevelIndex >= cardLayoutProperty.LayoutList.Length)
        {
            DebugManager.Instance.Log($"The current Level index is greater that the levels | " +
                $"currentLevelIndex : { currentLevelIndex} | TotalLevels : {cardLayoutProperty.LayoutList.Length}");
            return;
        }

        var layout = cardLayoutProperty.LayoutList[currentLevelIndex];

        int totalGridCards = layout.Rows * layout.Columns;
        DebugManager.Instance.Log(totalGridCards.ToString());

        matches = 0;
        turns = 0;
        currentScore = 0;

        GenerateGrid?.Invoke(totalGridCards);
    }

    private void OnLevelCompeted()
    {
        //Update the score

        //Update the LevelIndex
        if(currentLevelIndex <= cardLayoutProperty.LayoutList.Length)
        {
            currentLevelIndex++;
        }

    }

    private void ResetLevelData()
    {
        DebugManager.Instance.Log("--- Resetting the Game Values : GameManager ---");

        //Reset all the values - Turns, matches, currentscore
    }

    private void OnApplicationQuit()
    {
        //Save the Level
    }
}
