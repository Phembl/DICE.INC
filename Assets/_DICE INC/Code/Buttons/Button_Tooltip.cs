using DICEINC.Global;
using UnityEngine;

public class Button_Tooltip : Button
{
    [SerializeField] private InteractionAreaType interactionArea;
    
    void Start()
    {
        if (interactionArea == InteractionAreaType.None)
        {
            Debug.LogError($"{name} InteractionAreaType is not set");
        }
        
        isActive = true;
    }
    
    protected override void ButtonAction()
    {
        if (!TooltipManager.instance.GetTooltipStatus()) //Open Tooltip
        {
            if (TooltipManager.instance.GetWorkingStatus()) return; //Checks if the tooltip is currently being build
            
           
            TooltipManager.instance.OpenTooltip(interactionArea);
        }
        
        else //Close Tooltip
        {
            if (TooltipManager.instance.GetWorkingStatus()) return; //Checks if the tooltip is currently being build
            
            
            TooltipManager.instance.CloseTooltip(); 
        }
    }
    
    
}
