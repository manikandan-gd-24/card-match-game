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
    [SerializeField] private int matches = 0;
    [SerializeField] private int turns = 0;
    [SerializeField] private int totalPairs = 0;
    [SerializeField] private int totalScore = 0;
    
    public int currentLevelIndex = 0;
    public int lastSavedLevel = 0;
    public int HighScore = 0;

    /// <summary>
    /// BOOLEANS ----------
    /// </summary>
    [SerializeField] private bool isGameCompleted = false;

    /// <summary>
    /// Others
    /// </summary>
    [SerializeField] private CardLayoutProperty cardLayoutProperty;
    [SerializeField] private SaveSystem saveSystem;

    /// <summary>
    /// EVENTS ----------
    /// </summary>
    public event Action OnGameCompleteStarted;
    public event Action OnGameStart;

    public event Action<int> GenerateGrid;
    public event Action<bool> OnLevelCompleted;
    public event Action<string> PlaySfxAudio;

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

    private void OnEnable()
    {
        //Init();
    }

    private void Start()
    {
        Init();
        LoadSavedData(saveSystem.Current.level, saveSystem.Current.highScore);

        SetDefaultValues();
    }

    private void Init()
    {
        Invoke("BindEvents", 0.5f);
    }

    /// <summary>
    /// Set to the default values
    /// </summary>
    private void SetDefaultValues()
    {
        cardLayoutProperty.ValidateLayouts();
        SetGameData(saveSystem.Current.level, saveSystem.Current.highScore);
    }

    /// <summary>
    /// Subscribe to the events
    /// </summary>
    private void BindEvents()
    {
        UIManager.Instance.onNewGame += OnNewGameStart;
        UIManager.Instance.OnHomeClick += ResetLevelData;
        UIManager.Instance.OnHomeClick += OnHomeClicked;
        UIManager.Instance.OnMainMenuClicked += ResetLevelData;
        UIManager.Instance.OnResetDataClicked += ResetGameData;

        SaveSystem.Instance.onDataSaved += LoadSavedData;        
        SaveSystem.Instance.onDataLoaded += LoadSavedData;        
    }

    private void SetGameData(int lastlevel, int highscore)
    {
        lastSavedLevel = lastlevel;
        HighScore = highscore;
    }

    public void OnNewGameStart()
    {
        DebugManager.Instance.Log("Starting a New game --- Gamemanger");

        OnGameStart?.Invoke();
        totalScore = 0;
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
        //BoardManager.Instance.
        OnScoreUpdated?.Invoke(matches, totalScore);

        if(totalPairs == matches)
        {
            OnGameCompleteStarted?.Invoke();
            //Level completed
            DebugManager.Instance.Log("Level Completed --- GameManager");
            StartCoroutine(OnLevelCompeted());            
        }
    }

    private IEnumerator OnLevelCompeted()
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

        yield return new WaitForSeconds(1f);
        PlaySfxAudio?.Invoke("lvlcompleted");

        OnLevelCompleted?.Invoke(isGameCompleted);
        OnSaveData?.Invoke(lastSavedLevel = currentLevelIndex, totalScore);

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

    private void ResetGameData()
    {
        lastSavedLevel = 0;
        HighScore = 0;
    }

    private void OnHomeClicked()
    {
        //OnSaveData?.Invoke(currentLevelIndex, totalScore);
    }

    private void LoadSavedData(int level, int lasthighscore)
    {
        if (level < cardLayoutProperty.LayoutList.Length)
            lastSavedLevel = level;
        else
            lastSavedLevel = 0;

        HighScore = lasthighscore;

        DebugManager.Instance.Log("highscore : " + lasthighscore);
        DebugManager.Instance.Log("LastLevel : " + level);
        //UIManager.Instance.ShowHighScore();
    }

    private void OnApplicationQuit()
    {
        //Save the Level

        UIManager.Instance.onNewGame -= OnNewGameStart;
        UIManager.Instance.OnHomeClick -= ResetLevelData;
    }
}
