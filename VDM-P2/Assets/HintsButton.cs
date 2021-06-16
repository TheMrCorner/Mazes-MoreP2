using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsButton : MonoBehaviour
{
    [Header("Configuration")]
    public LevelManager _lm;
    public BoardManager _bm;

    private Button _btn; 

    // Start is called before the first frame update
    void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        switch (_bm.TryShowHint())
        {
            case -1:
                _lm.StoreScreen();
                break;
            case -2:
                _lm.HintsCompletedScreen();
                break;
            default:
                _lm.UnHintsScreen();
                break;
        } // switch
    } // TskOnClick
}
