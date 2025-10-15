using UnityEngine;

public class Menu_BackgroundCloser : MonoBehaviour
{
    void OnMouseUp()
    {
        if (MenuManager.instance.GetMenuStatus() && 
            !MenuManager.instance.GetBusyStatus())
        {
            MenuManager.instance.CloseMenu(); 
        }
        
    }
}
