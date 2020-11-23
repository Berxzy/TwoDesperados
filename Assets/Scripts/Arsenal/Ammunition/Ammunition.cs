using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    public GameObject hitEffect;

    public float damage;
    public float moveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        Entity entity = collider2D.transform.GetComponent<Entity>();

        if(!(entity is Ghost))
        {
            if (entity != null)
            {
                entity.RecieveDamage(damage);

                if (hitEffect != null)
                {
                    Destroy(Instantiate(hitEffect, transform.position + new Vector3(0, 0, -3), Quaternion.identity).gameObject, 1);
                }
            }

            Destroy(this.gameObject);
        }
    }
}
