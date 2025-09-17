using System;
using UnityEngine;
using DICEINC.Global;
using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    
    
    [SerializeField] private bool printLog;

    [TitleGroup("References")] 
    [Header("Workshop")]
    [SerializeField] private Workshop workshopComponent;
    [SerializeField] private Canvas workshopCanvas;
    [SerializeField] private GameObject workshopTooltip;
    

    private InteractionArea currentInteractionArea;
    private Canvas currentCanvas;
    private GameObject currentTooltip;
    
    private bool isCurrentlyWorking;
    public bool GetWorkingStatus() => isCurrentlyWorking;
    
    private bool tooltipIsOpen;
    public bool GetTooltipStatus() => tooltipIsOpen;
    
    public static TooltipManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        workshopTooltip.SetActive(false);
    }

    public void OpenTooltip(InteractionAreaType interactionArea)
    {
        if (printLog) Debug.Log("|-------------- TOOLTIP --------------|");
        if (printLog) Debug.Log($"{interactionArea.ToString()} Tooltip is opened.");
        
        isCurrentlyWorking = true;
        
        var tooltipData = GetTooltipData(interactionArea);
        
        currentCanvas.planeDistance = 80;
        currentCanvas.sortingOrder = 2;
        
        currentTooltip.GetComponent<CanvasGroup>().alpha = 0;
        currentTooltip.SetActive(true);
        
        Transform tooltipContent = currentTooltip.transform.GetChild(1);
        tooltipContent.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tooltipData.areaTitle;
        tooltipContent.GetChild(1).gameObject.GetComponent<TMP_Text>().text = tooltipData.areaDescription;
        
        tooltipIsOpen = true;
        
        currentTooltip.GetComponent<CanvasGroup>().DOFade(1, 0.5f)
            .OnComplete(() =>
            {
                isCurrentlyWorking = false;
            });
        
    }

    public void CloseTooltip()
    {
        if (printLog) Debug.Log("|-------------- TOOLTIP --------------|");
        if (printLog) Debug.Log($"Tooltip is closing.");
        
        isCurrentlyWorking = true;
        
        currentTooltip.GetComponent<CanvasGroup>().DOFade(0, 0.5f)
            .OnComplete(() =>
            {
                currentCanvas.planeDistance = 100;
                currentCanvas.sortingOrder = 0;
                currentTooltip.SetActive(false);
                isCurrentlyWorking = false;
                tooltipIsOpen = false;
            });
        
        
    }

    //Called by InteractionArea if something has been bought while open
    public void UpdateTooltip(InteractionAreaType interactionArea)
    {
        var updatedTooltipData = GetTooltipData(interactionArea);
        Transform tooltipContent = currentTooltip.transform.GetChild(1);
        tooltipContent.GetChild(1).gameObject.GetComponent<TMP_Text>().text = updatedTooltipData.areaDescription;
    }

    private TooltipData GetTooltipData(InteractionAreaType interactionArea)
    {
        TooltipData data = new TooltipData();

        switch (interactionArea)
        {
            case InteractionAreaType.Workshop:
                data = workshopComponent.GetTooltipData();
                currentCanvas = workshopCanvas;
                currentTooltip = workshopTooltip;
                break;
        }
        
        return data;
    }

}
