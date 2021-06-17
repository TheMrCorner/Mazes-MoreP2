using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Detects if ads have been removed and deactivates the button.
/// 
/// </summary>
public class AdDetector : MonoBehaviour
{
    Button _btn;

    // Start is called before the first frame update
    void Start()
    {
        _btn = GetComponent<Button>();

        UpdateButtonStatus();
    } // Start

    /// <summary>
    /// 
    /// Update button status to block it when necessary.
    /// 
    /// </summary>
    public void UpdateButtonStatus()
    {
        if (GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            _btn.interactable = false;
        } // if
    } // UpdateButtonStatus
} // AdDetector
