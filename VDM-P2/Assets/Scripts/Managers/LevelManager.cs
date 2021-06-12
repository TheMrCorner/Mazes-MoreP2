using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //TODO: do we still use the underscore for public fields like this?
    [Tooltip("Board Manager object")]
    public BoardManager _boardManager;


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

    // Update is called once per frame
    void Update()
    {
        
    }

    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------- PUBLIC -------------------
    public void PlayLevel(int level)
    {
        Map map = Map.FromJson("Assets/Levels/" + GameManager.GetInstance().GetGameMode() + "/" + level.ToString() + ".json");
        _boardManager.SetMap(map);
    }

    public void ReceiveInput(InputManager.InputType it)
    {
        _boardManager.ReceiveInput(it);
    }
}
