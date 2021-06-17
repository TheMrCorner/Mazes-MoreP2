using UnityEngine;
using UnityEngine.UI;

public class PackageSelectionButton : MonoBehaviour
{
    [Header("Configuration")]
    public Text _title;
    public Text _percentage;
    public Button _butComponent;

    private string _link;

    // Start is called before the first frame update
    void Start()
    {
        // Get button component
        _butComponent = gameObject.GetComponent<Button>();

        // Make sure transition is spriteSwap
        _butComponent.transition = Selectable.Transition.SpriteSwap;

    } // Start

    void TaskOnClick()
    {
        GameManager.GetInstance().SetPackage(_title.text);
        GameManager.GetInstance().ChangeToLevelSelection();
    } // TaskOnClick

    void OpenLink()
    {
        GameManager.GetInstance().OpenLink(_link);
    } // OpenLink

    public void SetSprites(Sprite normal, Sprite pressed)
    {
        // Set the sprites that will be used
        _butComponent.GetComponent<Image>().sprite = normal;

        // Set pressed sprite
        SpriteState s = _butComponent.spriteState;
        s.pressedSprite = pressed;
        _butComponent.spriteState = s;
    } // SetSprites

    public void SetPackageName(string name)
    {
        _title.text = name;
    } // SetPackageName

    public void SetPercentage(int percentage)
    {
        _percentage.text = percentage.ToString() + "%";
    } // SetPercentage

    public void SetNormalButton()
    {
        // Add listener
        _butComponent.onClick.AddListener(TaskOnClick);
    } // SetNormalButton

    public void SetAd(string l)
    {
        _title.gameObject.SetActive(false);
        _percentage.gameObject.SetActive(false);
        _link = l;
        _butComponent.onClick.AddListener(OpenLink);
    } // SetAd
} // PackageSelectionButton
