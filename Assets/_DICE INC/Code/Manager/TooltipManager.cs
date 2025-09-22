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
    [Header("Factory")]
    [SerializeField] private Factory factoryManager;
    [SerializeField] private Canvas workshopCanvas;
    [SerializeField] private GameObject workshopTooltip;
    [Header("Transformer")]
    [SerializeField] private Casino casinoManager;
    [SerializeField] private Canvas casinoCanvas;
    [SerializeField] private GameObject casinoTooltip;
    [Header("Dieworld")]
    [SerializeField] private Diceworld diceworldManager;
    [SerializeField] private Canvas diceworldCanvas;
    [SerializeField] private GameObject diceworldTooltip;
    [Header("Stockmarket")]
    [SerializeField] private Stockmarket stockmarketManager;
    [SerializeField] private Canvas stockmarketCanvas;
    [SerializeField] private GameObject stockmarketTooltip;
    

    private InteractionArea currentInteractionArea;
    
    private InteractionAreaType currentInteractionAreaType;
    public InteractionAreaType GetCurrentInteractionArea() => currentInteractionAreaType;
    
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
        currentInteractionAreaType = interactionArea;
        
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
                currentInteractionAreaType = InteractionAreaType.None;
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
            case InteractionAreaType.Factory:
                data = factoryManager.GetTooltipData();
                currentCanvas = workshopCanvas;
                currentTooltip = workshopTooltip;
                break;
            
            case InteractionAreaType.Stockmarket:
                data = stockmarketManager.GetTooltipData();
                currentCanvas = stockmarketCanvas;
                currentTooltip = stockmarketTooltip;
                break;
            
            case InteractionAreaType.Diceworld:
                data = diceworldManager.GetTooltipData();
                currentCanvas = diceworldCanvas;
                currentTooltip = diceworldTooltip;
                break;
            
            case InteractionAreaType.Transformer:
                data = casinoManager.GetTooltipData();
                currentCanvas = casinoCanvas;
                currentTooltip = casinoTooltip;
                break;
        }
        
        return data;
    }

}
