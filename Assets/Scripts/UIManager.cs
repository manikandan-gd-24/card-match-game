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
    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    [SerializeField] private Button exitGame;
    [SerializeField] private Button home;

    /// <summary>
    /// ---------- TM_Pro Texts ----------
    /// </summary>
    [Header("TEXTS")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI turnsText;
    [SerializeField] private TextMeshProUGUI scoreText;
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

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        BindEvents();
    }

    private void Init()
    {
        Instance = this;

        continueGame.onClick.AddListener(Continue);
        newGame.onClick.AddListener(NewGame);
        exitGame.onClick.AddListener(ExitGame);
        home.onClick.AddListener(OnHome);
    }

    private void BindEvents()
    {
        GameManager.Instance.OnLevelCompleted += HandleLevelCompleted;
        GameManager.Instance.OnScoreUpdated += UpdateMatchesUI;
    }

    private void Continue()
    {
        //Play Last saved Game
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

        ResetData();
        OnMainMenuClicked?.Invoke();
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
