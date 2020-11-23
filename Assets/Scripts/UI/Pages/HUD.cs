using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : Page
{
    public Text score;
    public Player player;

    public GameObject moveStartImage;
    public GameObject moveCurrentImage;
    public GameObject fireImage;
    public Text wave;

    LevelManager lManager;

    public override void Start()
    {
        base.Start();

        lManager = LevelManager.levelManagerInstance;
        player = lManager.playerGameObject;
    }

    public void OnPause()
    {
        MenuManager.menuManagerInstance.OpenPage(PAGE_NAME.PAUSE_MENU);
    }

    public override void Update()
    {
        base.Update();
        score.text = LevelManager.levelManagerInstance.deadEnemiesCount + "";

        wave.text = "WAVE:" + lManager.currentWaveIndex + "/" + lManager.numberOfWaves;
        if(lManager.currentWaveIndex != lManager.numberOfWaves)
            wave.text += " " + (int)lManager.currentWaveDuration + "s";

        //Draw inputs
        if (player != null)
        {
            if(player.movementDirection != Vector2.zero)
            {
                moveStartImage.SetActive(true);
                moveCurrentImage.SetActive(true);

                if (player.isShootingInputActive)
                {
                    fireImage.SetActive(true);
                    fireImage.transform.position = player.fireTouchPosition;
                }

                moveStartImage.transform.position = player.moveTouchStartPosition;
                moveCurrentImage.transform.position = player.moveTouchCurrentPosition;
            }
            else
            {
                moveStartImage.SetActive(false);
                moveCurrentImage.SetActive(false);

                if (!player.isShootingInputActive)
                    fireImage.SetActive(false);
            }
        }
    }
}
