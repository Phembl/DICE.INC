using UnityEngine;

public class Button_Menu_Close : Button
{
    void Start()
    {
        isActive = true;
    }

    protected override void ButtonAction()
    {
        if (MenuManager.instance.GetMenuStatus() && 
            !MenuManager.instance.GetBusyStatus())
        {
            // Close Menu
            MenuManager.instance.CloseMenu();
        }
    }
}
