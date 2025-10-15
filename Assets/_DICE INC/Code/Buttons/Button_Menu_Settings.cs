using UnityEngine;

public class Button_Menu_Settings : Button
{
    void Start()
    {
        isActive = true;
    }

    protected override void ButtonAction()
    {
        if (!MenuManager.instance.GetBusyStatus()) MenuManager.instance.OpenMenu(2);
    }
}
