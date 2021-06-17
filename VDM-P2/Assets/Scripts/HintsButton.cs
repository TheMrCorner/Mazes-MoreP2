using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Configures the button that will try to show a hint in-game.
/// 
/// </summary>
public class HintsButton : MonoBehaviour
{
    [Header("Configuration")]
    public LevelManager _lm;     // LevelManager for notifications
    public BoardManager _bm;     // BoardManager for notifications

    private Button _btn;         // Button component.

    // Start is called before the first frame update
    void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(TaskOnClick);
    } // Start

    /// <summary>
    /// 
    /// Update the sprite and set it unavailable to use if ads are removed.
    /// 
    /// </summary>
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
} // HintsButton
