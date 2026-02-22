using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    /// <summary>
    /// ---------- Gameobject ----------
    /// </summary>
    [Header("GAMEOBJECTS")]
    [Space(5)]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameSceen;
    [SerializeField] private GameObject levelCompletedUI;
    [SerializeField] private GameObject GameCompletedUI;

    /// <summary>
    /// ---------- Buttons ----------
    /// </summary>
    [Header("BUTTONS")]
    [Space(5)]
    [SerializeField] private Button newGame_Button;
    [SerializeField] private Button continueGame_Button;
    [SerializeField] private Button exitGame_Button;
    [SerializeField] private Button home_Button;
    [SerializeField] private Button resetData_Button;

    /// <summary>
    /// ---------- TM_Pro Texts ----------
    /// </summary>
    [Header("TEXTS")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI turnsText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteUIScoreText;
    [SerializeField] private TextMeshProUGUI gameCompleteUIScoreText;

    /// <summary>
    /// ---------- INTEGERS ----------
    /// </summary>
    private int totalScore = 0;

    /// <summary>
    /// ---------- Events ----------
    /// </summary>
    public event Action onNewGame;
    public event Action OnHomeClick;
    public event Action OnMainMenuClicked;
    public event Action OnResetDataClicked;

    private void Awake()
    {
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
        //Init();
        Invoke("Init", 1f);
              
    }

    private void Init()
    {
        continueGame_Button.onClick.AddListener(Continue);
        newGame_Button.onClick.AddListener(NewGame);
        exitGame_Button.onClick.AddListener(ExitGame);
        home_Button.onClick.AddListener(OnHome);

        if (GameManager.Instance.lastSavedLevel > 0)
        {
            continueGame_Button.gameObject.SetActive(true);
        }
        else
            continueGame_Button.gameObject.SetActive(false);

        //DebugManager.Instance.Log("Load data : " + GameManager.Instance.highScore.ToString());
        ShowHighScore();
        resetData_Button.onClick.AddListener(ResetSavedData);

        BindEvents();
    }

    //private void Load

    private void BindEvents()
    {
        GameManager.Instance.OnLevelCompleted += HandleLevelCompleted;
        GameManager.Instance.OnScoreUpdated += UpdateMatchesUI;
        GameManager.Instance.OnGameCompleteStarted += DisableHome;
        GameManager.Instance.OnGameStart += ResetData;
    }

    private void DisableHome()
    {
        home_Button.interactable = false;
    }

    public void EnableHome()
    {
        home_Button.interactable = true;
    }

    public void ShowHighScore()
    {
        highScoreText.text = "Highscore : " + GameManager.Instance.HighScore.ToString();
    }

    private void Continue()
    {
        //Play Last saved Game
        if (GameManager.Instance.lastSavedLevel > 0)
            GameManager.Instance.currentLevelIndex = GameManager.Instance.lastSavedLevel;

        NewGame();


        //Load the last game stats

    }

    private void NewGame()
    {
        DebugManager.Instance.Log("Enabling New Game");
        onNewGame?.Invoke();

        //Start New Game
        mainMenu.SetActive(false);
        gameSceen.SetActive(true);
    }    

    public void UpdateTurnsUI(string turns)
    {
        turnsText.text = turns;
    }

    public void UpdateMatchesUI(int matches, int score)
    {
        totalScore = score;

        matchesText.text = matches.ToString();
        UpdateScoreUI(score.ToString());
    }

    public void UpdateScoreUI(string score)
    {
        scoreText.text = score;
    }

    private void HandleLevelCompleted(bool gamestatus)
    {
        home_Button.interactable = false;

        if (gamestatus)
        {
            GameCompletedUI.SetActive(true);
            levelCompletedUI.SetActive(false);

            gameCompleteUIScoreText.text = $"Score : {totalScore.ToString()}";
        }
        else
        {
            levelCompletedUI.SetActive(true);
            GameCompletedUI.SetActive(false);

            levelCompleteUIScoreText.text = $"Score : {totalScore.ToString()}";
            ShowHighScore();
        }

        //StartCoroutine(OnLevelCompleted(gamestatus));
    }    

    private void ExitGame()
    {
        DebugManager.Instance.Log("Exiting the Game!!");

        //Quit Game
        Application.Quit();
    }

    private void OnHome()
    {
        DebugManager.Instance.Log("On Home -- Main Menu will be enabled");
        //Save the last game


        //Load the main menu
        mainMenu.SetActive(true);
        gameSceen.SetActive(false);

        if (GameManager.Instance.lastSavedLevel >= 0)
            continueGame_Button.gameObject.SetActive(true);

        ShowHighScore();
        OnHomeClick?.Invoke();
    }

    public void OnMainMenu(bool isgamecompleted)
    {
        //Save current game

        //Load Main menu
        mainMenu.SetActive(true);
        gameSceen.SetActive(false);

        if (isgamecompleted)
            GameCompletedUI.SetActive(false);
        else
            levelCompletedUI.SetActive(false);

        if (GameManager.Instance.lastSavedLevel >= 0)
            continueGame_Button.gameObject.SetActive(true);

        ShowHighScore();

        ResetData();
        OnMainMenuClicked?.Invoke();
    }

    private void ResetSavedData()
    {
        SaveSystem.Instance.ResetSave();

        if (continueGame_Button.gameObject.activeSelf)
            continueGame_Button.gameObject.SetActive(false);

        highScoreText.text = $"Highscore : {0.ToString()}";

        OnResetDataClicked?.Invoke();
    }

    private void ResetData()
    {
        totalScore = 0;
        matchesText.text = 0.ToString();
        turnsText.text = 0.ToString();
        scoreText.text = 0.ToString();
    }

    private void OnApplicationQuit()
    {
        GameManager.Instance.OnLevelCompleted -= HandleLevelCompleted;
    }
}
