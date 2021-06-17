using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Class that manages the Level and shows all the 
/// different panels to the player, as well as creates 
/// the board.
/// 
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Tooltip("Board Manager object")]
    public BoardManager _boardManager;            // BoardManager instance
    public Canvas _canvas;                        // Canvas of the scene
    public Camera _camera;                        // Camera 
    public RectTransform _topPanel;               // Top panel of canvas
    public RectTransform _botPanel;               // Bottom panel of canvas
    public GameObject _endPanel;                  // End panel of the level

    [Header("Texts and data settings")]
    public Text _levelAndMode;                    // Level and mode text
    public Text _hints;                           // Hints text
    public Text _hinstPanel;                      // Hints panel to confirm usage

    [Header("Options panels")]
    public GameObject _pausePanel;                // Pause panel GO
    public GameObject _restorePanel;              // Restore panel GO
    public GameObject _hintsPanel;                // Hints panel GO
    public GameObject _optionsPanel;              // Options panel GO
    public GameObject _storePanel;                // Store panel GO
    public GameObject _completeHintsPanel;        // Complete usage of hints GO
    public Button _homeEndedButton;               // Home button Level complete panel
    public Button _homePauseButton;               // Home button level paused

    private bool _paused = false;                 // Pause flag for Input control


    // ----------------------------------------------
    // --------------- UNITY METHODS ----------------
    // ----------------------------------------------

    void Awake()
    {
        if (_boardManager == null)
        {
            Debug.LogError("Board Manager reference not set");
        } // if
        if (_endPanel == null)
        {
            Debug.LogError("End panel reference not set");
        } // if
        else
        {
            _boardManager.Init(this);
        } // else
    } // Awake

    private void Start()
    {
        GameManager.GetInstance().SetCamera(_camera);
        GameManager.GetInstance().SetCanvas(_canvas);

        _homePauseButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);
        _homeEndedButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);

        PlayLevel();
    } // Start

    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------- PUBLIC -------------------
    /// <summary>
    /// 
    /// Shows the ending panel.
    ///
    /// </summary>
    public void ShowEndMenu()
    {
        _endPanel.SetActive(true);
        _paused = true;
    } // ShowEndMenu

    /// <summary>
    /// 
    /// Sets all the scene to play the next level. 
    /// 
    /// </summary>
    public void PlayLevel()
    {
        // Set info
        AdManager.GetInstance().ShowVideo();
        _levelAndMode.text = GameManager.GetInstance().GetPackageName() +
               " - " + GameManager.GetInstance().GetLevel().ToString();
        _hints.text = _hinstPanel.text = GameManager.GetInstance().GetPlayerData()._hints.ToString();

        // Deactivate all unnecessary panels
        _pausePanel.SetActive(false);
        _restorePanel.SetActive(false);
        _hintsPanel.SetActive(false);
        _storePanel.SetActive(false);
        _completeHintsPanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _endPanel.SetActive(false);

        // Prepare board
        LevelPackage lp = GameManager.GetInstance().GetLevelPackage();
        int level = GameManager.GetInstance().GetLevel();

        Map map = Map.FromJson(lp.levels[level].ToString());

        _boardManager.EmptyBoard();
        _boardManager.SetMap(map);
    } // PlayLevel

    /// <summary>
    /// 
    /// Update the hints texts with the new information (bought hints).
    /// 
    /// </summary>
    public void UpdateTexts()
    {
        _hints.text = _hinstPanel.text = GameManager.GetInstance().GetPlayerData()._hints.ToString();
    } // UpdateTexts

    /// <summary>
    /// 
    /// Receive new Input and process it. 
    /// 
    /// </summary>
    /// <param name="it"> (InputType) Type of new input. </param>
    public void ReceiveInput(InputManager.InputType it)
    {
        if (!_paused)
        {
            _boardManager.ReceiveInput(it);
        } // if
    } // ReceiveInput

    /// <summary>
    /// 
    /// Call to pause the game, activates the necessary panels.
    /// 
    /// </summary>
    public void PauseGame()
    {
        _optionsPanel.SetActive(true);
        _pausePanel.SetActive(true);
        _paused = true;
    } // PauseGame

    /// <summary>
    /// 
    /// Resume game. 
    /// 
    /// </summary>
    public void UnPauseGame()
    {
        _pausePanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    /// <summary>
    /// 
    /// Show restore panel option.
    /// 
    /// </summary>
    public void RestoreGameScreen()
    {
        _optionsPanel.SetActive(true);
        _restorePanel.SetActive(true);
        _paused = true;
    } // PauseGame

    /// <summary>
    /// 
    /// Deactivate restore panel. 
    /// 
    /// </summary>
    public void UnRestoreGameScreen()
    {
        _restorePanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _paused = false;
    } // UnPauseGame

    /// <summary>
    /// 
    /// Show the hints screen. 
    /// 
    /// </summary>
    public void HintsScreen()
    {
        _optionsPanel.SetActive(true);
        _hintsPanel.SetActive(true);
        _paused = true;
    } // PauseGame

    /// <summary>
    /// 
    /// Hide the Hints' Screen.
    /// 
    /// </summary>
    public void UnHintsScreen()
    {
        _hintsPanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _paused = false;
        UpdateTexts();
    } // UnPauseGame

    /// <summary>
    /// 
    /// Show store screen.
    /// 
    /// </summary>
    public void StoreScreen()
    {
        _optionsPanel.SetActive(true);
        _storePanel.SetActive(true);
        _paused = true;

    } // StoreScreen

    /// <summary>
    /// 
    /// Hide storescreen. 
    /// 
    /// </summary>
    public void UnStoreScreen()
    {
        _storePanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _paused = false;
        UpdateTexts();
    } // UnStoreScreen

    /// <summary>
    /// 
    /// Show the panel that notifies when all hints have been used.
    /// 
    /// </summary>
    public void HintsCompletedScreen()
    {
        _optionsPanel.SetActive(true);
        _completeHintsPanel.SetActive(true);
        _paused = true;
    } // HintsCompletedScreen

    /// <summary>
    /// 
    /// Hide the hints completed panel.
    /// 
    /// </summary>
    public void UnHintsCompletedScreen()
    {
        _completeHintsPanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _paused = false;
    } // UnHintsCompletedScreen
} // LevelManager
