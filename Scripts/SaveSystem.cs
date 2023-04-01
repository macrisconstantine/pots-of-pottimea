using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// This class was given for use by the instructor, so I do not fully understand it.
// But it allows for game data to be serialized and saved
public static class SaveSystem
{
    public static void SaveGame(SaveData playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.sav";
        Debug.Log("Saving: " + path);
        FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    public static SaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/game.sav";
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist!!!");
            return null;
        }
        Debug.Log("Loading: " + path);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        SaveData temp = (SaveData)formatter.Deserialize(stream);
        stream.Close();
        return temp;
    }
}
