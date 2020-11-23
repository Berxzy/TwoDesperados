using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float health = 100;

    //fx
    public AudioSource dyingSound;

    public virtual void RecieveDamage(float damage)
    {
        if (health > 0 && health - damage <= 0)
        {
            health = 0;
            Die();
        }
        else
        {
            health -= damage;
        }
    }

    public virtual void Die()
    {
        health = 0;

        if(dyingSound != null)
        {
            dyingSound.Play();
        }
    }
}
