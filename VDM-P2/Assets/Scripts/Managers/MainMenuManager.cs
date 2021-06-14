using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuration")]
    [Header("Objects")]
    public Canvas _canvas;
    public Text _categoriesText;
    public Text _levelPlayer;
    public Camera _sceneCamera;
    public RectTransform _buttonZone;
    public RectTransform _columns;
    public RectTransform _button;

    [Header("Panels")]
    public GameObject _topPanel;
    public GameObject _bottomPanel;
    public GameObject _centerPanel;

    [Header("Options")]
    public GameObject _optionsAndStore;
    public ButtonToggleSprite _settingsButton;
    public ButtonToggleSprite _storeButton;
    public GameObject _optionsPanel;
    public GameObject _storePanel;

    [Header("Limits and Info")]
    public int _topLimit = 5;
    public int _bottomLimit = 10;
    public int _leftLimit = 10;
    public int _rightLimit = 10;
    public int _spaceButtons = 12;

    // Start is called before the first frame update
    void Start()
    {
        // Set this scene camera as the GameManager's camera
        GameManager.GetInstance().SetCamera(_sceneCamera);

        _buttonZone.gameObject.SetActive(false);

        _categoriesText.gameObject.SetActive(false);

        // First get level package for loading information
        _levelPlayer.text = Math.Floor(GameManager.GetInstance().GetPlayerData()._playerLevel).ToString();        
    } // Start

    #region SettingsAndStorePanelChange

    public void GoToSettingsStore()
    {
        _topPanel.SetActive(false);
        _centerPanel.SetActive(false);
        _bottomPanel.SetActive(false);
    }

    public void Configuration()
    {
        GoToSettingsStore();
        _optionsAndStore.SetActive(true);
        _optionsPanel.SetActive(true);
        _storePanel.SetActive(false);
        _settingsButton.TaskOnClick();
    } // Configuration

    public void Store()
    {
        GoToSettingsStore();
        _optionsAndStore.SetActive(true);
        _optionsPanel.SetActive(false);
        _storePanel.SetActive(true);
        _storeButton.TaskOnClick();
    } // Store

    public void ChangeToConfig()
    {
        _optionsPanel.SetActive(true);
        _storePanel.SetActive(false);
        if (_storeButton.IsPressed())
        {
            _storeButton.TaskOnClick();
        } // if
    } // ChangeToConfig

    public void ChangeToStore()
    {
        _optionsPanel.SetActive(false);
        _storePanel.SetActive(true);
        if (_settingsButton.IsPressed())
        {
            _settingsButton.TaskOnClick();
        } // if
    } // ChangeToStore

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

    void SetButton(GameObject b, int package)
    {
        LevelPackage lp = GameManager.GetInstance().GetLevelPackage(package);

        b.GetComponent<PackageSelectionButton>().SetPackageName(lp.name);

        float completedLevels = (float)GameManager.GetInstance().GetPlayerData()._completedLevelsPackage[lp.name];
        float nLevels = lp.levels.Length;
        int percentage = (int)((completedLevels / nLevels) * 100);

        Debug.Log(percentage + "%");

        b.GetComponent<PackageSelectionButton>().SetPercentage(percentage);
        b.GetComponent<PackageSelectionButton>().SetSprites(lp.buttonUp, lp.buttonDown);
    } // SetButton
} // MainMenuManager
