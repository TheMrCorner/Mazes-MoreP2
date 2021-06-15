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
    public LevelManager _levelManager = null;
    public LevelPackage[] _levels;          // Array of LevelPackages

    [Header("Debugging")]
    public bool _debugging = false;         // Sets if debug mode is on, for avoiding some changes


    private string _package = "Classic";     // Sets game difficulty
    private int _level = 1;                  // Sets the level to be loaded

    // SCALING DATA
    private Vector2 _scalingReferenceResolution;
    private Scaling _scalator;

    // GAME/SCENE MANAGEMENT
    private Random _rnd;
    private PlayerData _player;
    private RectTransform _topPanel;
    private RectTransform _bottomPanel;
    private MainMenuManager _mainMenu;
    private int _lastScene;
    private bool _adReward = false;
    private bool _isPaused = false;

    #endregion

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

    #region StartUpGameManager
    /// <summary>
    /// Variable that stores the instance of the GameManager, Singleton
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// Awake function of GameManager. Checks if another instance of this GameObject exists and 
    /// if not, initializes all required atributes and values of the GameManager, creating a new
    /// one. 
    /// 
    /// If the GameManager already exists, destroy this gameObject. 
    /// </summary>
    private void Awake()
    {
        // If GameManager is not created and initialized...
        if (_instance == null)
        {
            // Set this GameManager as instance
            _instance = this;

            string[] packagesNames = new string[_levels.Length];

            for (int i = 0; i < _levels.Length; i++)
            {
                packagesNames[i] = _levels[i].name;
            } // for

            // Store canvas' scaling reference resolution
            _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

            // Create scalator
            Vector2 res = new Vector2(Screen.width, Screen.height);
            _scalator = new Scaling(res, _scalingReferenceResolution, (int)_cam.orthographicSize);

            // Initialize random value
            _rnd = new Random();

            // Get Player information and store it
            _player = FileLoader.ReadPlayerData(packagesNames);

            DontDestroyOnLoad(_instance);

        }
        else if (_instance != this)
        {
            _instance._levelManager = _levelManager;

            Destroy(gameObject);
        }
    }


    // ------------------- PUBLIC -------------------

    public void ReceiveInput(InputManager.InputType it)
    {
        _levelManager.ReceiveInput(it);
    }


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
            else if (child.name == "Bottom")
            {
                GetInstance()._bottomPanel = child.GetComponent<RectTransform>();
            } // if
        } // foreach
    } // ReloadPanels

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

    #region GameManagement


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



    /// <summary>
    /// Function called when the Level is completed. Updates the level and
    /// calls the level manager so it shows the end panel
    /// </summary>
    public void LevelCompleted()
    {
        _level++;
        _levelManager.ShowEndMenu();
    }


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

    public void AdEnded()
    {
        GetInstance()._player._hints++;

        // MainMenu
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            _mainMenu.AdCompleted();
        } // if
    } // AdEnded
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
    #endregion

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


    /// <summary>
    /// 
    /// Change scene to LevelSelection updating LevelData object with the LevelPackage
    /// selected.
    /// 
    /// </summary>
    /// <param name="package"> (string) Name of the package to update it. </param>
    public void ChangeToLevelSelection()
    {
        // Update last Scene
        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;

        // Load LevelSelection scene
        SceneManager.LoadScene(1);
    } // ChangeToLevelSelection


    public void ChangeToLevel()
    {
        // Update last Scene
        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;

        // Load LevelSelection scene
        SceneManager.LoadScene(2);
    } // ChangeToLevel

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
    } // SetCamera

    public void SetCanvas(Canvas c)
    {
        GetInstance()._cnv = c;
        ReloadPanels();
    } // SetCanvas

    public void SetPackage(string p)
    {
        GetInstance()._package = p;
    } // SetPackage

    public void SetLevel(int i)
    {
        GetInstance()._level = i;
    } // SetLevel

    public void SetMainMenuManager(MainMenuManager mg)
    {
        GetInstance()._mainMenu = mg;
    } // SetMainMenuManager
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

    /// <summary>
    /// Gives access to the canvas stored in instance.
    /// </summary>
    /// <returns>Canvas canvas access</returns>
    public Canvas GetCanvas()
    {
        return GetInstance()._cnv;
    }

    public float GetTopPanelHeight()
    {
        return GetInstance()._topPanel.rect.height;
    } // GetTopPanelHeight

    public float GetBottomPanelHeight()
    {
        return GetInstance()._bottomPanel.rect.height;
    } // GetTopPanelHeight

    /// <summary>
    /// Gives the reference resolution used when scaling things for later use. 
    /// </summary>
    /// <returns>Vector2 Reference resolution</returns>
    public Vector2 getReferenceResolution()
    {
        return GetInstance()._scalingReferenceResolution;
    }

    public int GetNumPackages()
    {
        return GetInstance()._levels.Length;
    } // GetNumPackages

    /// <summary>
    /// 
    /// Get the name of the current 
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetPackageName()
    {
        return GetInstance()._package;
    } // GetPackageName


    public LevelPackage GetLevelPackage(int i)
    {
        return GetInstance()._levels[i];
    } // GetPackage

    public int GetLevel()
    {
        return GetInstance()._level;
    } // GetLevel

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
    public LevelPackage GetLevelPackage()
    {
        for (int i = 0; i < GetInstance()._levels.Length; i++)
        {
            if (GetInstance()._levels[i].name == _package)
            {
                return GetInstance()._levels[i];
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
        return GetInstance()._player;
    } // GetPlayerData
    #endregion

    #region AppLifeManagement
    /// <summary>
    /// Function that will manage the close of the app, saving the player's current status.
    /// </summary>
    private void OnApplicationQuit()
    {
        // Save player information
        FileLoader.SavePlayerData(GetInstance()._player);
    }

    #endregion
}