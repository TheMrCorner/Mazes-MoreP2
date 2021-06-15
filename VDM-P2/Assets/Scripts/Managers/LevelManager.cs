using System.Collections;
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

    [Header("Texts and data settings")]
    public Text _levelAndMode;
    public Text _hints;

    [Header("Options panels")]
    public GameObject _pausePanel;
    public GameObject _restorePanel;
    public GameObject _hintsPanel;
    public GameObject _optionsPanel;

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
        else
        {
            _boardManager.Init(this);
        }
    }

    private void Start()
    {
        GameManager.GetInstance().SetCamera(_camera);
        GameManager.GetInstance().SetCanvas(_canvas);

        _levelAndMode.text = GameManager.GetInstance().GetPackageName() + 
            " - " + GameManager.GetInstance().GetLevel().ToString();
        _hints.text = GameManager.GetInstance().GetPlayerData()._hints.ToString();

        _pausePanel.SetActive(false);
        _restorePanel.SetActive(false);
        _hintsPanel.SetActive(false);
        _optionsPanel.SetActive(false);

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
    public void PlayLevel()
    {
        LevelPackage lp = GameManager.GetInstance().GetLevelPackage();
        int level = GameManager.GetInstance().GetLevel();

        string levelUWU = lp.levels[level].ToString();

        Map map = Map.FromJson(lp.levels[level].ToString());
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
        _pausePanel.SetActive(true);
        _paused = true;
    } // PauseGame

    public void UnPauseGame()
    {
        _pausePanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    public void RestoreGameScreen()
    {
        _restorePanel.SetActive(true);
        _paused = true;
    } // PauseGame

    public void UnRestoreGameScreen()
    {
        _restorePanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    public void HintsScreen()
    {
        _hintsPanel.SetActive(true);
        _paused = true;
    } // PauseGame

    public void UnHintsScreen()
    {
        _hintsPanel.SetActive(false);
        _paused = false;
    } // UnPauseGame
} // LevelManager
