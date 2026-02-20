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
    [SerializeField] private int currentLevelIndex = 0;
    [SerializeField] private int matches = 0;
    [SerializeField] private int turns = 0;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int totalPairs = 0;
    [SerializeField] private int totalScore = 0;

    [SerializeField] private bool isGameCompleted = false;

    /// <summary>
    /// Others
    /// </summary>
    [SerializeField] private CardLayoutProperty cardLayoutProperty;

    public event Action<int> GenerateGrid;
    public event Action<bool> OnLevelCompleted;
    public event Action<int, int> OnScoreUpdated;
    public event Action<int, int> OnSaveData;

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
        Init();
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
        UIManager.Instance.OnMainMenuClicked += ResetLevelData;
    }

    public void OnNewGameStart()
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
        totalPairs = totalGridCards / 2;
        DebugManager.Instance.Log(totalGridCards.ToString());

        matches = 0;
        turns = 0;
        currentScore = 0;

        //GenerateGrid?.Invoke(totalGridCards);
        GenerateGrid?.Invoke(totalPairs);
    }

    public void IncrementTurns()
    {
        turns++;
        UIManager.Instance.UpdateTurnsUI(turns.ToString());
    }

    public void IncrementMatches()
    {
        matches++;        
        //currentScore += 10;
        totalScore += 10;
        //UIManager.Instance.UpdateMatchesUI(matches.ToString(), currentScore.ToString());

        OnScoreUpdated?.Invoke(matches, totalScore);

        if(totalPairs == matches)
        {
            //Level completed
            DebugManager.Instance.Log("Level Completed --- GameManager");
            OnLevelCompeted();
            
        }
    }

    private void OnLevelCompeted()
    {
        //Update the score

        //Update the LevelIndex
        if(currentLevelIndex < cardLayoutProperty.LayoutList.Length-1)
        {
            currentLevelIndex++;            
        }
        else
        {
            isGameCompleted = true;
        }

        OnLevelCompleted?.Invoke(isGameCompleted);
        OnSaveData?.Invoke(currentLevelIndex, totalScore);
    }

    public void ResetLevelData()
    {
        DebugManager.Instance.Log("--- Resetting the Game Values : GameManager ---");

        turns = 0;
        matches = 0;
        totalPairs = 0;
        currentLevelIndex = 0;
        totalScore = 0;
        isGameCompleted = false;
        
        //Reset all the values - Turns, matches, currentscore
    }

    private void OnApplicationQuit()
    {
        //Save the Level

        UIManager.Instance.onNewGame -= OnNewGameStart;
        UIManager.Instance.OnHomeClick -= ResetLevelData;
    }
}
