using UnityEngine;

/// <summary>
/// 
/// Scriptable object that stores the data of a 
/// package. 
/// 
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "LevelPackage")]
public class LevelPackage : ScriptableObject
{
    public string name;
    public TextAsset[] levels;
    public Color playerColor;
    public Sprite buttonUp;
    public Sprite buttonDown;
} // LevelPackage
