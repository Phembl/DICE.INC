using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interactor_StartResearch : MonoBehaviour
{
    //Sprites
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteHovered;
    
    
    //Colors
    private Color colorNormal;
    private Color colorDark;
    private Color colorInactive;
    
    private TMP_Text title;
    private Image image;
    
    //State Tracking
    private bool isHovered;
    private bool isActive;
    public void SetActivity(bool active) => isActive = active;
    

    void Start()
    {
        colorNormal = SettingsManager.instance.colorNormal;
        colorDark = SettingsManager.instance.colorDark;
        colorInactive = SettingsManager.instance.colorInactive;
        
        image = GetComponent<Image>();
        title = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        
        image.sprite = spriteNormal;
        image.color = colorNormal;
        title.color = colorNormal;
        
    }
    

    void OnMouseOver()
    {
        if (isHovered || !isActive) return;
        
        isHovered = true;
        
        image.sprite = spriteHovered;
        title.color = colorDark;
        
    }

    void OnMouseExit()
    {
        if (!isHovered) return;
        
        isHovered = false;
        
        image.sprite = spriteNormal;
        title.color = colorNormal;
        
        
    }

    void OnMouseDown()
    {
        if (!isActive) return;
        
        Lab.instance.StartStopResearch();
    }
}
