using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //level save data
    public int n = 15;
    public int m = 15;

    public int numberOfSolidObstacles = 40;
    public int numberOfDestructibleObstacles = 60;

    public int numberOfWaves = 3;
    public int waveDuration = 10;
    public int numberOfEnemiesPerWave = 3;

    //sound settings
    public bool isSoundOn = true;

    public static SaveData GetSaveData()
    {
        string saveDataString = PlayerPrefs.GetString("SaveData");

        if(saveDataString.Length == 0)
        {
            ResetSaveData();
            saveDataString = PlayerPrefs.GetString("SaveData");
        }
        
        return JsonUtility.FromJson<SaveData>(saveDataString);
    }

    public  static void ResetSaveData()
    {
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(new SaveData()));
    }

    public static void SaveSaveData(SaveData saveData)
    {
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(saveData));
    }
}
