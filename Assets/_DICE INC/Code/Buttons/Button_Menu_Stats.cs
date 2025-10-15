using UnityEngine;

public class Button_Menu_Stats : Button
{
    void Start()
    {
        isActive = true;
    }

    protected override void ButtonAction()
    {
        if (!MenuManager.instance.GetBusyStatus()) MenuManager.instance.OpenMenu(1);
    }
}
