using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public PAGE_NAME pageName = PAGE_NAME.INVALID;

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void Close()
    {

    }

    public Rect GetElementRect(int index, float width, float height)
    {
        return new Rect(Screen.width / 2 - width / 2, index * height + index * height / 10, width, height);
    }
}
