using DICEINC.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_Tooltip : MonoBehaviour
{
    [SerializeField] private InteractionAreaType interactionArea;
    [SerializeField] private Sprite spritePressed;
    
    private Sprite spriteUnpressed;
    
    //Components
    private Image bgImage;
    
    //State Tracking
    private bool isActive;
    

    void Start()
    {
        if (interactionArea == InteractionAreaType.None)
        {
            Debug.LogError($"{name} InteractionAreaType is not set");
        }
        
        bgImage = GetComponent<Image>();
        spriteUnpressed = bgImage.sprite;
      
    }

    void OnMouseDown()
    {
        bgImage.sprite = spritePressed;
        
    }

    void OnMouseUp()
    {
        bgImage.sprite = spriteUnpressed;
        
        if (!isActive) //Open Tooltip
        {
            if (TooltipManager.instance.GetWorkingStatus()) return; //Checks if the tooltip is currently being build
            
            isActive = true;
            TooltipManager.instance.OpenTooltip(interactionArea);
            
        }
        
        else //Close Tooltip
        {
            if (TooltipManager.instance.GetWorkingStatus()) return; //Checks if the tooltip is currently being build
            
            isActive = false;
            TooltipManager.instance.CloseTooltip(); 
        }
    }
}
