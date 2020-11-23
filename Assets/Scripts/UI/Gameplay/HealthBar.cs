using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : Bar
{
    Entity entity;

    public override void Start()
    {
        entity = transform.parent.GetComponent<Entity>();

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if(entity != null)
            SetValue(entity.health);
    }
}
