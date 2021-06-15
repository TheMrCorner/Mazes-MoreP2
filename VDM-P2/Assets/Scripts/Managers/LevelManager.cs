using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //TODO: do we still use the underscore for public fields like this?
    [Tooltip("Board Manager object")]
    public BoardManager _boardManager;
    public Canvas _canvas;
    public Camera _camera;
<<<<<<< HEAD
    public GameObject _endPanel;
=======
    public RectTransform _topPanel;
    public RectTransform _botPanel;
>>>>>>> main


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
        _boardManager.ReceiveInput(it);
    }
}
