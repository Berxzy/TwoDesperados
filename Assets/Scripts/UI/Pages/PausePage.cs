using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePage : Page
{
    public GameObject resumeButton;
    public Text scoreText;
    public Text deathReasonText;
    public GameObject winningMusic;

    // Start is called before the first frame update
    public override void Start()
    {
        Time.timeScale = 0;

        LevelManager lManager = LevelManager.levelManagerInstance;

        if (lManager.HasGameEnded())
        {
            if (resumeButton != null)
            {
                resumeButton.SetActive(false);
            }

            scoreText.gameObject.SetActive(true);
            scoreText.text = "YOU KILLED " + LevelManager.levelManagerInstance.deadEnemiesCount + " ENEMIES!";

            deathReasonText.gameObject.SetActive(true);

            if(!lManager.IsAlive(lManager.playerGameObject))
            {
                deathReasonText.text = "YOU GOT KILLED!";
            }
            else if(!lManager.IsAlive(lManager.playerBaseGameObject))
            {
                deathReasonText.text = "BASE GOT DESTROYED!";
            }
            else if(lManager.HasPlayerWon())
            {
                winningMusic.SetActive(true);
                deathReasonText.text = "YOU DEFENDED THE BASE! GOOD JOB SOLDIER!";
            }
        }
    }

    private bool ShowEndGameScreen(Entity entity)
    {
        if (entity != null && entity.health <= 0)
        {
            

            return true;
        }

        return false;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Close()
    {
        base.Close();
        Time.timeScale = 1;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnResume()
    {
        MenuManager.menuManagerInstance.OpenPage(PAGE_NAME.HUD);
    }
}
