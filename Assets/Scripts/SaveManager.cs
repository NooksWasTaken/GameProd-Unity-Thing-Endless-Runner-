using System.IO;
using UnityEngine;
using GameData;

public class SaveManager : MonoBehaviour
{
    internal SaveData saveData;
    public string filename = "data.json";
    string filepath;

    void Awake()
    {
        Load();
    }

    internal void Load()
    {
        saveData = new SaveData();

        filepath = $"{Application.dataPath}/{filename}";
        string data = ReadJSON();
        Debug.Log("LOG: Data retrieval successful. Loading data into game...");

        JsonUtility.FromJsonOverwrite(data, saveData);
    }

    internal void Save()
    {
        string newData = JsonUtility.ToJson(saveData);
        FileStream fileIO;

        if (File.Exists(filepath))
        {
            Debug.Log("LOG: Overwriting save data...");

            fileIO = new FileStream(filepath, FileMode.Open);
            using (StreamWriter fileWriter = new StreamWriter(fileIO)) fileWriter.Write(newData);
            
            Debug.Log("LOG: Data saved successfully.");
        }
        else
        {
            Debug.Log("LOG: Creating save data...");

            fileIO = File.Create(filepath);
            using (StreamWriter fileWriter = new StreamWriter(fileIO)) fileWriter.Write(newData);

            if (File.Exists(filepath)) Debug.Log("LOG: Save data creation successful.");
            else
            {
                Debug.LogError("ERROR: Save data creation failed. Attempting again...");
                Save();
            }
        }
    }

    string ReadJSON()
    {
        if (File.Exists(filepath))
        {
            Debug.Log("LOG: Existing save data found. Reading file...");

            using (StreamReader fileReader = new StreamReader(filepath))
            {
                return fileReader.ReadToEnd();
            }
        }
        else
        {
            Debug.Log("LOG: Save data does not exist.");

            Save();
            ReadJSON();
        }

        return string.Empty;
    }
}
