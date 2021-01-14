using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelPackage")]
public class LevelPackage : ScriptableObject
{
    public TextAsset[] levels;
    public Color playerColor;
}
