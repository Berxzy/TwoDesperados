using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPage : Page
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        SaveData saveData = SaveData.GetSaveData();
        AudioListener.volume = saveData.isSoundOn ? 1 : 0;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToSettings()
    {
        MenuManager.menuManagerInstance.OpenPage(PAGE_NAME.SETTINGS);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
