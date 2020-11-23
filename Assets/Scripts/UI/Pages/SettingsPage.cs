using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : Page
{
    SaveData saveData;
    bool shouldSave;

    public Text width;
    public Text height;
    public Text obstaclesCount;
    public Text destructibleObstaclesCount;
    public Text wavesCount;
    public Text waveDuration;
    public Text enemiesInWave;
    public Text sound;

    public override void Start()
    {
        base.Start();

        saveData = SaveData.GetSaveData();
        shouldSave = false;

        UpdateValues();
    }

    public void UpdateValues()
    {
        width.text = saveData.n + "";
        height.text = saveData.m + "";
        obstaclesCount.text = saveData.numberOfSolidObstacles + "";
        destructibleObstaclesCount.text = saveData.numberOfDestructibleObstacles + "";
        wavesCount.text = saveData.numberOfWaves + "";
        waveDuration.text = saveData.waveDuration + "";
        enemiesInWave.text = saveData.numberOfEnemiesPerWave + "";
        sound.text = saveData.isSoundOn ? "SOUND ON" : "SOUND OFF";
    }

    public override void Close()
    {
        if(shouldSave)
        {
            SaveData.SaveSaveData(saveData);
        }

        saveData = SaveData.GetSaveData();
        UpdateSound();
    }

    public void OnExitPageButton(bool shouldSave)
    {
        this.shouldSave = shouldSave;
        MenuManager.menuManagerInstance.OpenPage(PAGE_NAME.MAIN_MENU);
    }

    public void ResetToDefault()
    {
        SaveData.ResetSaveData();
        saveData = SaveData.GetSaveData();
        UpdateSound();
        UpdateValues();
    }

    private int CheckAndCorectValue(int value, int minValue = 0, int maxValue = int.MaxValue)
    {
        if (value < minValue)
            value = minValue;
        else if (value > maxValue)
            value = maxValue;

        return value;
    }

    public void ChangeLevelWidth(int value)
    {
        saveData.n = CheckAndCorectValue(saveData.n + value, 5);
    }

    public void ChangeLevelHeight(int value)
    {
        saveData.m = CheckAndCorectValue(saveData.m + value, 5);
    }

    public void ChangeNumberOfSolidObstacles(int value)
    {
        saveData.numberOfSolidObstacles = CheckAndCorectValue(saveData.numberOfSolidObstacles + value);
    }
    public void ChangeNumberOfDestructibleObstacles(int value)
    {
        saveData.numberOfDestructibleObstacles = CheckAndCorectValue(saveData.numberOfDestructibleObstacles + value);
    }
    public void ChangeNumberOfWaves(int value)
    {
        saveData.numberOfWaves = CheckAndCorectValue(saveData.numberOfWaves + value, 1);
    }
    public void ChangeWaveDuration(int value)
    {
        saveData.waveDuration = CheckAndCorectValue(saveData.waveDuration + value);
    }
    public void ChangeNumberOfEnemiesInWave(int value)
    {
        saveData.numberOfEnemiesPerWave = CheckAndCorectValue(saveData.numberOfEnemiesPerWave + value, 1);
    }

    public void ChangeSoundSetting()
    {
        saveData.isSoundOn = AudioListener.volume <= 0;
        UpdateSound();
    }

    private void UpdateSound()
    {
        AudioListener.volume = saveData.isSoundOn ? 1 : 0;
    }
}
