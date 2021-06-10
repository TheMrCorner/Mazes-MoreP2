using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // Public ------------------------------------------
#if UNITY_EDITOR
    public int _levelToPlay = 1;
#endif
    public LevelManager _levelManager;
    public LevelPackage[] _levelPackages;

    // Private -----------------------------------------
    static GameManager _instance; // Singleton instance

    // for level handling
    enum GameMode { CLASSIC, ICE};
    GameMode _gameMode = GameMode.CLASSIC;

    /// <summary>
    /// Awake function of GameManager. Checks if another instance of this GameObject exists and 
    /// if not, initializes all required atributes and values of the GameManager, creating a new
    /// one. 
    /// 
    /// If the GameManager already exists, destroy this gameObject. 
    /// </summary>
    private void Awake()
    {
        if (_instance != null)
        {
            _instance._levelManager = _levelManager;
            StartNewScene();
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            // Set this GameManager as instance
            _instance = this;

            // Set this gameObject to not Destroy when changing between scenes
            DontDestroyOnLoad(gameObject);
        }

    } // Awake

    // Start is called before the first frame update
    void Start()
    {
        // TODO: remove this when we're done testing
        _levelManager.PlayLevel(_levelToPlay);
    } // Start

    // Update is called once per frame
    void Update()
    {

    } // Update

    /// <summary>
    /// Gives access to the GameManager instance for the rest of scripts and objects, 
    /// also is used for changing some values in the gameManager only by the GameManager.
    /// </summary>
    /// <returns>Actual instance of the GameManager</returns>
    public static GameManager GetInstance()
    {
        return _instance;
    }

    private void StartNewScene()
    {
        if (_levelManager)
        {
            // TODO: change this to proper level handling
            //level manager->load levelToPlay
            _levelManager.PlayLevel(_levelToPlay);
        }
    }

    public string GetGameMode()
    {
        switch (_gameMode)
        {
            case GameMode.CLASSIC:
                return "classic";
            case GameMode.ICE:
                return "ice_floor";
            default:
                return "";
        }
    }

    public void AdEnded()
    {
        // TODO: fill out 

        //if (GetInstance().adRewardCoins)
        //{
        //    AddCoins();
        //}
        //else if (GetInstance().adDoubleChallenge)
        //{
        //    AddChallengeExtraCoins();
        //}
        //else if (GetInstance().adChallengeInit)
        //{
        //    InitChallenge();
        //}
    }
} // GameManager



///// <summary>
///// GameManager class. Manages all changes between scenes, the levels that will be
///// played and the challenges. 
///// 
///// Has all the information about the player: how many coins they have, levels completed
///// per difficulty and medals gained in challenges. Manages the changes in their stats
///// and saves it when the app is closed. 
///// 
///// Manages the time left for a challenge and notifies the MainMenu when a challenge is 
///// available. 
///// 
///// Has the instance of the different AssetBundles. All the scripts will access this instances
///// to retrieve all prefab information.
///// </summary>
//public class GameManager : MonoBehaviour
//{

//    #region Variables
//    // Public
//    /// <summary>
//    /// Variables needed to set the scaling class for scaling all objects in the scene 
//    /// correctly. 
//    /// 
//    /// The canvas is used for getting the reference resolution and for calculating the 
//    /// space left for the Board and Game to establish. 
//    /// 
//    /// Camera is needed for calculating the translation factor between Unity Units and
//    /// pixels. 
//    /// </summary>
//    [Header("Resolution Configuration")]
//    /// <summary> Canvas of the scene </summary>
//    public Canvas _cnv;
//    /// <summary> Camera of the scene </summary>
//    public Camera _cam;

//    /// <summary>
//    /// This variables are only public for debugging a concrete scene or gamemode. They
//    /// are later set in the different functions with the values needed or random values 
//    /// if debugging is just for certain things. 
//    /// </summary>
//    [Header("Debugging")]
//    public bool _debugging = false;         // Sets if debug mode is on, for avoiding some changes
//    public bool _challenge = false;         // Sets if gamemode is a challenge
//    public int _difficulty = 0;             // Sets game difficulty
//    public int _level = 0;                  // Sets the level to be loaded

//    // Private
//    /// <summary> How many levels are in a certain difficulty </summary>
//    int[] _levelsInDifficulty;

//    /*
//    // GAME DATA
//    /// <summary> LoadAssetBundle instance </summary>
//    LoadAssetBundle _lab;
//    /// <summary> CurrentPlayer data </summary>
//    PlayerData _currentPlayerData;
//    /// <summary> Game Configuration read from a file </summary>
//    GameConfig _gc;
//    /// <summary> Info about the game, for calculations and data config </summary>
//    GameFilesInfo _gi;
//    */

//    /*
//    // SCALING DATA
//    /// <summary> Reference resolution for scaling </summary>
//    Vector2 _scalingReferenceResolution;
//    /// <summary> Top panel in the canvas </summary>
//    RectTransform _panelSuperior;
//    /// <summary> Bottom panel in canvas </summary>
//    RectTransform _panelInferior;
//    /// <summary> Scaling instance, simplify some things </summary>
//    Scaling _scalator; 
//    */

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

//            // Set this gameObject to not Destroy when changing between scenes
//            DontDestroyOnLoad(gameObject);

//            /*
//            _gc = LoadingFiles.ReadGameConfig();

//            // Store canvas' scaling reference resolution
//            _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

//            // Initialize Scaling with cam values and scalingreference values
//            _scalator = new Scaling(new Vector2(Screen.width, Screen.height), _scalingReferenceResolution, (int)_cam.orthographicSize);

//            // Search all panels for later calculation
//            ReloadPanels();

//            // Load all AssetBundles 
//            _lab = new LoadAssetBundle();

//            // Depending on OS system, load from a different path and using different techniques
//#if !UNITY_EDITOR && UNITY_ANDROID 
//            _lab.LoadBundlesAndroid(Application.streamingAssetsPath + "/AssetBundles/");
//#else
//            _lab.LoadBundlesWindows(Application.streamingAssetsPath + "/AssetBundles/");
//#endif

//            // Initialize random value
//            _rnd = new Random();

//            // Load gameInfo previously generated, data driven information
//            _gi = LoadingFiles.ReadGameInfo();

//            // Set all values of this information
//            SetGameInfo();

//            // Get Player information and store it
//            _currentPlayerData = LoadingFiles.ReadPlayerData(_maxDifficulty);

//            // Set the time waiting for challenge info
//            SetTimeForChallenge();
//            */
//        }
//        else if (_instance != this)
//        {
//            Destroy(gameObject);
//        }
//    }


//    /*
//    /// <summary>
//    /// Calculates time reamining for a new challenge. Checks if the actual
//    /// time is less than the challenge date. 
//    /// </summary>
//    public void SetTimeForChallenge()
//    {
//        if (GetInstance().ConvertDateToSecond() < GetInstance()._currentPlayerData._dateForNextChallenge)
//        {
//            int challengeTimeRemaining = GetInstance()._currentPlayerData._dateForNextChallenge - GetInstance().ConvertDateToSecond();

//            GetInstance()._timeChallengeWait = challengeTimeRemaining;
//            GetInstance()._challengeWaiting = true;
//        }
//        else
//        {
//            GetInstance()._timeChallengeWait = (GetInstance()._gc._challengeWaitTime * 60.0f);
//            GetInstance()._challengeWaiting = false;
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Sets all the necessary Game information. If no values were assigned previously, 
//    /// generates some random values. 
//    /// 
//    /// If DebugMode is true, changes nothing.
//    /// </summary>
//    public void SetGameInfo()
//    {
//        // Check if maximum difficulty configurated in JSON is the same as the number of files in /StreamingAssets/Levels
//        if (GetInstance()._gi._numDifficulties == GetInstance()._gc._maxDifficulty)
//        {
//            // Get how many difficulties are
//            int difficulties = GetInstance()._gi._numDifficulties;

//            // Establishes maximum difficulty for random generated level
//            GetInstance()._maxDifficulty = difficulties;

//            // Gets how many levels per difficulty are
//            GetInstance()._levelsInDifficulty = new int[GetInstance()._maxDifficulty];

//            for (int i = 0; i < _levelsInDifficulty.Length; i++)
//            {
//                LevelReader temp = new LevelReader(i);
//                GetInstance()._levelsInDifficulty[i] = temp.GetNumLevels();
//            }

//            // If the actual scene is Game scene generate random ones
//            if (SceneManager.GetActiveScene().buildIndex == 2)
//            {
//                // Check debugmode
//                if (!_debugging)
//                {
//                    GetInstance()._difficulty = Random.Range(0, _maxDifficulty);
//                    LevelReader temp = new LevelReader(GetInstance()._difficulty);
//                    GetInstance()._level = Random.Range(1, temp.GetNumLevels() + 1);
//                }
//            }

//            // If the actual scene is LevelSelection, generate random difficulty
//            if (SceneManager.GetActiveScene().buildIndex == 1)
//            {
//                // Check debugmode
//                if (!_debugging)
//                {
//                    GetInstance()._difficulty = Random.Range(0, _maxDifficulty);
//                }
//            }
//        }
//        else
//        {
//            Debug.LogError("Difficulties not match, check maxDifficulty in " +
//                "game_config or check number of files in Levels folder.");
//        }
//    }
//    */


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


//    /*
//    /// <summary>
//    /// Function that manages the challenge state. Called when the challenge has ended, 
//    /// no matter if player won or not. 
//    /// </summary>
//    public void ChallengeCompleted()
//    {
//        // Update challenge value
//        GetInstance()._challenge = false;
//        GetInstance()._challengeWaiting = true;
//        GetInstance()._timeChallengeWait = GetInstance()._gc._challengeWaitTime * 60.0f;
//    }
//    */


//    /*
//    /// <summary>
//    /// Called when the time available to complete a challenge has ended. 
//    /// </summary>
//    public void TimeIsUp()
//    {
//        // Inform LevelManager that Game has ended when no more time is left
//        GetInstance()._lm.EndGame();
//    }
//    */


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


//    /*
//    /// <summary>
//    /// Add coins to the player when an ad has ended. Gives the player a random number
//    /// of coins between 10 and a maximum established in editor after watching an ad. 
//    /// </summary>
//    void AddCoins()
//    {
//        // Generate the random amount of coins
//        int add = Random.Range(10, GetInstance()._gc._coinsMaxReward + 1);
//        // Update player coins with those new coins
//        GetInstance()._currentPlayerData._coinsPlayer += add;

//        _lm.UpdateCoins();
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called after watching an ad for givin the player an extra amount of 
//    /// coins when winning a challenge. 
//    /// </summary>
//    void AddChallengeExtraCoins()
//    {
//        // Update player data, then return to Main Menu
//        GetInstance()._currentPlayerData._coinsPlayer += GetInstance()._gc._challengeReward;
//        ReturnToMenu();
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the player presses the button in a normal Game to win coins
//    /// for buying some hints later. 
//    /// </summary>
//    public void AdRewardCoins()
//    {
//        // Set the flag true for later checking
//        GetInstance()._adRewardCoins = true;
//        // Call AdManager to initiate the ad
//        AdManager.GetInstance().ShowRewardedVideo();
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the player presses the button in Challenge Victory screen. 
//    /// Gives them more coins after winning. 
//    /// </summary>
//    public void AdRewardDoubleBounty()
//    {
//        // Activate the flag
//        GetInstance()._adDoubleChallenge = true;
//        // Call AdManager to initiate the ad
//        AdManager.GetInstance().ShowRewardedVideo();
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the player watches an ad to play the challenge. 
//    /// </summary>
//    public void AdRewardInitChallenge()
//    {
//        // Activate the flag
//        GetInstance()._adChallengeInit = true;
//        // Calls AdManager to initiate Ad process
//        AdManager.GetInstance().ShowRewardedVideo();
//    }
//    */


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

//    #region LevelSelectionManagement
//    /*
//    /// <summary>
//    /// Sets the top text for the LevelSelection menu. Selects the right name 
//    /// from the collection of difficulty names and puts it in the Text.
//    /// </summary>
//    public void CreateTextLevelSelectionMenu()
//    {
//        // Search for the Text child
//        foreach (Transform t in GetInstance()._panelSuperior.transform)
//        {
//            // Set the corresponding text
//            if (t.name == "Difficulty")
//            {
//                t.GetComponent<Text>().text = getDifficultyText();
//            }
//        }
//    }
//    */

//    #endregion

//    #region SceneManagement
//    /*
//    /// <summary>
//    /// Function called by the Back buttons. Loads the last scene that was playing. 
//    /// </summary>
//    public void ReturnToLastScene()
//    {
//        // Check if the level is a challenge to change it's value
//        if (GetInstance()._challenge)
//        {
//            GetInstance()._challenge = false;
//        }
//        // Loads the last scene
//        SceneManager.LoadScene(GetInstance()._lastScene);
//    }
//    */


//    /*
//    /// <summary>
//    /// Return to the MainMenu Scene. 
//    /// </summary>
//    public void ReturnToMenu()
//    {
//        // Load MainMenu scene
//        SceneManager.LoadScene(0);
//    }
//    */


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

//        // Load Game scene
//        SceneManager.LoadScene(2);
//    }
//    */


//    /*
//    /// <summary>
//    /// Change scene to LevelSelection with a concrete difficulty to load it's information
//    /// </summary>
//    /// <param name="diff">Difficulty to be loaded</param>
//    public void ChangeToLevelSelection(int diff)
//    {
//        // Update instance's difficulty
//        GetInstance()._difficulty = diff;

//        // Update last Scene
//        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;

//        // Load LevelSelection scene
//        SceneManager.LoadScene(1);
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the player pays to play a challenge. Take coins from the player
//    /// current amount of them. Then, load the challenge.
//    /// </summary>
//    public void PaidChallenge()
//    {
//        // Take the coins from the player
//        if (GetInstance()._currentPlayerData._coinsPlayer >= GetInstance()._gc._challengeCost)
//        {
//            GetInstance()._currentPlayerData._coinsPlayer -= GetInstance()._gc._challengeCost;

//            InitChallenge();
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Init Game Scene with challenge configuration. Called when a challenge is going to be 
//    /// initiated. 
//    /// </summary>
//    public void InitChallenge()
//    {
//        // Store last scene index
//        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;
//        // Set challenge 
//        GetInstance()._challenge = true;
//        // Generate random difficulty and random level
//        GetInstance()._difficulty = Random.Range(0, GetInstance()._maxDifficulty);
//        GetInstance()._level = Random.Range(1, GetInstance()._levelsInDifficulty[GetInstance()._difficulty]);

//        // Load Game scene
//        SceneManager.LoadScene(2);
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when a challenge has ended and the player could not complete it.
//    /// </summary>
//    public void ChallengeFailed()
//    {
//        // Reset challenge value
//        GetInstance()._challenge = false;
//        // Set challenge completed for timers
//        GetInstance().ChallengeCompleted();
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the player completes a challenge successfully. 
//    /// </summary>
//    public void ChallengeWin()
//    {
//        // Update challenge value
//        GetInstance()._challenge = false;
//        // Update player information with the rewards
//        GetInstance()._currentPlayerData._coinsPlayer += GetInstance()._gc._challengeReward;
//        GetInstance()._currentPlayerData._challengesCompleted += 1;

//        GetInstance().ChallengeCompleted();
//    }
//    */
//    #endregion

//    #region Setters
//    /*
//    /// <summary>
//    /// Function called to set instance's LevelManager for future communication. 
//    /// </summary>
//    /// <param name="man">LevelManager to set</param>
//    public void setLevelManager(LevelManager man)
//    {
//        GetInstance()._lm = man;
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called to set the Main Menu instance for changing some things. 
//    /// </summary>
//    /// <param name="mm"></param>
//    public void setMainMenu(MainMenu mm)
//    {
//        GetInstance()._mm = mm;
//    }
//    */


//    /*
//    /// <summary>
//    /// Function called when the player pays for a Hint in a level. Takes the amount
//    /// of coins set in the editor as a hintprice. 
//    /// </summary>
//    public void CoinsUsed()
//    {
//        // Take the coins from the player
//        GetInstance()._currentPlayerData._coinsPlayer -= GetInstance()._gc._hintCost;
//    }
//    */


//    /*
//    /// <summary>
//    /// Sets the canvas of the scene as the instance's canvas for calculations and reference
//    /// resolutions. Reloads the information of the panels. 
//    /// </summary>
//    /// <param name="can">Canvas to set as Instance's canvas</param>
//    public void SetCanvas(Canvas can)
//    {
//        // Set canvas
//        GetInstance()._cnv = can;

//        // Reload the instance's panels for lates use
//        ReloadPanels();
//    }
//    */


//    /// <summary>
//    /// Set the camera of the scene as the instance's camera. 
//    /// </summary>
//    /// <param name="c">Camera to be set</param>
//    public void SetCamera(Camera c)
//    {
//        // Set the camera
//        GetInstance()._cam = c;
//    }


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


//    /// <summary>
//    /// Gives the screen resolution for lates use.
//    /// </summary>
//    /// <returns>Vector2 Screen resolution</returns>
//    public Vector2 getResolution()
//    {
//        return new Vector2(Screen.width, Screen.height);
//    }


//    /*
//    /// <summary>
//    /// Gives the text associated with a difficulty level.
//    /// </summary>
//    /// <returns>string Difficulty text</returns>
//    public string getDifficultyText()
//    {
//        return GetInstance()._gc._difficultyTexts[GetInstance()._difficulty];
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives the reference resolution used when scaling things for later use. 
//    /// </summary>
//    /// <returns>Vector2 Reference resolution</returns>
//    public Vector2 getReferenceResolution()
//    {
//        return GetInstance()._scalingReferenceResolution;
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives the value of the top panel height. 
//    /// </summary>
//    /// <returns>float Top panel height</returns>
//    public float panelSuperiorHeight()
//    {
//        return GetInstance()._panelSuperior.rect.height;
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives the value of the bottom panel height. 
//    /// </summary>
//    /// <returns>float Bottom panel height</returns>
//    public float panelInferiorHeight()
//    {
//        return GetInstance()._panelInferior.rect.height;
//    }
//    */


//    /// <summary>
//    /// Gives the actual difficulty of the game.
//    /// </summary>
//    /// <returns>int Actual difficulty</returns>
//    public int getDifficulty()
//    {
//        return GetInstance()._difficulty;
//    }

//    /// <summary>
//    /// Get the number of levels in a specific difficulty. 
//    /// </summary>
//    /// <param name="diff">Difficulty to access</param>
//    /// <returns>int Number of levels</returns>
//    public int getLevelsInDifficulty(int diff)
//    {
//        // Check if the difficulty to be accessed is registered
//        if (GetInstance()._difficulty < GetInstance()._levelsInDifficulty.Length)
//        {
//            // Return how many levels there are
//            return GetInstance()._levelsInDifficulty[diff];
//        }
//        else
//        {
//            // If not, inform
//            Debug.LogError("Difficulty level not defined");
//            return 0;
//        }
//    }

//    /// <summary>
//    /// Get actual level of the game
//    /// </summary>
//    /// <returns>int Actual level to be played</returns>
//    public int getLevel()
//    {
//        return GetInstance()._level;
//    }


//    /*
//    /// <summary>
//    /// Get the cost of a hint. 
//    /// </summary>
//    /// <returns>int Hintprice</returns>
//    public int getPrice()
//    {
//        return GetInstance()._gc._hintCost;
//    }
//    */


//    /*
//    // Info and prefab getters
//    /// <summary>
//    /// Get how many path skins are created for the game and included in the 
//    /// assetBundles
//    /// </summary>
//    /// <returns>int Number of skins</returns>
//    public int getNumPathSkins()
//    {
//        return GetInstance()._gi._numPathSkins;
//    }
//    */


//    /*
//    /// <summary>
//    /// Get how many Touch skins are for the game. 
//    /// </summary>
//    /// <returns>int Number of Touch skins</returns>
//    public int getNumTouchSkins()
//    {
//        return GetInstance()._gi._numTouchSkins;
//    }
//    */


//    /*
//    /// <summary>
//    /// Get how many Tile skins there are and use them for random generation.
//    /// </summary>
//    /// <returns>int Num of skins</returns>
//    public int getNumTileSkins()
//    {
//        return GetInstance()._gi._numTileSkins;
//    }
//    */


//    /*
//    /// <summary>
//    /// Loads a tile prefab from the AssetBundles and returns it to be used later in the game. 
//    /// </summary>
//    /// <param name="name">Prefab to be loaded</param>
//    /// <returns>GameObject Prefab of the tile chosen</returns>
//    public GameObject getPrefabFromTileAssetBundle(string name)
//    {
//        return GetInstance()._lab.getTileSkins().LoadAsset<GameObject>(name);
//    }
//    */


//    /*
//    /// <summary>
//    /// Loads a path prefab from the AssetBundle and returns it to be used in the game. 
//    /// </summary>
//    /// <param name="name">Prefab to be loaded</param>
//    /// <returns>GameObject Prefab of the path skin chosen</returns>
//    public GameObject getPrefabFromPathAssetBundle(string name)
//    {
//        return GetInstance()._lab.getPathSkins().LoadAsset<GameObject>(name);
//    }
//    */


//    /*
//    /// <summary>
//    /// Loads a touch prefab from the AssetBundle and returns it to use in the game. 
//    /// </summary>
//    /// <param name="name">Prefab to be loaded</param>
//    /// <returns>GameObject Prefab of the touch skin chosen</returns>
//    public GameObject getPrefabFromTouchAssetBundle(string name)
//    {
//        return GetInstance()._lab.getTouchSkins().LoadAsset<GameObject>(name);
//    }
//    */


//    // Challenge
//    /// <summary>
//    /// Function that gives information about the challenge status. Used for intializing 
//    /// a challenge. 
//    /// </summary>
//    /// <returns>bool Challenge status</returns>
//    public bool getChallenge()
//    {
//        return GetInstance()._challenge;
//    }


//    /*
//    /// <summary>
//    /// Gives the number of minutes that the player has to complete the challenge. 
//    /// </summary>
//    /// <returns>int Time in minutes</returns>
//    public int getChallengeTime()
//    {
//        return GetInstance()._gc._challengeWaitTime;
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives the value of the reward after winning a challenge. 
//    /// </summary>
//    /// <returns>int Challenge reward value</returns>
//    public int getChallengeReward()
//    {
//        return GetInstance()._gc._challengeReward;
//    }
//    */


//    /*
//    /// <summary>
//    /// Amount of coins to pay in order to enter a challenge. 
//    /// </summary>
//    /// <returns>int Coins needed to play</returns>
//    public int getChallengePrice()
//    {
//        return GetInstance()._gc._challengeCost;
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives the time remaining for the player to play a challenge. 
//    /// </summary>
//    /// <returns>float Time remaining in seconds</returns>
//    public float getTimeRemaining()
//    {
//        return GetInstance()._timeChallengeWait;
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives the status of the challenge. If the player played a challenge
//    /// recently, they must wait for time to end.
//    /// </summary>
//    /// <returns></returns>
//    public bool getChallengeWaiting()
//    {
//        return GetInstance()._challengeWaiting;
//    }
//    */


//    /*
//    // Player stats getters
//    /// <summary>
//    /// Gives how many coins does the player have. 
//    /// </summary>
//    /// <returns>int Amount of coins the player still has</returns>
//    public int getPlayerCoins()
//    {
//        return GetInstance()._currentPlayerData._coinsPlayer;
//    }
//    */


//    /*
//    /// <summary>
//    /// Gives how many levels has the player completed in some difficulty. 
//    /// </summary>
//    /// <param name="difficulty">Difficulty to be checked</param>
//    /// <returns>int Number of levels completed in that difficulty</returns>
//    public int getCompletedLevelsInDifficulty(int difficulty)
//    {
//        // Check if the difficulty is correct
//        if (difficulty < GetInstance()._currentPlayerData._completedLevelsInDifficulty.Length)
//        {
//            return GetInstance()._currentPlayerData._completedLevelsInDifficulty[difficulty];
//        }
//        else
//        {
//            // If not, informs
//            Debug.LogError("Difficulty level not defined");
//            return 0;
//        }
//    }
//    */


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

//    #region ApplicationLifeManagement
//    /// <summary>
//    /// Function called by buttons to quit application.
//    /// </summary>
//    public void ExitGame()
//    {
//        Application.Quit();
//    }


//    /*
//    /// <summary>
//    /// Checks if the app has the focus and reloads all the info about the 
//    /// player and the times in case it's needed. 
//    /// </summary>
//    /// <param name="focus"></param>
//    private void OnApplicationFocus(bool focus)
//    {
//        isPaused = !focus;

//        if (focus)
//        {
//            // Get Player information and store it
//            GetInstance()._currentPlayerData = LoadingFiles.ReadPlayerData(GetInstance()._maxDifficulty);

//            // Set the time waiting for challenge info
//            SetTimeForChallenge();
//        }
//        else
//        {
//            // Update _currentPlayerData 
//            if (GetInstance()._challengeWaiting)
//            {
//                GetInstance()._currentPlayerData._dateForNextChallenge = (int)(ConvertDateToSecond() + GetInstance()._timeChallengeWait);
//            }
//            // Save player information
//            LoadingFiles.SavePlayerData(GetInstance()._currentPlayerData);
//        }
//    }
//    */


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

//        // Save player information
//        LoadingFiles.SavePlayerData(GetInstance()._currentPlayerData);
//    }
//    */


//    /*
//    /// <summary>
//    /// Function that will manage the close of the app, saving the player's current status.
//    /// </summary>
//    private void OnApplicationQuit()
//    {
//        // Update _currentPlayerData 
//        if (GetInstance()._challengeWaiting)
//        {
//            GetInstance()._currentPlayerData._dateForNextChallenge = (int)(ConvertDateToSecond() + GetInstance()._timeChallengeWait);
//        }

//        // Save player information
//        LoadingFiles.SavePlayerData(GetInstance()._currentPlayerData);
//    }
//    */
//    #endregion
//}