using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform ammunitionSpawnPoint;

    public Ammunition ammunition;

    public float reloadTime;
    public int clipSize;
    public float fireRate;

    private float currentReloadTime;
    private int currentClipSize;

    //fx
    public AudioSource shootingSound;

    // Start is called before the first frame update
    void Start()
    {
        currentClipSize = clipSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentReloadTime > 0)
        {
            currentReloadTime -= Time.deltaTime;
        }
    }

    public bool Fire()
    {
        if(currentReloadTime <= 0 && currentClipSize > 0)
        {
            Ammunition projectile = Instantiate(ammunition, ammunitionSpawnPoint);
            projectile.transform.parent = null;

            currentClipSize--;

            if(currentClipSize == 0)
            {
                currentClipSize = clipSize;
                currentReloadTime = reloadTime;
            }
            else
            {
                currentReloadTime = fireRate;
            }

            if (shootingSound != null)
                shootingSound.Play();

            return true;
        }

        return false;
    }
}
