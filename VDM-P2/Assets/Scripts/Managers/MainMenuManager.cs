using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Class that manages the main menu, showing the necessary information. 
/// 
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Configuration")]
    [Header("Objects")]
    public Canvas _canvas;                      // Canvas of the scene
    public Text _categoriesText;                // Category text for category selection
    public Text _levelPlayer;                   // Level of the player text
    public Camera _sceneCamera;                 // Camera
    public RectTransform _buttonZone;           // Zone for the categories buttons
    public RectTransform _columns;              // Zone to spawn buttons
    public RectTransform _button;               // Rectangle of the buttons
    public GameObject _hintsMessagePanel;       // Hints message panel

    [Header("Panels")] 
    public GameObject _topPanel;                // TopPanel
    public GameObject _bottomPanel;             // BottomPanel
    public GameObject _centerPanel;             // Centerpanel

    [Header("Options")]
    public GameObject _optionsAndStore;         // OptionsPanel
    public ButtonToggleSprite _settingsButton;  // Settings buttons
    public ButtonToggleSprite _storeButton;     // Store button
    public GameObject _optionsPanel;            // Options panel
    public GameObject _storePanel;              // Store panel

    [Header("Limits and Info")]
    public int _topLimit = 5;                   // Top panel
    public int _bottomLimit = 10;               // Bottom panel
    public int _leftLimit = 10;                 // Left limit
    public int _rightLimit = 10;                // Right limit for buttons
    public int _spaceButtons = 12;              // Space between buttons

    // Start is called before the first frame update
    void Start()
    {
        _optionsAndStore.SetActive(true);

        // Set this scene camera as the GameManager's camera
        GameManager.GetInstance().SetCamera(_sceneCamera);
        GameManager.GetInstance().SetMainMenuManager(this);

        _buttonZone.gameObject.SetActive(false);
        _categoriesText.gameObject.SetActive(false);
        _hintsMessagePanel.SetActive(false);
        _optionsAndStore.SetActive(false);

        // First get level package for loading information
        _levelPlayer.text = Math.Floor(GameManager.GetInstance().GetPlayerData()._playerLevel).ToString();        
    } // Start

    #region SettingsAndStorePanelChange

    /// <summary>
    /// 
    /// Deactivates necessary panels.
    /// 
    /// </summary>
    public void GoToSettingsStore()
    {
        _topPanel.SetActive(false);
        _centerPanel.SetActive(false);
        _bottomPanel.SetActive(false);
    }

    /// <summary>
    /// 
    /// Open the Settings screen. 
    /// 
    /// </summary>
    public void Configuration()
    {
        GoToSettingsStore();
        _optionsAndStore.SetActive(true);
        _optionsPanel.SetActive(true);
        _storePanel.SetActive(false);
        _settingsButton.TaskOnClick();
    } // Configuration

    /// <summary>
    /// 
    /// Open the store screen.
    /// 
    /// </summary>
    public void Store()
    {
        GoToSettingsStore();
        _optionsAndStore.SetActive(true);
        _optionsPanel.SetActive(false);
        _storePanel.SetActive(true);
        _storeButton.TaskOnClick();
    } // Store

    /// <summary>
    /// 
    /// Shows the Config panel.
    /// 
    /// </summary>
    public void ChangeToConfig()
    {
        _optionsPanel.SetActive(true);
        _storePanel.SetActive(false);
        if (_storeButton.IsPressed())
        {
            _storeButton.TaskOnClick();
        } // if
    } // ChangeToConfig

    /// <summary>
    /// 
    /// Show the store screen. 
    /// 
    /// </summary>
    public void ChangeToStore()
    {
        _optionsPanel.SetActive(false);
        _storePanel.SetActive(true);
        if (_settingsButton.IsPressed())
        {
            _settingsButton.TaskOnClick();
        } // if
    } // ChangeToStore

    /// <summary>
    /// 
    /// Return to the main screen with the play button.
    /// 
    /// </summary>
    public void Backwards()
    {
        // Make sure buttons are nor pressed
        if (_storeButton.IsPressed())
        {
            _storeButton.TaskOnClick();
        } // if

        if (_settingsButton.IsPressed())
        {
            _settingsButton.TaskOnClick();
        } // if

        _optionsAndStore.SetActive(false);

        _topPanel.SetActive(true);
        _centerPanel.SetActive(true);
        _bottomPanel.SetActive(true);
    } // Backwards
    #endregion

    #region AdResults

    /// <summary>
    /// 
    /// Called when a rewarded ad is successful. Shows a panel.
    /// 
    /// </summary>
    public void AdCompleted()
    {
        _hintsMessagePanel.SetActive(true);
        //_hintsMessage.Play("hints_window");
    } // AdCompleted

    /// <summary>
    /// 
    /// Hides the panel of the result.
    /// 
    /// </summary>
    public void MessageExit()
    {
        _hintsMessagePanel.SetActive(false);
    } // MessageExit
    #endregion

    /// <summary>
    /// 
    /// Method that instanciates all buttons like the Levelmanager
    /// instantiates the level buttons. Is called by the button 
    /// "Play" in the Scene. 
    /// 
    /// </summary>
    public void Play()
    {
        // Deactivate all innecessary objects
        _bottomPanel.SetActive(false);
        _centerPanel.SetActive(false);

        // Activate the text at the top of the screen
        _buttonZone.gameObject.SetActive(true);
        _categoriesText.gameObject.SetActive(true);
        _categoriesText.text = "Categories";

        // Create and instantiate buttons
        int buttons = GameManager.GetInstance().GetNumPackages();

        // Calculate height and resize
        float origHeight = _buttonZone.rect.height;
        float newHeight = (_button.rect.height * buttons) + ((_spaceButtons + _topLimit + _bottomLimit) * buttons);
        _buttonZone.sizeDelta = new Vector2(_buttonZone.rect.width, newHeight);

        float posY = _buttonZone.position.y;
        float testing = (posY * newHeight) / origHeight;

        // Resize with screen
        newHeight = (newHeight / GameManager.GetInstance().GetScaling().TransformationFactor()) / 2;
        origHeight = (origHeight / GameManager.GetInstance().GetScaling().TransformationFactor()) / 2;

        InstantiateButtons(buttons);
    } // Player

    /// <summary>
    /// 
    /// Instantiate all buttons (equal to the number of packages registered).
    /// 
    /// </summary>
    /// <param name="b"> (int) Number of packages. </param>
    void InstantiateButtons(int b)
    {
        GameObject button;

        _columns.GetComponent<VerticalLayoutGroup>().padding.top = _topLimit;
        _columns.GetComponent<VerticalLayoutGroup>().padding.bottom = _bottomLimit;
        _columns.GetComponent<VerticalLayoutGroup>().padding.left = _leftLimit;
        _columns.GetComponent<VerticalLayoutGroup>().padding.right = _rightLimit;
        _columns.GetComponent<VerticalLayoutGroup>().spacing = _spaceButtons;

        for (int i = 0; i < b; i++)
        {
            button = Instantiate(_button.gameObject, _columns.transform);

            SetButton(button, i);
        } // for
    } // InstantiateButtons

    /// <summary>
    /// 
    /// Sets the button to work as intended. This chooses 
    /// between ads and normal buttons.
    /// 
    /// </summary>
    /// <param name="b"> (GameObject) Button to set. </param>
    /// <param name="package"> (int) Current package. </param>
    void SetButton(GameObject b, int package)
    {
        LevelPackage lp = GameManager.GetInstance().GetLevelPackage(package);

        if (lp.name != "ad")
        {
            b.GetComponent<PackageSelectionButton>().SetPackageName(lp.name);

            float completedLevels = (float)GameManager.GetInstance().GetPlayerData()._completedLevelsPackage[lp.name];
            float nLevels = lp.levels.Length;
            int percentage = (int)((completedLevels / nLevels) * 100);

            b.GetComponent<PackageSelectionButton>().SetPercentage(percentage);
            b.GetComponent<PackageSelectionButton>().SetNormalButton();
        } // if
        else
        {
            b.GetComponent<PackageSelectionButton>().SetAd(lp.levels[0].ToString());
        } // else

        b.GetComponent<PackageSelectionButton>().SetSprites(lp.buttonUp, lp.buttonDown);
    } // SetButton
} // MainMenuManager
