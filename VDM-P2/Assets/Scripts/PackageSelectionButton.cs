using UnityEngine;
using UnityEngine.UI;

public class PackageSelectionButton : MonoBehaviour
{
    [Header("Configuration")]
    public Text _title;
    public Text _percentage;
    public Button _butComponent;

    // Start is called before the first frame update
    void Start()
    {
        // Get button component
        _butComponent = gameObject.GetComponent<Button>();

        // Make sure transition is spriteSwap
        _butComponent.transition = Selectable.Transition.SpriteSwap;

        // Add listener
        _butComponent.onClick.AddListener(TaskOnClick);
    } // Start

    void TaskOnClick()
    {
        GameManager.GetInstance().SetPackage(_title.text);
        GameManager.GetInstance().ChangeToLevelSelection();
    } // TaskOnClick

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
} // PackageSelectionButton
