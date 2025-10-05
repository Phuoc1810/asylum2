using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/playerSave.dat";

    // Save any object (like PlayerData)
    public static void Save<T>(T data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
        Debug.Log("Game Saved: " + savePath);
    }

    // Load object (returns null if no save exists)
    public static T Load<T>() where T : class
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Open))
            {
                return formatter.Deserialize(stream) as T;
            }
        }
        else
        {
            Debug.LogWarning("Save file not found: " + savePath);
            return null;
        }
    }

    // Delete save if needed
    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save deleted.");
        }
    }
}
