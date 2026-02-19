using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    /// <summary>
    /// Gameobject Variables ----------
    /// </summary>
    [Header("Screens")]
    [Space(5)]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameSceen;

    /// <summary>
    /// BUtton Variables ----------
    /// </summary>
    [Header("Buttons")]
    [Space(5)]
    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    [SerializeField] private Button exitGame;
    [SerializeField] private Button home;

    /// <summary>
    /// Events ----------
    /// </summary>
    public event Action onNewGame;
    public event Action OnHomeClick;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Instance = this;

        continueGame.onClick.AddListener(Continue);
        newGame.onClick.AddListener(NewGame);
        exitGame.onClick.AddListener(ExitGame);
        home.onClick.AddListener(OnHome);
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

    private void OnApplicationQuit()
    {
        
    }
}
