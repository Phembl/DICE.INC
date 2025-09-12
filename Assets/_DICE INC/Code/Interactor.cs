using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DICEINC.Global;

public class Interactor : MonoBehaviour
{
    [SerializeField] private string interactorName;
    public string GetInteractorName() => interactorName;
    [Space]
    [SerializeField] private InteractionArea interactionArea;
    [SerializeField] private Resource costResource;
    [Space]
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteHovered;
    [Space]
    [SerializeField] private bool isShop;
    
    //Colors
    private Color colorNormal;
    private Color colorDark;
    private Color colorInactive;
    
    //Text
    private TMP_Text titleTMP;
    private TMP_Text costTMP;
    private Image background;
    
    //State Tracking
    private bool isHovered;
    private bool isUnlocked;
    private bool isPurchasable;
    private double currentCost;
    private int currentCount;

    private Coroutine clickAnim;
    
    void OnEnable()
    {
        CPU.OnPipsChanged += CheckAvailabilityPips;
        CPU.OnDiceChanged += CheckAvailabilityDice;
        CPU.OnToolsChanged += CheckAvailabilityTools;
        CPU.OnLuckChanged += CheckAvailabilityLuck;
        CPU.OnMDiceChanged += CheckAvailabilityMDice;
        CPU.OnDataChanged += CheckAvailabilityData;
    }

    void OnDisable()
    {
        CPU.OnPipsChanged -= CheckAvailabilityPips;
        CPU.OnDiceChanged -= CheckAvailabilityDice;
        CPU.OnToolsChanged -= CheckAvailabilityTools;
        CPU.OnLuckChanged -= CheckAvailabilityLuck;
        CPU.OnMDiceChanged -= CheckAvailabilityMDice;
        CPU.OnDataChanged -= CheckAvailabilityData;
    }

    
    public void InitializeInteractor(int _currentCost)
    {
        
        colorNormal = Settings.instance.colorNormal;
        colorDark = Settings.instance.colorDark;
        colorInactive = Settings.instance.colorInactive;
        
        background = GetComponent<Image>();
        titleTMP = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        costTMP = transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        
        background.sprite = spriteNormal;
        
        currentCost = _currentCost;
        
        //Initially every Interactor is locked
        titleTMP.text = "???";
        costTMP.text = "";
            
        titleTMP.color = colorInactive;
        costTMP.color = colorInactive;
        background.color = colorInactive;

        isUnlocked = false;
        
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
    
    void CheckAvailabilityTools()
    {
        if (costResource != Resource.Tools) return;
        
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
    
    void CheckAvailability()
    {
        if (!isUnlocked) return;
            
        switch (costResource)
        {
            case Resource.Pips:
                if (currentCost > CPU.instance.GetPips()) isPurchasable = false;
                else isPurchasable = true;
                break;
            
            case Resource.Tools:
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
    
        
        if (isPurchasable)
        {
            if (!isHovered)
            {
                titleTMP.color = colorNormal;
                costTMP.color = colorNormal;
                background.color = colorNormal;
            }
            
        }

        else
        {
            if (clickAnim != null)  StopCoroutine(clickAnim);
            background.sprite = spriteNormal;
            background.color = colorInactive;
            titleTMP.color = colorInactive;
            costTMP.color = colorInactive;
        }
   
    }
    
    #endregion
    
    void OnMouseOver()
    {
        if (isHovered || !isPurchasable || !isUnlocked) return;
        
        isHovered = true;
        
        background.sprite = spriteHovered;
        titleTMP.color = colorDark;
        costTMP.color = colorDark;
        
    }

    void OnMouseExit()
    {
        if (!isHovered) return;
        
        isHovered = false;

        if (isPurchasable)
        {
            background.sprite = spriteNormal;
            titleTMP.color = colorNormal;
            costTMP.color = colorNormal;
        }
       
    }

    void OnMouseDown()
    {
        if (!isPurchasable || !isUnlocked) return;
        
        if (clickAnim != null)  StopCoroutine(clickAnim);
        clickAnim = StartCoroutine(ClickAnimation());

        int myIndex = transform.GetSiblingIndex();
        interactionArea.Interaction(myIndex, costResource);
    }

    private IEnumerator ClickAnimation()
    {
        
        background.color = colorDark;
        titleTMP.color = colorNormal;
        costTMP.color = colorNormal;
        
        yield return new WaitForSeconds(0.1f);
        
        background.color = colorNormal;
        titleTMP.color = colorDark;
        costTMP.color = colorDark;
    }

    #region |-------------- UPDATING --------------|
    //Run By Interaction Parent (Shop, workshop, etc.)
    public void UpdatePrice(double _newCost)
    {
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
        
        //Show count only if not Shop Interactor
        if (isShop) titleTMP.text = interactorName;
        else titleTMP.text = $"{interactorName}({currentCount.ToString()})";
    }
    #endregion
}
