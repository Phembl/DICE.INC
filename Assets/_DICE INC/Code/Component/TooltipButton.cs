using DICEINC.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipButton : MonoBehaviour
{
    [SerializeField] private InteractionAreaType interactionArea;
    
    //Colors
    private Color colorNormal;
    private Color colorDark;
    
    //Components
    private TMP_Text markTMP;
    private Image bgImage;
    
    //State Tracking
    private bool isHovered;
    private bool isActive;

    private bool printLog;

    void Start()
    {
        if (interactionArea == InteractionAreaType.None)
        {
            Debug.LogError($"{name} InteractionAreaType is not set");
        }
        
        markTMP = transform.GetChild(0).GetComponent<TMP_Text>();
        bgImage = GetComponent<Image>();
        
        if (!markTMP) Debug.LogError($"{name} did not find markTMP");
        
        SetColor();
    }

    void SetColor()
    {
        colorNormal = SettingsManager.instance.colorNormal;
        colorDark = SettingsManager.instance.colorDark;

        bgImage.color = colorDark;
        markTMP.color = colorNormal;
    }
    
    void OnMouseOver()
    {
        if (isHovered) return;
        
        isHovered = true;
       
        bgImage.color = colorNormal;
        markTMP.color = colorDark;
        
    }

    void OnMouseExit()
    {
        if (!isHovered) return;
        
        isHovered = false;
        
        bgImage.color = colorDark;
        markTMP.color = colorNormal;
    }

    void OnMouseDown()
    {
        

        if (!isActive) //Open Tooltip
        {
            if (TooltipManager.instance.GetWorkingStatus()) return; //Checks if the tooltip is currently being build
            
            isActive = true;
            markTMP.text = "X";
            TooltipManager.instance.OpenTooltip(interactionArea);
            
        }
        
        else //Close Tooltip
        {
            if (TooltipManager.instance.GetWorkingStatus()) return; //Checks if the tooltip is currently being build
            
            isActive = false;
            markTMP.text = "?";
            TooltipManager.instance.CloseTooltip(); 
        }
        
    }
}
