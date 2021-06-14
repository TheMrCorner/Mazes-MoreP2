using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject {

    // Package of which the level is
    public LevelPackage _package;

    // Current level playing in package
    public int _level;
} // LevelData