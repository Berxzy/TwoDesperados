using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PAGE_NAME
{ 
    MAIN_MENU,
    SETTINGS,
    HUD,
    PAUSE_MENU,
    INVALID
}

public class MenuManager : MonoBehaviour
{
    public static MenuManager menuManagerInstance;
    private Page currentPage;

    public List<Page> allPages = new List<Page>();

    private void Awake()
    {
        menuManagerInstance = this;

        if(allPages.Count > 0)
        {
            OpenPage(allPages[0].pageName);
        }
    }

    public void OpenPage(PAGE_NAME pageName)
    {
        CloseCurrentPage();

        foreach(Page page in allPages)
        {
            if(page != null && page.pageName == pageName)
            {
                currentPage = Instantiate(page, this.transform);
                break;
            }
        }
    }

    public void CloseCurrentPage()
    {
        if(currentPage != null)
        {
            currentPage.Close();
            Destroy(currentPage.gameObject);
        }
    }
}
