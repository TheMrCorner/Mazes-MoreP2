using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public 

    // Private 
    private static GameManager _instance; // Singleton instance

    // Init singleton in Awake
    private void Awake() {
        if(_instance == null){
            // Save GameManager instance
            _instance = this; 

            // TODO: Implement initialization
            // ...
        } // if
        else if (_instance != this) {
            Destroy(gameObject);
        } // else if
    } // Awake

    // Start is called before the first frame update
    void Start()
    {
        
    } // Start

    // Update is called once per frame
    void Update()
    {
        
    } // Update
} // GameManager
