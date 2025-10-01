using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DICEINC.Global;
using Sirenix.OdinInspector;

public class Interactor : MonoBehaviour
{
    [SerializeField] private string interactorName;
    public string GetInteractorName() => interactorName;
    [Space]
    [SerializeField] private InteractionArea interactionArea;
    [SerializeField] private Resource costResource;
    [Space]
    [ShowInInspector, ReadOnly] private int maxCount;
    [Space]
    [SerializeField] private bool dontShowCount;
    
    
    //Colors
    private Color colorActive;
    //private Color colorDark;
    private Color colorInactive;
    private Color colorScreen;
    
    //Text
    private TMP_Text titleTMP;
    private TMP_Text costTMP;
    private Image background;
    
    //State Tracking
    private bool isHovered;
    private bool isUnlocked;
    private bool isMaxed;
    private bool isPurchasable;
    private double currentCost;
    private int currentCount;

    private Coroutine clickAnim;

    //Special Case
    private bool isAIWorker;
    
    void OnEnable()
    {
        CPU.OnPipsChanged += CheckAvailabilityPips;
        CPU.OnDiceChanged += CheckAvailabilityDice;
        CPU.OnMaterialChanged += CheckAvailabilityMaterial;
        CPU.OnLuckChanged += CheckAvailabilityLuck;
        CPU.OnMDiceChanged += CheckAvailabilityMDice;
        CPU.OnDataChanged += CheckAvailabilityData;
    }

    void OnDisable()
    {
        CPU.OnPipsChanged -= CheckAvailabilityPips;
        CPU.OnDiceChanged -= CheckAvailabilityDice;
        CPU.OnMaterialChanged -= CheckAvailabilityMaterial;
        CPU.OnLuckChanged -= CheckAvailabilityLuck;
        CPU.OnMDiceChanged -= CheckAvailabilityMDice;
        CPU.OnDataChanged -= CheckAvailabilityData;
    }

    
    public void InitializeInteractor(int _currentCost, int _maxCount)
    {
        
        colorActive = SettingsManager.instance.colorNormal;
        colorInactive = SettingsManager.instance.colorInactive;
        
        background = GetComponent<Image>();
        colorScreen = background.color;
        
        titleTMP = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        costTMP = transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        
        
        currentCost = _currentCost;
        maxCount = _maxCount;
        
        //Initially every Interactor is locked
        titleTMP.text = "???";
        costTMP.text = "";
            
        titleTMP.color = colorInactive;
        costTMP.color = colorInactive;

        isUnlocked = false;
        
        if (interactorName == "ai worker") isAIWorker = true;
        
        Debug.Log($"Interactor: {interactorName} is initialized.");
    }

    public void Unlock()
    {
        Debug.Log($"{interactorName} is unlocked.");
        
       isUnlocked = true;
       
       UpdateCount(0);
       UpdatePrice(currentCost);
    }

    #region |-------------- AVAILABILIY --------------|

    void CheckAvailabilityPips()
    {
        if (costResource != Resource.Pips) return;
        
        CheckAvailability();
    }
    
    void CheckAvailabilityDice()
    {
        if (costResource != Resource.Dice) return;
        
        CheckAvailability();
    }
    
    void CheckAvailabilityMaterial()
    {
        if (costResource != Resource.Material) return;
        
        CheckAvailability();
    }
    
    void CheckAvailabilityLuck()
    {
        if (costResource != Resource.Luck) return;
        
        CheckAvailability();
    }
    
    void CheckAvailabilityMDice()
    {
        if (costResource != Resource.mDice) return;
        
        CheckAvailability();
    }
    
    void CheckAvailabilityData()
    {
        if (costResource != Resource.Data) return;
        
        CheckAvailability();
    }
    
    public void CheckAvailability()
    {
        if (!isUnlocked) return;
            
        switch (costResource)
        {
            case Resource.Pips:
                if (currentCost > CPU.instance.GetPips()) isPurchasable = false;
                else isPurchasable = true;
                break;
            
            case Resource.Material:
                if (currentCost > CPU.instance.GetTools()) isPurchasable = false;
                else isPurchasable = true;
                break;
            
            case Resource.Luck:
                if (currentCost > CPU.instance.GetLuck()) isPurchasable = false;
                else isPurchasable = true;
                break;
            
            case Resource.mDice:
                if (currentCost > CPU.instance.GetMDice()) isPurchasable = false;
                else isPurchasable = true;
                break;
            
            case Resource.Data:
                if (currentCost > CPU.instance.GetData()) isPurchasable = false;
                else isPurchasable = true;
                break;
        }

        //AI Worker needs to additionally check if a normal worker is available to be purchasable
        if (isAIWorker)
        {
            if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,0) <= 0) isPurchasable = false;
        }
        
        if (isPurchasable)
        {
            if (!isHovered)
            {
                titleTMP.color = colorActive;
                costTMP.color = colorActive;
                background.color = colorScreen;
            }
            
        }

        else
        {
            if (clickAnim != null)  StopCoroutine(clickAnim);
            background.color = colorScreen;
            titleTMP.color = colorInactive;
            costTMP.color = colorInactive;
        }
   
    }
    
    #endregion
    
    void OnMouseOver()
    {
        if (isHovered || !isPurchasable || !isUnlocked || isMaxed) return;
        
        isHovered = true;
        
        background.color = colorActive;
        titleTMP.color = colorScreen;
        costTMP.color = colorScreen;
        
    }

    void OnMouseExit()
    {
        if (!isHovered) return;
        
        isHovered = false;

        if (isPurchasable)
        {
            background.color = colorScreen;
            titleTMP.color = colorActive;
            costTMP.color = colorActive;
        }
       
    }

    void OnMouseDown()
    {
        if (!isPurchasable || !isUnlocked || isMaxed) return;
        
        if (clickAnim != null)  StopCoroutine(clickAnim);
        clickAnim = StartCoroutine(ClickAnimation());

        int myIndex = transform.GetSiblingIndex();
        interactionArea.Interaction(myIndex, costResource);
    }

    private IEnumerator ClickAnimation()
    {
        background.DOFade(0.7f, 0.05f);
        
        yield return new WaitForSeconds(0.1f);
        
        background.DOFade(1f, 0.05f);
    }

    #region |-------------- UPDATING --------------|
    //Run By Interaction Parent (Import, workshop, etc.)
    public void UpdatePrice(double _newCost)
    {
        if (isMaxed) return;
        
        currentCost = _newCost;
        //The internal price should be updatable, even if locked (only used by X 100)
        if (!isUnlocked) return;
        
        string displayPrice = Utility.ShortenNumberToString(currentCost);
        costTMP.richText = false;
        costTMP.SetText($"{displayPrice} {costResource}");
        
        CheckAvailability();
    }

    public void UpdateCount(int _newCount)
    {
        currentCount = _newCount;
        
        if (currentCount >= maxCount && maxCount > 0)
        {
            isMaxed = true;
            currentCount = maxCount;
            costTMP.SetText("MAX");
        }
        
        //Show count only if not Import Interactor
        if (dontShowCount) titleTMP.text = interactorName;
        else titleTMP.text = $"{interactorName}({currentCount.ToString()})";
    }
    #endregion
}
