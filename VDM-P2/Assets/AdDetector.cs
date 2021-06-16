using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdDetector : MonoBehaviour
{
    Button _btn;

    // Start is called before the first frame update
    void Start()
    {
        _btn = GetComponent<Button>();

        UpdateButtonStatus();
    } // Start

    public void UpdateButtonStatus()
    {
        if (GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            _btn.interactable = false;
        } // if
    } // UpdateButtonStatus
} // AdDetector
