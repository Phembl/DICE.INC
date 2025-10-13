using UnityEngine;

public class Tooltip_BackgroundCloser : MonoBehaviour
{
    void OnMouseUp()
    {
        if (TooltipManager.instance.GetTooltipStatus() && !TooltipManager.instance.GetWorkingStatus())
        {
            TooltipManager.instance.CloseTooltip(); 
        }
        
    }
}
