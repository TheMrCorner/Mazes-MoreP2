using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Configuration")]
    [Header("Objects")]
    public Canvas _canvas;                          // Canvas of the menu
    public Text _packageName;                       // Package name to display
    public Text _levelPlayer;                       // Player level text to display
    public Camera _sceneCamera;                     // Camera of the scene 
    public GameObject _levelSelectionButton;        // Prefab of the button
    public RectTransform _buttonZone;               // Empty rectangle to put buttons
    public RectTransform _columns;                  // Rectangle to manage columns
    public RectTransform _raws;                     // Rectangle to manage raws

    [Header("Limits and info")]
    public int _levelsCompleted = 0;                // Number of levels completed
    public int _btnsRaw = 5;                        // Number of buttons per raw
    public int _topLimit = 5;                       // Distance to the top
    public int _bottomLimit = 10;                   // Distance to bottom
    public int _spaceRaws = 12;                     // Space between raws

    private int _currentButton;                             // Number of buttons in this bundle
    private LevelPackage _lp;                               // Current level package

    // Start is called before the first frame update
    void Start()
    {
        // Set this scene camera as the GameManager's camera
        GameManager.GetInstance().SetCamera(_sceneCamera);

        // First get level package for loading information
        _lp = GameManager.GetInstance().GetLevelPackage();

        _levelsCompleted = GameManager.GetInstance().GetPlayerData()._completedLevelsPackage[_lp.name];

        // Number of raws
        int raws = _lp.levels.Length / _btnsRaw;

        // Calculate height and resize
        float origHeight = _buttonZone.rect.height;
        float newHeight = (_raws.rect.height * raws) + ((_spaceRaws + _topLimit + _bottomLimit) * raws);
        _buttonZone.sizeDelta = new Vector2(_buttonZone.rect.width, newHeight);

        float posY = _buttonZone.position.y;
        float testing = (posY * newHeight) / origHeight;

        // Resize with screen
        newHeight = (newHeight / GameManager.GetInstance().GetScaling().TransformationFactor()) / 2;
        origHeight = (origHeight / GameManager.GetInstance().GetScaling().TransformationFactor()) / 2;

        _packageName.text = FormatPackageName(_lp.name);

        _levelPlayer.text = Math.Floor(GameManager.GetInstance().GetPlayerData()._playerLevel).ToString();
        // Instantiate all
        InstantiateRaws(raws);
    }

    private string FormatPackageName(string n)
    {
        StringBuilder name = new StringBuilder(n.Length * 2);
        name.Append(n[0]);

        for (int i = 1; i < n.Length; i++)
        {
            if(char.IsUpper(n[i]) && char.IsLower(n[i - 1]))
            {
                name.Append(' ');
            } // if

            name.Append(n[i]);
        } // for

        return name.ToString();
    } // FormatPackageName

    /// <summary>
    /// 
    /// Instantiate each raw with buttons considering the buttons zone and the top and bottom limits
    /// 
    /// </summary>
    /// <param name="num">Number of raws per colum</param>
    public void InstantiateRaws(int num)
    {
        GameObject r;
        _currentButton = 1;

        // Sets the limits and space between raws
        _columns.GetComponent<VerticalLayoutGroup>().padding.top = _topLimit;
        _columns.GetComponent<VerticalLayoutGroup>().padding.bottom = _bottomLimit;

        _columns.GetComponent<VerticalLayoutGroup>().spacing = _spaceRaws;

        // For each raw to the maximun number of raws instantiates n buttons
        for (int i = 0; i < num; i++)
        {
            r = Instantiate(_raws.gameObject, _columns.transform);

            InstantiateButtons(r);
        } // for
    } // InstantiateRaws

    
    /// <summary>
    /// 
    /// Sets the raws size and the spacing between them to instantiate the buttons inside
    /// 
    /// </summary>
    /// <param name="raw"> (GameObject) Empty object that packages all buttons in a raw. </param>
    public void InstantiateButtons(GameObject raw)
    {
        // Temporal gameobject to refer to Button
        GameObject temp;

        // Set padding
        raw.gameObject.GetComponent<HorizontalLayoutGroup>().padding.left = 12;
        raw.gameObject.GetComponent<HorizontalLayoutGroup>().padding.right = 12;

        // Set spacing and anchors
        raw.gameObject.GetComponent<HorizontalLayoutGroup>().spacing = 17;
        raw.gameObject.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

        // For each space for buttons in a raw instantiate one of them and sets all them need
        for (int i = 0; i < _btnsRaw; i++)
        {
            temp = Instantiate(_levelSelectionButton, raw.transform);

            SetButton(temp);
        } // for
    } // InstantiateButtons

    /// <summary>
    /// 
    /// Checks if the level buttons correspond to a completed or not completed level to make
    /// it interactable or not.
    /// 
    /// </summary>
    /// <param name="bt"> (GameObject) Level selection button </param>
    public void SetButton(GameObject bt)
    {
        // Set level number
        bt.GetComponent<LevelSelectionButton>().SetLevel(_currentButton);
        bt.GetComponent<LevelSelectionButton>().SetColor(_lp.playerColor);

        // If the button that is instantiating is lower or equal to the competed level
        if (_currentButton - 1 < _levelsCompleted)
        {
            // Sets this level button active and interactable
            bt.GetComponent<LevelSelectionButton>().SetCompleted(true);
            bt.GetComponent<LevelSelectionButton>().SetInteractable(true);
        } // if
        else if(_currentButton - 1 == _levelsCompleted)
        {
            bt.GetComponent<LevelSelectionButton>().SetCompleted(false);
            bt.GetComponent<LevelSelectionButton>().SetInteractable(true);
        } // else if
        else
        {
            // If the button is higher to the completed level, sets it non active and not interactiable
            bt.GetComponent<LevelSelectionButton>().SetInteractable(false);
        } // else

        // If there are more levels increase the actual button value
        if (_currentButton <= _lp.levels.Length)
        {
            _currentButton++;
        } // if 
    } // SetButton
} // LevelSelectionManager
