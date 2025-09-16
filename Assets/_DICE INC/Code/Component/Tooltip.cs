using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    //Colors
    private Color colorNormal;
    private Color colorDark;
    
    //Components
    private TMP_Text markTMP;
    private Image bgImage;
    
    //State Tracking
    private bool isHovered;

    void Start()
    {
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
        Debug.Log($"{name} is opening Tooltip");
    }
}
