using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public bool isVisibleWhenMax = true;

    private float originalSize;
    private float maxValue = -1;
    private float currentValue;

    private Vector3 offset;
    private Transform exParent;

    // Start is called before the first frame update
    public virtual void Start()
    {
        originalSize = transform.localScale.x;

        offset = transform.localPosition;
        exParent = transform.parent;

        transform.parent = null;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (exParent != null)
            transform.position = exParent.position + offset;
        else
            Destroy(gameObject);
    }

    public void SetValue(float value)
    {
        if (value > maxValue)
        {
            maxValue = value;
            transform.GetComponent<SpriteRenderer>().enabled = isVisibleWhenMax;
        }
        else if(!isVisibleWhenMax && value < maxValue)
        {
            transform.GetComponent<SpriteRenderer>().enabled = true;
        }

        currentValue = value < 0 ? 0 : value;
        transform.localScale = new Vector3(currentValue / maxValue * originalSize, transform.localScale.y, transform.localScale.z);
    }
}
