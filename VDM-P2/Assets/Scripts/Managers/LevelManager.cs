﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //TODO: do we still use the underscore for public fields like this?
    [Tooltip("Board Manager object")]
    public BoardManager _boardManager;
    public Canvas _canvas;
    public Camera _camera;
    public RectTransform _topPanel;
    public RectTransform _botPanel;

    public GameObject _endPanel;

    [Header("Texts and data settings")]
    public Text _levelAndMode;
    public Text _hints;
    public Text _hinstPanel;

    [Header("Options panels")]
    public GameObject _pausePanel;
    public GameObject _restorePanel;
    public GameObject _hintsPanel;
    public GameObject _optionsPanel;
    public Button _homeEndedButton;
    public Button _homePauseButton;

    private bool _paused = false;


    // ----------------------------------------------
    // --------------- UNITY METHODS ----------------
    // ----------------------------------------------

    void Awake()
    {
        if (_boardManager == null)
        {
            Debug.LogError("Board Manager reference not set");
        }
        if (_endPanel == null)
        {
            Debug.LogError("End panel reference not set");
        }
        else
        {
            _boardManager.Init(this);
        }
    }

    private void Start()
    {
        GameManager.GetInstance().SetCamera(_camera);
        GameManager.GetInstance().SetCanvas(_canvas);

        _homePauseButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);
        _homeEndedButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);

        PlayLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------- PUBLIC -------------------
    public void ShowEndMenu()
    {
        _endPanel.SetActive(true);
    }


    public void PlayLevel()
    {
        _levelAndMode.text = GameManager.GetInstance().GetPackageName() +
               " - " + GameManager.GetInstance().GetLevel().ToString();
        _hints.text = _hinstPanel.text = GameManager.GetInstance().GetPlayerData()._hints.ToString();

        _pausePanel.SetActive(false);
        _restorePanel.SetActive(false);
        _hintsPanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _endPanel.SetActive(false);

        LevelPackage lp = GameManager.GetInstance().GetLevelPackage();
        int level = GameManager.GetInstance().GetLevel();

        string levelUWU = lp.levels[level].ToString();

        Map map = Map.FromJson(lp.levels[level].ToString());

        _boardManager.EmptyBoard();
        _boardManager.SetMap(map);
    }

    public void ReceiveInput(InputManager.InputType it)
    {
        if (!_paused)
        {
            _boardManager.ReceiveInput(it);
        } // if
    } // ReceiveInput

    public void PauseGame()
    {
        _optionsPanel.SetActive(true);
        _pausePanel.SetActive(true);
        _paused = true;
    } // PauseGame

    public void UnPauseGame()
    {
        _optionsPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    public void RestoreGameScreen()
    {
        _optionsPanel.SetActive(true);
        _restorePanel.SetActive(true);
        _paused = true;
    } // PauseGame

    public void UnRestoreGameScreen()
    {
        _optionsPanel.SetActive(false);
        _restorePanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    public void HintsScreen()
    {
        _optionsPanel.SetActive(true);
        _hintsPanel.SetActive(true);
        _paused = true;
    } // PauseGame

    public void UnHintsScreen()
    {
        _optionsPanel.SetActive(false);
        _hintsPanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    public void StoreScreen()
    {

    } // StoreScreen

    public void UnStoreScreen()
    {

    } // UnStoreScreen

    public void HintsCompletedScreen()
    {

    } // HintsCompletedScreen

    public void UnHintsCompletedScreen()
    {

    } // UnHintsCompletedScreen
} // LevelManager
