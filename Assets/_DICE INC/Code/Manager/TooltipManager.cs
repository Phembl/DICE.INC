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
    [Header("Import")]
    [SerializeField] private Import importManager;
    [SerializeField] private Canvas importCanvas;
    [SerializeField] private GameObject importTooltip;
    [Header("Lab")]
    [SerializeField] private Lab labManager;
    [SerializeField] private Canvas labCanvas;
    [SerializeField] private GameObject labTooltip;
    [Header("Factory")]
    [SerializeField] private Factory factoryManager;
    [SerializeField] private Canvas factoryCanvas;
    [SerializeField] private GameObject factoryTooltip;
    [Header("Transformer")]
    [SerializeField] private Transformer transformerManager;
    [SerializeField] private Canvas transformerCanvas;
    [SerializeField] private GameObject transformerTooltip;
    [Header("Technology")]
    [SerializeField] private Technology technologyManager;
    [SerializeField] private Canvas technologyCanvas;
    [SerializeField] private GameObject technologyTooltip;
    [Header("Stockmarket")]
    [SerializeField] private Stockmarket stockmarketManager;
    [SerializeField] private Canvas stockmarketCanvas;
    [SerializeField] private GameObject stockmarketTooltip;
    [Header("Datacenter")]
    [SerializeField] private Datacenter datacenterManager;
    [SerializeField] private Canvas datacenterCanvas;
    [SerializeField] private GameObject datacenterTooltip;
    

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
        factoryTooltip.SetActive(false);
    }

    public void OpenTooltip(InteractionAreaType interactionArea)
    {
        if (printLog) Debug.Log("|-------------- TOOLTIP --------------|");
        if (printLog) Debug.Log($"{interactionArea.ToString()} Tooltip is opened.");
        
        isCurrentlyWorking = true;
        currentInteractionAreaType = interactionArea;
        
        var tooltipData = GetTooltipData(interactionArea);
        
        currentCanvas.planeDistance = 90;
        currentCanvas.sortingOrder = 2;
        
        currentTooltip.GetComponent<CanvasGroup>().alpha = 0;
        currentTooltip.SetActive(true);
        
        Transform tooltipContent = currentTooltip.transform.GetChild(1);
        tooltipContent.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tooltipData.areaTitle;
        tooltipContent.GetChild(1).gameObject.GetComponent<TMP_Text>().text = tooltipData.areaDescription;
        
        tooltipIsOpen = true;
        
        //Set Tooltip Collider Size (to block BackgroundCloser)
        Vector2 tooltipSize = tooltipContent.GetComponent<RectTransform>().sizeDelta;
        tooltipContent.gameObject.GetComponent<BoxCollider2D>().size = tooltipSize;
        
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
        
        Vector2 tooltipSize = tooltipContent.GetComponent<RectTransform>().sizeDelta;
        tooltipContent.gameObject.GetComponent<BoxCollider2D>().size = tooltipSize;
    }

    private TooltipData GetTooltipData(InteractionAreaType interactionArea)
    {
        TooltipData data = new TooltipData();

        switch (interactionArea)
        {
            case InteractionAreaType.Import:
                data = importManager.GetTooltipData();
                currentCanvas = importCanvas;
                currentTooltip = importTooltip;
                break;
            
            case InteractionAreaType.Lab:
                data = labManager.GetTooltipData();
                currentCanvas = labCanvas;
                currentTooltip = labTooltip;
                break;
            
            case InteractionAreaType.Factory:
                data = factoryManager.GetTooltipData();
                currentCanvas = factoryCanvas;
                currentTooltip = factoryTooltip;
                break;
            case InteractionAreaType.Transformer:
                data = transformerManager.GetTooltipData();
                currentCanvas = transformerCanvas;
                currentTooltip = transformerTooltip;
                break;
            
            case InteractionAreaType.Technology:
                data = technologyManager.GetTooltipData();
                currentCanvas = technologyCanvas;
                currentTooltip = technologyTooltip;
                break;
            
            case InteractionAreaType.Stockmarket:
                data = stockmarketManager.GetTooltipData();
                currentCanvas = stockmarketCanvas;
                currentTooltip = stockmarketTooltip;
                break;
            
            case InteractionAreaType.Datacenter:
                data = datacenterManager.GetTooltipData();
                currentCanvas = datacenterCanvas;
                currentTooltip = datacenterTooltip;
                break;
         
        }
        
        return data;
    }

}
