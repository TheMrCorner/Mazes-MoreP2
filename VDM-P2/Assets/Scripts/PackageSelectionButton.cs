using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Component added to the buttons that are going to select
/// the gamemode. The information is set dynamically when it
/// is instantiated. This gives the possibility to add new 
/// ads and packages without touching any code.
/// 
/// </summary>
public class PackageSelectionButton : MonoBehaviour
{
    [Header("Configuration")]
    public Text _title;           // Title of the GameMode.
    public Text _percentage;      // Percentage of levels completed.
    public Button _butComponent;  // Button component to set sprites.

    private string _link;         // Link to open when the button is set as an Ad.

    // Start is called before the first frame update
    void Start()
    {
        // Get button component
        _butComponent = gameObject.GetComponent<Button>();

        // Make sure transition is spriteSwap
        _butComponent.transition = Selectable.Transition.SpriteSwap;

    } // Start

    /// <summary>
    /// 
    /// Normal task on click. Set the GameMode in the GameManager and 
    /// notify to change scene.
    /// 
    /// </summary>
    void TaskOnClick()
    {
        GameManager.GetInstance().SetPackage(_title.text);
        GameManager.GetInstance().ChangeToLevelSelection();
    } // TaskOnClick

    /// <summary>
    /// 
    /// Ad's task on click. This opens a browser window with 
    /// the link provided.
    /// 
    /// </summary>
    void OpenLink()
    {
        GameManager.GetInstance().OpenLink(_link);
    } // OpenLink

    /// <summary>
    /// 
    /// Set the sprites that will be used by the button to 
    /// animate and change between them.
    /// 
    /// </summary>
    /// <param name="normal"> (Sprite) Standard/Idle sprite. </param>
    /// <param name="pressed"> (Sprite) Pressed sprite. </param>
    public void SetSprites(Sprite normal, Sprite pressed)
    {
        // Set the sprites that will be used
        _butComponent.GetComponent<Image>().sprite = normal;

        // Set pressed sprite
        SpriteState s = _butComponent.spriteState;
        s.pressedSprite = pressed;
        _butComponent.spriteState = s;
    } // SetSprites

    /// <summary>
    /// 
    /// Sets the GameMode that this button will open. 
    /// 
    /// </summary>
    /// <param name="name"> (string) Package name. </param>
    public void SetPackageName(string name)
    {
        _title.text = name;
    } // SetPackageName

    /// <summary>
    /// 
    /// Sets the percentage of levels completed in this GameMode.
    /// 
    /// </summary>
    /// <param name="percentage"> (int) Percentage. </param>
    public void SetPercentage(int percentage)
    {
        _percentage.text = percentage.ToString() + "%";
    } // SetPercentage

    /// <summary>
    /// 
    /// Sets this button to be a normal GameMode selector, instead
    /// of an Ad. Adds the TaskOnClick method as a listener for the
    /// Button's onClick.
    /// 
    /// </summary>
    public void SetNormalButton()
    {
        // Add listener
        _butComponent.onClick.AddListener(TaskOnClick);
    } // SetNormalButton

    /// <summary>
    /// 
    /// Sets this button as an Ad. Sets the link that this will 
    /// open when clicked and deactivates all texts.
    /// 
    /// </summary>
    /// <param name="l"> (string) Link. </param>
    public void SetAd(string l)
    {
        _title.gameObject.SetActive(false);
        _percentage.gameObject.SetActive(false);
        _link = l;
        _butComponent.onClick.AddListener(OpenLink);
    } // SetAd
} // PackageSelectionButton
