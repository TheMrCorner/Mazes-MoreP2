using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// GameManager class. Manages all changes between scenes, the levels that will be
/// played and the challenges. 
/// 
/// Has all the information about the player: how many coins they have, levels completed
/// per difficulty and medals gained in challenges. Manages the changes in their stats
/// and saves it when the app is closed. 
/// 
/// Manages the time left for a challenge and notifies the MainMenu when a challenge is 
/// available. 
/// 
/// Has the instance of the different AssetBundles. All the scripts will access this instances
/// to retrieve all prefab information.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables
    /// </summary>
    [Header("Resolution Configuration")]
    public Canvas _cnv;
    public Camera _cam;

    [Header("Levels")]
    public LevelPackage[] _levels;          // Array of LevelPackages

    [Header("Debugging")]
    public bool _debugging = false;         // Sets if debug mode is on, for avoiding some changes
    public string _package = "Classic";     // Sets game difficulty
    public int _level = 0;                  // Sets the level to be loaded
    
    // SCALING DATA
    private Vector2 _scalingReferenceResolution;
    private Scaling _scalator; 
    
    // GAME/SCENE MANAGEMENT
    private Random _rnd;
    private PlayerData _player;
    private RectTransform _topPanel;
    private int _lastScene;
    private bool _adReward = false;
    private bool _isPaused = false;
    
    #endregion

//    /*
//    // GAME/SCENE MANAGEMENT
//    /// <summary>  Last position of the hints array </summary>
//    Vector2 _lastTouchPosition;
//    /// <summary> LevelManager, for communication in-game </summary>
//    LevelManager _lm;
//    /// <summary> Random instance for generating random levels </summary>
//    Random _rnd; 
//    MainMenu _mm;
//    /// <summary> Scene before actual scene </summary>
//    int _lastScene;
//    /// <summary> Max difficulty in game (data driven) </summary>
//    int _maxDifficulty = 4;
//    /// <summary> Flag to control when ad is requested </summary>
//    bool _adRewardCoins = false;
//    /// <summary> Flag to double challenge win bounty </summary>
//    bool _adDoubleChallenge = false;
//    /// <summary> After ad init challenge </summary>
//    bool _adChallengeInit = false;
//    /// <summary> Flag to check if challenge has been completed </summary>
//    bool _challengeWaiting = false;
//    /// <summary> Time for the next challenge </summary>
//    float _timeChallengeWait = 0.0f;
//    /// <summary> Android focus in the app management </summary>
//    bool isPaused = false;
//    */
//    #endregion

//    #region Utilities
//    /// <summary>
//    /// Converts the actual date and hour into seconds.
//    /// </summary>
//    /// <returns>Date transformed in seconds</returns>
//    int ConvertDateToSecond()
//    {
//        int totalHours = 0;

//        totalHours += System.DateTime.Now.Second;
//        totalHours += System.DateTime.Now.Minute * 60;
//        totalHours += System.DateTime.Now.Hour * 60 * 60;
//        totalHours += System.DateTime.Now.Day * 24 * 60 * 60;
//        totalHours += System.DateTime.Now.Month * 30 * 24 * 60 * 60;

//        return totalHours;
//    }
//    #endregion

//    #region StartUpGameManager
//    /// <summary>
//    /// Variable that stores the instance of the GameManager, Singleton
//    /// </summary>
//    private static GameManager _instance;

//    /// <summary>
//    /// Awake function of GameManager. Checks if another instance of this GameObject exists and 
//    /// if not, initializes all required atributes and values of the GameManager, creating a new
//    /// one. 
//    /// 
//    /// If the GameManager already exists, destroy this gameObject. 
//    /// </summary>
//    private void Awake()
//    {
//        // If GameManager is not created and initialized...
//        if (_instance == null)
//        {
//            // Set this GameManager as instance
//            _instance = this;

            string[] packagesNames = new string[_levels.Length];

            for (int i = 0; i < _levels.Length; i++)
            {
                packagesNames[i] = _levels[i].name;
            } // for

            // Store canvas' scaling reference resolution
            _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

//            // Store canvas' scaling reference resolution
//            _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

            // Initialize random value
            _rnd = new Random();

            // Get Player information and store it
            _player = FileLoader.ReadPlayerData(packagesNames);

        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

//            // Set the time waiting for challenge info
//            SetTimeForChallenge();
//            */
//        }
//        else if (_instance != this)
//        {
//            Destroy(gameObject);
//        }
//    }

    
    /// <summary>
    /// 
    /// Load all panels in scene for dimensions info.
    /// 
    /// </summary>
    public void ReloadPanels()
    {
        // Search in the canvas and check names
        foreach (Transform child in GetInstance()._cnv.transform)
        {
            if (child.name == "Top")
            {
                GetInstance()._topPanel = child.GetComponent<RectTransform>();
            } // if
        } // foreach
    } // ReloadPanels
    

//    /*
//    /// <summary>
//    /// Load all panels in scene for dimensions info. 
//    /// </summary>
//    public void ReloadPanels()
//    {
//        // Search in the canvas and check names
//        foreach (Transform child in GetInstance()._cnv.transform)
//        {
//            if (child.name == "Top")
//            {
//                GetInstance()._panelSuperior = child.GetComponent<RectTransform>();
//            }
//            else if (child.name == "Bottom")
//            {
//                GetInstance()._panelInferior = child.GetComponent<RectTransform>();
//            }
//        }
//    }
//    */

    /// <summary>
    /// 
    /// Gives access to the GameManager instance for the rest of scripts and objects, 
    /// also is used for changing some values in the gameManager only by the GameManager.
    /// 
    /// </summary>
    /// <returns> (GameManager) Actual instance of the GameManager. </returns>
    public static GameManager GetInstance()
    {
        return _instance;
    } // GetInstance
    #endregion

//    /// <summary>
//    /// Gives access to the GameManager instance for the rest of scripts and objects, 
//    /// also is used for changing some values in the gameManager only by the GameManager.
//    /// </summary>
//    /// <returns>Actual instance of the GameManager</returns>
//    public static GameManager GetInstance()
//    {
//        return _instance;
//    }
//    #endregion

//    #region GameManagement
//    /*
//    /// <summary>
//    /// Updates the time remaining for the next challenge. Notifies the 
//    /// MainMenu to update the time left text. 
//    /// </summary>
//    private void Update()
//    {
//        // If is waiting time
//        if (GetInstance()._challengeWaiting)
//        {
//            // Calculates the elapsed time since the last frame
//            GetInstance()._timeChallengeWait -= Time.deltaTime;

//            if (SceneManager.GetActiveScene().buildIndex == 0)
//                GetInstance()._mm.UpdateTime(GetInstance()._timeChallengeWait);

//            // If the timer ends
//            if (GetInstance()._timeChallengeWait <= 0)
//            {
//                // Restart the time information and the original state of the main menu
//                // The challenge button is active and not blocked again 
//                GetInstance()._timeChallengeWait = GetInstance()._gc._challengeWaitTime;
//                GetInstance()._challengeWaiting = false;
//            }
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called by the InputManager when the player touches the 
//    /// screen. Checks if the actual scene is the Game one, and manages all
//    /// necesary communication with the LevelManager. 
//    /// </summary>
//    /// <param name="touchPosition">Position of the Input in Screen</param>
//    public void ScreenTouched(Vector2 touchPosition)
//    {
//        // Check if the actual scene is the game scene
//        if (SceneManager.GetActiveScene().buildIndex == 2)
//        {
//            // Check if the Input happened in the play zone
//            if (IsInPlayZone(touchPosition))
//            {
//                // Inform the LevelManager
//                touchPosition = GetInstance()._scalator.ScreenToWorldPosition(touchPosition);
//                GetInstance()._lm.ScreenTouched(touchPosition);
//            }
//        }
//        // If not, the GameManager doesn't have to manage the input
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the InputManager detects that the player has 
//    /// released the scren (stopped touching it). Informs the LevelManager
//    /// if the current scene is the Game scene. 
//    /// </summary>
//    public void ScreenReleased()
//    {
//        // Check actual scene is GameScene
//        if (SceneManager.GetActiveScene().buildIndex == 2)
//            GetInstance()._lm.ScreenReleased();
//    }
//    */


//    /*
//    /// <summary>
//    /// Checks if some coordinates are between the two panels, top and bottom.
//    /// </summary>
//    /// <param name="position">Coordinates to check</param>
//    /// <returns>Whether position is in PlayZone or not</returns>
//    public bool IsInPlayZone(Vector2 position)
//    {
//        // Check if actual scene is GameScene
//        if (SceneManager.GetActiveScene().buildIndex == 2)
//            // Return if the coordinates are in the PlayZone
//            return position.y < (GetInstance()._scalator.CurrentResolution().y - panelSuperiorHeight()
//                * GetInstance()._cnv.scaleFactor) && position.y > (panelInferiorHeight() * GetInstance()._cnv.scaleFactor);
//        else
//            return false;
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the Level is completed. Updates player information
//    /// with the new level completed. 
//    /// </summary>
//    public void LevelCompleted()
//    {
//        // Checks if the actual level is the last one completed
//        if (GetInstance()._level == GetInstance()._currentPlayerData._completedLevelsInDifficulty[GetInstance()._difficulty])
//        {
//            // If so, unlocks the next level
//            GetInstance()._currentPlayerData._completedLevelsInDifficulty[GetInstance()._difficulty] += 1;
//        }
//    }
//    */

    /*
    /// <summary>
    /// Function called by Buttons to return the level to it's initial state. 
    /// </summary>
    public void ResetLevel()
    {
        // Call LevelManager to reset the level
        GetInstance()._lm.ReloadLevel();
    }
    */

//    /*
//    /// <summary>
//    /// Function called by Buttons to return the level to it's initial state. 
//    /// </summary>
//    public void ResetLevel()
//    {
//        // Call LevelManager to reset the level
//        GetInstance()._lm.ReloadLevel();
//    }
//    */

    /*
    /// <summary>
    /// Function that notifies when an ad has ended and manages what happens next. 
    /// If the ad played was a rewarded ad, gives coins to the player. If the ad 
    /// was for doubling the bounty earned in a challenge, calls that function. 
    /// If the ad was for initiating a challenge, calls all necessary functions. 
    /// </summary>
    public void AdEnded()
    {
        
    }
    */
    #endregion

    #region LevelSelectionManagement
    
    /// <summary>
    /// 
    /// Sets the top text for the LevelSelection menu. Selects the right name 
    /// from the collection of difficulty names and puts it in the Text.
    /// 
    /// </summary>
    public void CreateTextLevelSelectionMenu()
    {
        
    }
    

//    /*
//    /// <summary>
//    /// Function that notifies when an ad has ended and manages what happens next. 
//    /// If the ad played was a rewarded ad, gives coins to the player. If the ad 
//    /// was for doubling the bounty earned in a challenge, calls that function. 
//    /// If the ad was for initiating a challenge, calls all necessary functions. 
//    /// </summary>
//    public void AdEnded()
//    {
//        // Ad for coins reward
//        if (GetInstance()._adRewardCoins)
//        {
//            AddCoins();
//        }
//        // Ad for double reward in challenge
//        else if (GetInstance()._adDoubleChallenge)
//        {
//            AddChallengeExtraCoins();
//        }
//        // Ad for playing a Challenge
//        else if (GetInstance()._adChallengeInit)
//        {
//            InitChallenge();
//        }
//    }
//    */
//    #endregion

    #region SceneManagement
    
    /// <summary>
    /// Function called by the Back buttons. Loads the last scene that was playing. 
    /// </summary>
    public void ReturnToLastScene()
    {
        // Loads the last scene
        SceneManager.LoadScene(GetInstance()._lastScene);
    }
    
    
    /// <summary>
    /// Return to the MainMenu Scene. 
    /// </summary>
    public void ReturnToMenu()
    {
        // Load MainMenu scene
        SceneManager.LoadScene(0);
    }

    /*
    /// <summary>
    /// Loads the next level after completing one. Checks if it is the last level of it's
    /// difficulty and if so, loads the next difficulty and it's first level. If the difficuñty
    /// is the last one, returns to the first difficulty.
    /// </summary>
    public void NextLevel()
    {
        // Increase actual level
        GetInstance()._level += 1;


//    /*
//    /// <summary>
//    /// Loads the next level after completing one. Checks if it is the last level of it's
//    /// difficulty and if so, loads the next difficulty and it's first level. If the difficuñty
//    /// is the last one, returns to the first difficulty.
//    /// </summary>
//    public void NextLevel()
//    {
//        // Increase actual level
//        GetInstance()._level += 1;

//        // Check which level is the next one
//        if (GetInstance()._level > 100)
//        {
//            // Load the level 1
//            GetInstance()._level = 1;

//            // Increase difficulty
//            GetInstance()._difficulty += 1;
//            // Check difficulty after increase
//            if (GetInstance()._difficulty > GetInstance()._maxDifficulty)
//            {
//                // Return to first difficulty
//                GetInstance()._difficulty = 0;
//            }
//        }

//        // Reloads the LevelScene
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }
//    */


//    /*
//    /// <summary>
//    /// Loads the Game scene with a concrete level selected. 
//    /// </summary>
//    /// <param name="l">Level to be loaded</param>
//    public void InitLevel(int l)
//    {
//        // Store last scene for later loading it if necessary
//        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;
//        // Establish the level
//        GetInstance()._level = l;

    
    /// <summary>
    /// 
    /// Change scene to LevelSelection updating LevelData object with the LevelPackage
    /// selected.
    /// 
    /// </summary>
    /// <param name="package"> (string) Name of the package to update it. </param>
    public void ChangeToLevelSelection(string package)
    {
        // Update last Scene
        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;

        // Load LevelSelection scene
        SceneManager.LoadScene(1);
    } // ChangeToLevelSelection

    #endregion

    #region Setters

    /// <summary>
    /// 
    /// Set the camera of the scene as the instance's camera.
    /// 
    /// </summary>
    /// <param name="c"> (Camera) Camera to be set. </param>
    public void SetCamera(Camera c)
    {
        // Set the camera
        GetInstance()._cam = c;
    }
    #endregion

    #region Getters
    
    /// <summary>
    /// Gives access to the scalator instance.
    /// </summary>
    /// <returns>Scaling scaling instance stored in GM instance</returns>
    public Scaling GetScaling()
    {
        return GetInstance()._scalator;
    }
    

//    /*
//    /// <summary>
//    /// Sets the remaining time for the next challenge. Stores it for later use. 
//    /// </summary>
//    /// <param name="timing">Time for the next challenge</param>
//    public void SetChallengeTimeRemaining(int timing)
//    {
//        // Calculate the date for it
//        GetInstance()._currentPlayerData._dateForNextChallenge = GetInstance().ConvertDateToSecond() + timing;
//    }
//    */
//    #endregion

//    #region Getters
//    /*
//    /// <summary>
//    /// Gives access to the scalator instance.
//    /// </summary>
//    /// <returns>Scaling scaling instance stored in GM instance</returns>
//    public Scaling GetScaling()
//    {
//        return GetInstance()._scalator;
//    }
//    */


//    /// <summary>
//    /// Gives access to the canvas stored in instance.
//    /// </summary>
//    /// <returns>Canvas canvas access</returns>
//    public Canvas GetCanvas()
//    {
//        return GetInstance()._cnv;
//    }


    
    /// <summary>
    /// Gives the reference resolution used when scaling things for later use. 
    /// </summary>
    /// <returns>Vector2 Reference resolution</returns>
    public Vector2 getReferenceResolution()
    {
        return GetInstance()._scalingReferenceResolution;
    }

    public string GetPackageName()
    {
        return _package;
    } // GetPackageName


    /// <summary>
    /// 
    /// Returns the package of the level selected. Searches in
    /// the LevelPackage's list and compares the names of the 
    /// objects. When object is found, returns it. 
    /// 
    /// If LevelPackage is not found, return null. 
    /// 
    /// </summary>
    /// <returns> (LevelPackage) Selected LevelPackage. </returns>
    public LevelPackage getLevelPackage()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            if(_levels[i].name == _package)
            {
                return _levels[i];
            } // if
        } // for

        return null;
    } // getLevelPackage

    /// <summary>
    /// 
    /// Gives access to the player data and all the completed level 
    /// and etc.
    /// 
    /// </summary>
    /// <returns> (PlayerData) Actual player data loaded. </returns>
    public PlayerData GetPlayerData()
    {
        return _player;
    } // GetPlayerData
    #endregion


//    /*
//    /// <summary>
//    /// Gives how many challenges has the player completed successfully. 
//    /// </summary>
//    /// <returns>int Number of challenges completed</returns>
//    public int getChallengesCompleted()
//    {
//        return GetInstance()._currentPlayerData._challengesCompleted;
//    }
//    */
//    #endregion

    
    // /// <summary>
    // /// Checks if the app has the focus and reloads all the info about the 
    // /// player and the times in case it's needed. 
    // /// </summary>
    // /// <param name="focus"></param>
    // private void OnApplicationFocus(bool focus)
    // {
    //     isPaused = !focus;

    //     if (focus)
    //     {
    //         // Get Player information and store it
    //         GetInstance()._currentPlayerData = LoadingFiles.ReadPlayerData(GetInstance()._maxDifficulty);

    //         // Set the time waiting for challenge info
    //         SetTimeForChallenge();
    //     }
    //     else
    //     {
    //         // Update _currentPlayerData 
    //         if (GetInstance()._challengeWaiting)
    //         {
    //             GetInstance()._currentPlayerData._dateForNextChallenge = (int)(ConvertDateToSecond() + GetInstance()._timeChallengeWait);
    //         }
    //         // Save player information
    //         LoadingFiles.SavePlayerData(GetInstance()._currentPlayerData);
    //     }
    // }
    

//        if (focus)
//        {
//            // Get Player information and store it
//            GetInstance()._currentPlayerData = LoadingFiles.ReadPlayerData(GetInstance()._maxDifficulty);

    
    // /// <summary>
    // /// When the app loses focus, saves the info about the player, because the 
    // /// mobilephone can have the functionality to stop apps that don't have 
    // /// the focus, this way we ensure the player information is safe.
    // /// </summary>
    // /// <param name="pause"></param>
    // private void OnApplicationPause(bool pause)
    // {
    //     // Update _currentPlayerData 
    //     if (GetInstance()._challengeWaiting)
    //     {
    //         GetInstance()._currentPlayerData._dateForNextChallenge = (int)(ConvertDateToSecond() + GetInstance()._timeChallengeWait);
    //     }

    //     // Save player information
    //     LoadingFiles.SavePlayerData(GetInstance()._currentPlayerData);
    // }
    

//    /*
//    /// <summary>
//    /// When the app loses focus, saves the info about the player, because the 
//    /// mobilephone can have the functionality to stop apps that don't have 
//    /// the focus, this way we ensure the player information is safe.
//    /// </summary>
//    /// <param name="pause"></param>
//    private void OnApplicationPause(bool pause)
//    {
//        // Update _currentPlayerData 
//        if (GetInstance()._challengeWaiting)
//        {
//            GetInstance()._currentPlayerData._dateForNextChallenge = (int)(ConvertDateToSecond() + GetInstance()._timeChallengeWait);
//        }

    
    // /// <summary>
    // /// Function that will manage the close of the app, saving the player's current status.
    // /// </summary>
    // private void OnApplicationQuit()
    // {
    //     // Update _currentPlayerData 
    //     if (GetInstance()._challengeWaiting)
    //     {
    //         GetInstance()._currentPlayerData._dateForNextChallenge = (int)(ConvertDateToSecond() + GetInstance()._timeChallengeWait);
    //     }

    //     // Save player information
    //     LoadingFiles.SavePlayerData(GetInstance()._currentPlayerData);
    // }
    
    #endregion
}