using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public struct PlayerData
{
    // Player's information 
    public float _playerLevel;                               // Coins that the player has
    public Dictionary<string, int> _completedLevelsPackage;  // Levels completed per package
    public int _hints;                                       // Hints available
    public int _timePlayed;                                  // Time played this match
    public bool _adsRemoved;                                 // if the player paid for no ads
    private int _hash;                                       // hash code 

    public PlayerData(float level, Dictionary<string, int> completed, int hints, bool removed)
    {
        _playerLevel = level;
        _completedLevelsPackage = completed;
        _hints = hints;
        _adsRemoved = removed;
        _hash = 0;
        _timePlayed = 0;

        GetTimePlayed();
    } // PlayerData

    public int GetTimePlayed()
    {
        _timePlayed = 0;
        foreach (var data in _completedLevelsPackage)
        {
            _timePlayed += data.Value;
        } // foreach 

        return _timePlayed;
    } // GetTimePlayed

    /// <summary>
    /// Give access to the hash. Consult only. 
    /// </summary>
    /// <returns>int Hash actual value</returns>
    public int GetHash()
    {
        return _hash;
    } // GetHash

    /// <summary>
    /// Used to set the new hash value after calculation. 
    /// </summary>
    /// <param name="h">int New hash value</param>
    public void SetHash(int h)
    {
        _hash = h;
    } // SetHash
} // PlayerData

public class FileLoader
{
    static private int numPackages = 0490;

    public static PlayerData ReadPlayerData(string[] packages)
    {
        if(File.Exists(Application.persistentDataPath + "/creta.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream f = File.Open(Application.persistentDataPath + "creta.dat", FileMode.Open);

            PlayerData d = (PlayerData)bf.Deserialize(f);

            int totalTimePlayed = d._timePlayed;
            int hash = d.GetHash();

            d.SetHash(0);
            int checkTime = numPackages;
            foreach (var data in d._completedLevelsPackage)
            {
                checkTime += data.Value;
            } // foreach
            int checkHash = Encrypt(bf, d);
            f.Close();

            if (hash == checkHash && totalTimePlayed == checkTime)
            {
                return d;
            } // if
            else
            {
                return NewPlayerData(packages);
            } // else
        } // if
        else
        {
            return NewPlayerData(packages);
        } // else
    } // ReadPlayerData

    public static void SavePlayerData(PlayerData d)
    {
        // Initialize BinaryFormatter
        BinaryFormatter bf = new BinaryFormatter();

        // Create the new player save file
        FileStream file = File.Create(Application.persistentDataPath + "/creta.dat");

        // Save the levels played with the coins (Salt)
        d._timePlayed = numPackages + d.GetTimePlayed();

        // Reset the hash for new codification
        if (d.GetHash() != 0)
        {
            d.SetHash(0);
        }

        // Create new hash code and write info in the file
        d.SetHash(Encrypt(bf, d));
        bf.Serialize(file, d);

        file.Close();
    } // SavePlayerData

    public static PlayerData NewPlayerData(string[] packages)
    {
        Dictionary<string, int> completed = new Dictionary<string, int>();

        for (int i = 0; i < packages.Length; i++)
        {
            completed.Add(packages[i], 7);
        } // for

        PlayerData dat = new PlayerData(0.0f, completed, 0, false);

        return dat;
    } // NewPlayerData

    public static int Encrypt(BinaryFormatter b, PlayerData d)
    {
        // Create MemoryStream
        MemoryStream ms = new MemoryStream();

        // Serialize info in memoryStream
        b.Serialize(ms, d);

        // Seek 0 value
        ms.Seek(0, SeekOrigin.Begin);

        // Create and return hash code
        byte[] bytes = new byte[ms.Length];
        return ms.Read(bytes, 0, (int)ms.Length.GetHashCode());
    } // Encrypt
} // FileLoader