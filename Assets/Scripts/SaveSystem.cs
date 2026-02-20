using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveSystem : MonoBehaviour
{
    private const string FileName = "save_data.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public SaveData Current { get; private set; } = new SaveData(); // works now

    private void Start()
    {
        GameManager.Instance.OnSaveData += SaveProgress;
    }

    public SaveData Load()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                Current = CreateDefault();
                Save(Current);
                return Current;
            }

            string json = File.ReadAllText(FilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                Current = CreateDefault();
                Save(Current);
                return Current;
            }

            Current = JsonUtility.FromJson<SaveData>(json) ?? CreateDefault();
            return Current;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"SaveSystem Load failed. Creating default. Error: {e.Message}");
            Current = CreateDefault();
            Save(Current);
            return Current;
        }
    }

    public void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            Current = data;
            DebugManager.Instance.Log(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveSystem Save failed: {e.Message}");
        }
    }

    public void SaveProgress(int level, int score)
    {
        if (Current == null) Current = CreateDefault();

        Current.level = level;
        Current.score = score;

        if (score > Current.highScore)
            Current.highScore = score;

        Save(Current);
    }

    public void ResetSave()
    {
        try
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            Current = CreateDefault();
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveSystem ResetSave failed: {e.Message}");
        }
    }

    private SaveData CreateDefault()
    {
        return new SaveData
        {
            level = 1,
            score = 0,
            highScore = 0
        };
    }
}

[Serializable]
public class SaveData
{
    public int level;
    public int score;
    public int highScore;

    // IMPORTANT: keep a parameterless constructor (or omit constructors entirely)
    public SaveData() { }

    // Optional convenience constructor
    public SaveData(int level, int score, int highScore)
    {
        this.level = level;
        this.score = score;
        this.highScore = highScore;
    }
}
