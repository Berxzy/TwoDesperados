using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Player player;
    PlayerBase playerBase;

    float lastPlayerHealth;

    public float shakeDuration = 0.5f;
    public float shakeRate = 0.1f;
    public float shakeIntensity = 0.1f;

    float currentShakeDuration;
    float currentShakeRate;

    // Start is called before the first frame update
    void Start()
    {
        player = LevelManager.levelManagerInstance.playerGameObject.GetComponent<Player>();
        playerBase = LevelManager.levelManagerInstance.playerBaseGameObject.GetComponent<PlayerBase>();

        lastPlayerHealth = player.health;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        if(!LevelManager.levelManagerInstance.HasGameEnded())
        {
            if (player != null)
            {
                if (player.health != lastPlayerHealth)
                {
                    lastPlayerHealth = player.health;

                    StartShake();
                }

                transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            }
        }

        //Update shake
        if (currentShakeDuration > 0)
        {
            currentShakeDuration -= Time.deltaTime;

            if (currentShakeRate > 0)
            {
                currentShakeRate -= Time.deltaTime;
            }
            else
            {
                transform.position += new Vector3(Random.Range(0, shakeIntensity), Random.Range(0, shakeIntensity), 0);
                currentShakeRate = shakeRate;
            }
        }
    }

    void StartShake()
    {
        currentShakeDuration = shakeDuration;
    }
}
