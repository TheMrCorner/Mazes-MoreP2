using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
/// GameManager class. Manages all changes between scenes, the levels that will be
/// played and the information of the player. 
/// 
/// Has all the information about the player. Manages the changes in their stats
/// and saves it when the app is closed or when it loses focus.
/// 
/// Has the instance of the different level packages, including the ads. 
/// 
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables
    /// </summary>
    [Header("Resolution Configuration")]
    public Canvas _cnv;                           // Canvas of the scene
    public Camera _cam;                           // Camera of the scene

    [Header("Levels")]
    public LevelManager _levelManager = null;     // LevelManager for level
    public LevelPackage[] _levels;                // Array of LevelPackages

    [Header("Debugging")]
    public bool _debugging = false;               // Sets if debug mode is on, for avoiding some changes


    private string _package = "Classic";          // Sets game difficulty
    private int _level = 99;                      // Sets the level to be loaded

    private int _playerLevel = 0;

    // SCALING DATA
    private Vector2 _scalingReferenceResolution;  // Reference resolution for scaling
    private Scaling _scalator;                    // Scaling object

    // GAME/SCENE MANAGEMENT
    private PlayerData _player;                   // Player data
    private RectTransform _topPanel;              // Top panel of the canvas
    private RectTransform _bottomPanel;           // Bottom panel of the canvas
    private MainMenuManager _mainMenu;            // MainMenuManager to change things and update data
    private int _lastScene;                       // Last scene to return to it if necessary

    #endregion

    #region StartUpGameManager
    /// <summary>
    /// 
    /// Variable that stores the instance of the GameManager, Singleton
    /// 
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// 
    /// Awake function of GameManager. Checks if another instance of this GameObject exists and 
    /// if not, initializes all required atributes and values of the GameManager, creating a new
    /// one. 
    /// 
    /// If the GameManager already exists, destroy this gameObject. 
    /// 
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

            // Get Player information and store it
            _player = FileLoader.ReadPlayerData(packagesNames);

            DontDestroyOnLoad(_instance);
        } // if
        else if (_instance != this)
        {
            _instance._levelManager = _levelManager;

            Destroy(gameObject);
        } // else if
    } // Awake


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
    /// <summary>
    /// 
    /// Function to calculate the level of the player.
    /// 
    /// </summary>
    /// <param name="completedLevels"> (int) Total completed levels. </param>
    /// <returns> (int) Level of the player. </returns>
    int XPFunction(int completedLevels)
    {
        float playerLevel = Mathf.Pow((completedLevels / 3), 0.75f);

        return Mathf.FloorToInt(playerLevel);
    } // XPFunction

    /// <summary>
    /// 
    /// Reverse of the function to calculate the level.
    /// 
    /// </summary>
    /// <param name="playerLevel"> (int) Player level. </param>
    /// <returns> (int) Completed levels. </returns>
    int ReverseXPFunction(int playerLevel)
    {
        float completedLevels = 3 * Mathf.Pow(playerLevel, 1.35f);

        return Mathf.RoundToInt(completedLevels);
    } // ReverseXPFunction
    
    /// <summary>
    /// 
    /// Calculates the current playerlevel.
    /// 
    /// </summary>
    /// <returns></returns>
    public int CalculatePlayerLevel()
    {
        int completedLevels = CalculateCompletedLevels();
        _playerLevel = XPFunction(completedLevels);
        return _playerLevel;
    } // CalculatePlayerLevel

    /// <summary>
    /// 
    /// Calculate percentage of levels completedlevels.
    /// 
    /// </summary>
    /// <returns> (float) Percentage of levels compelted. </returns>
    public float PercentageLevelCompleted()
    {
        int completedLevels = CalculateCompletedLevels();
        int currentPlayerLevel = XPFunction(completedLevels);
        int levelsToNext = ReverseXPFunction(currentPlayerLevel + 1);
        int levelsToCurrent = ReverseXPFunction(currentPlayerLevel);

        return ((float)(completedLevels) - levelsToCurrent) / (levelsToNext - levelsToCurrent);
    }// PercentageLevelCompleted

    /// <summary>
    /// 
    /// Calculate completed levels necessary for level.
    /// 
    /// </summary>
    /// <returns> (int) Levels for level. </returns>
    int CalculateCompletedLevels ()
    {
        int xp = 0;

        foreach (var completedLevels in _player._completedLevelsPackage)
        {
            xp += completedLevels.Value;
        }

        return xp;
    } // CalculateCompletedLevels.

    /// <summary>
    /// 
    /// Function called when the Level is completed. Updates the level and
    /// calls the level manager so it shows the end panel.
    /// 
    /// </summary>
    public void LevelCompleted()
    {
        _level++;
        if (_level < GameManager.GetInstance().GetLevelPackage().levels.Length)
        {
            if (GetInstance()._player._completedLevelsPackage[_package] <= _level)
                GetInstance()._player._completedLevelsPackage[_package]++;
            _levelManager.ShowEndMenu();
        }
        else
        {
            _levelManager.ShowFinalMenu();
        }
        
    } // LevelCompleted

    /// <summary>
    /// 
    /// Function called when the buying hints option is used.
    /// 
    /// </summary>
    /// <param name="hints"> (int) Number of hints bought. </param>
    public void AddHints(int hints)
    {
        GetInstance()._player._hints += hints;
        if(_levelManager != null)
            _levelManager.UpdateTexts();
    } // AddHints

    /// <summary>
    /// 
    /// Function called when the removing ads purchase is completed.
    /// 
    /// </summary>
    public void RemoveAds()
    {
        GetInstance()._player._adsRemoved = true;
    } // RemoveAds

    /// <summary>
    /// 
    /// Function called when a hint is used. 
    /// 
    /// </summary>
    public void HintShown()
    {
        GetInstance()._player._hints--;

        if (_levelManager != null)
        {
            _levelManager.UpdateTexts();
        } // if
    } // HintShown

    /// <summary>
    /// 
    /// Function called when a rewarded ad ended successfully.
    /// 
    /// </summary>
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

    #region SceneManagement
    /// <summary>
    /// 
    /// Loads the last scene that was playing. 
    /// 
    /// </summary>
    public void ReturnToLastScene()
    {
        // Loads the last scene
        SceneManager.LoadScene(GetInstance()._lastScene);
    } // ReturnToLastScene


    /// <summary>
    /// 
    /// Return to the MainMenu Scene. 
    /// 
    /// </summary>
    public void ReturnToMenu()
    {
        // Load MainMenu scene
        SceneManager.LoadScene(0);
    } // ReturnToMenu


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

    /// <summary>
    /// 
    /// Change to GameScene and play level.
    /// 
    /// </summary>
    public void ChangeToLevel()
    {
        // Update last Scene
        GetInstance()._lastScene = SceneManager.GetActiveScene().buildIndex;

        // Load LevelSelection scene
        SceneManager.LoadScene(2);
    } // ChangeToLevel
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

    /// <summary>
    /// 
    /// Set the current canvas. 
    /// 
    /// </summary>
    /// <param name="c"> (Canvas) Canvas of the scene. </param>
    public void SetCanvas(Canvas c)
    {
        GetInstance()._cnv = c;
        ReloadPanels();
    } // SetCanvas

    /// <summary>
    /// 
    /// Sets the package selected by the player.
    /// 
    /// </summary>
    /// <param name="p"> (string) Package selected. </param>
    public void SetPackage(string p)
    {
        GetInstance()._package = p;
    } // SetPackage

    /// <summary>
    /// 
    /// Sets the Level selected by the player.
    /// 
    /// </summary>
    /// <param name="i"> (int) Level selected. </param>
    public void SetLevel(int i)
    {
        GetInstance()._level = i;
    } // SetLevel

    /// <summary>
    /// 
    /// Sets the MainMenuManager.
    /// 
    /// </summary>
    /// <param name="mg"> (MainMenuManager) Current Main menu. </param>
    public void SetMainMenuManager(MainMenuManager mg)
    {
        GetInstance()._mainMenu = mg;
    } // SetMainMenuManager
    #endregion

    #region Getters

    /// <summary>
    /// 
    /// Gives access to the scalator instance.
    /// 
    /// </summary>
    /// <returns> (Scaling) Scaling instance stored in GM instance. </returns>
    public Scaling GetScaling()
    {
        return GetInstance()._scalator;
    } // GetScaling

    /// <summary>
    /// 
    /// Gives access to the canvas stored in instance.
    /// 
    /// </summary>
    /// <returns> (Canvas) Canvas access. </returns>
    public Canvas GetCanvas()
    {
        return GetInstance()._cnv;
    } // GetCanvas

    /// <summary>
    /// 
    /// Returns the height of the top panel.
    /// 
    /// </summary>
    /// <returns> (float) Panel height. </returns>
    public float GetTopPanelHeight()
    {
        return GetInstance()._topPanel.rect.height;
    } // GetTopPanelHeight

    /// <summary>
    /// 
    /// Returns the height of the bottom panel in pixels.
    /// 
    /// </summary>
    /// <returns> (float) Height of panel </returns>
    public float GetBottomPanelHeight()
    {
        return GetInstance()._bottomPanel.rect.height;
    } // GetTopPanelHeight

    /// <summary>
    /// 
    /// Gives the reference resolution used when scaling things for later use. 
    /// 
    /// </summary>
    /// <returns> (Vector2) Reference resolution. </returns>
    public Vector2 GetReferenceResolution()
    {
        return GetInstance()._scalingReferenceResolution;
    } // GetReferenceResolution

    /// <summary>
    /// 
    /// Gives the number of packages registered in the game.
    /// 
    /// </summary>
    /// <returns> (int) Number of packages. </returns>
    public int GetNumPackages()
    {
        return GetInstance()._levels.Length;
    } // GetNumPackages

    /// <summary>
    /// 
    /// Get the name of the current package selected.
    /// 
    /// </summary>
    /// <returns> (string) Package name.</returns>
    public string GetPackageName()
    {
        return GetInstance()._package;
    } // GetPackageName

    /// <summary>
    /// 
    /// Gives access to a level package selected
    /// by number, necessary for button instantiation.
    /// 
    /// </summary>
    /// <param name="i"> (int) Package to access. </param>
    /// <returns> (LevelPackage) Package with data. </returns>
    public LevelPackage GetLevelPackage(int i)
    {
        return GetInstance()._levels[i];
    } // GetPackage

    /// <summary>
    /// 
    /// Gives access to the actual level selected.
    /// 
    /// </summary>
    /// <returns> (int) Current level. </returns>
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
    /// 
    /// Eventhough it's not an applifemanagement method, here are all
    /// the methods that interact with external data. This method opens
    /// a browser window with the provided link.
    /// 
    /// </summary>
    /// <param name="link"> (string) Link. </param>
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    } // OpenLink

    /// <summary>
    /// 
    /// Function that will manage the close of the app, saving the player's current status. Not
    /// working in mobile.
    /// 
    /// </summary>
    private void OnApplicationQuit()
    {
        // Save player information
        FileLoader.SavePlayerData(GetInstance()._player);
    } // OnApplicationQuit

    /// <summary>
    /// 
    /// Save data also when application loses focus, to avoid
    /// losing data and etc.
    /// 
    /// </summary>
    /// <param name="focus"> (bool) Focus status. </param>
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            // Save player information
            FileLoader.SavePlayerData(GetInstance()._player);
        } // if
    } // OnApplicationFocus

    #endregion
}