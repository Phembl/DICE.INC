using System;
using System.Collections;
using System.Collections.Generic;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Shapes;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class Stockmarket : InteractionArea
{
    
    
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Stockmarket;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;

    [SerializeField] private TMP_Text stockValueTMP;
    [SerializeField] private Transform stockValueEntryHolder;
    [SerializeField] private GameObject stockValueEntryPrefab;

    [TitleGroup("Stockmarket")] 
    [SerializeField] private float timeBetweenUpdates;
    [SerializeField] private float startUpperRange;
    [SerializeField] private float startLowerRange;
    
    [Header("Marketing")]
    [SerializeField] private int marketingCostBase;
    [SerializeField] private float marketingCostMult;
    [SerializeField] private int marketingMax;
    [Space]
    [SerializeField] private float upperRangeIncrease;
    [ShowInInspector, ReadOnly] private float currentUpperRange;
    [ShowInInspector, ReadOnly] private float chanceToRaise = 50f;
    
    [Header("Bottomline")]
    [SerializeField] private int bottomlineCostBase;
    [SerializeField] private float bottomlineCostMult;
    [SerializeField] private int bottomlineMax;
    [Space]
    [SerializeField] private float lowerRangeIncrease;
    [ShowInInspector, ReadOnly] private float currentLowerRange;
    
    [Header("Progress")] 
    [SerializeField] private int marketingToUnlockBottomline;

    
    private bool stockmarketCycleActive;
    private float currentStockValue = 1f;
    
   
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        stockValueTMP.text = currentStockValue.ToString();
        
        currentUpperRange = startUpperRange;
        currentLowerRange = startLowerRange;
        
        if (!stockmarketCycleActive)
        {
            StartCoroutine(StockmarketCycle());
        }
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(marketingCostBase);
        costs.Add(bottomlineCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(marketingCostMult);
        costs.Add(bottomlineCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(marketingMax);
        max.Add(bottomlineMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, index);
        
        switch (index)
        {
            case 0: //Marketing
                CheckProgress();
                currentUpperRange = startUpperRange + (upperRangeIncrease * count);
                break;
            
            case 1: //Bottomline
                currentLowerRange = startLowerRange + (lowerRangeIncrease * count);
                break;
            
        }
    }
    
    protected override void CheckProgress()
    {
        
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, 0) > marketingToUnlockBottomline &&
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 1))
        {
            UnlockInteractor(1);
        }
        
    }
    
    private IEnumerator StockmarketCycle()
    {
        stockmarketCycleActive = true;
        
        GameObject nextEntry = Instantiate(stockValueEntryPrefab, stockValueEntryHolder);
        
        while (stockmarketCycleActive)
        {
            
            yield return new WaitForSeconds(timeBetweenUpdates);
           
            if (stockValueEntryHolder.childCount > 19) Destroy(stockValueEntryHolder.GetChild(0).gameObject);
                
            foreach (Transform nextEntryHolder in stockValueEntryHolder)
            {
                Vector3 entryTarget = new Vector3(nextEntryHolder.localPosition.x - 20, nextEntryHolder.localPosition.y, nextEntryHolder.localPosition.z);
                nextEntryHolder.localPosition = entryTarget;
            }
                
            //Roll next stock value within range
            float nextValueChange = Random.Range(currentLowerRange, currentUpperRange);
            currentStockValue += nextValueChange;
            
            if (currentStockValue <= 0) currentStockValue = 0;
            CPU.instance.ChangeDiceRollStockValue(currentStockValue);
            
            stockValueTMP.text = currentStockValue.ToString("F2");
            
            //Next Entry
            nextEntry = Instantiate(stockValueEntryPrefab, stockValueEntryHolder);
            float nextEntryY = (float)Math.Round(currentStockValue * 100f);
            Vector3 nextEntryPosition = new Vector3(0, nextEntryY, 0);
            nextEntry.transform.localPosition = nextEntryPosition;
            
            //Draw Line
            Vector3 lastEntryPosition = stockValueEntryHolder.GetChild(nextEntry.transform.GetSiblingIndex() - 1).localPosition;
            Vector3 lineEnd = new Vector3(lastEntryPosition.x, lastEntryPosition.y - nextEntryPosition.y, lastEntryPosition.z);
            nextEntry.GetComponent<Line>().End = lineEnd;
            
            
            //TODO: Add some form of Zoom Effect so that the stock display can't go through Screen top
            
            //TODO: Introduce random shift events, like market crash
            
        }
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        int marketingCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, 0);
        int bottomlineCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, 1);

        float currentUpperRangePercentage = currentUpperRange * 100f;
        if (marketingCount == 0)
        {
            currentUpperRangePercentage = 0f;
        }
        data.areaTitle = "Stockmarket";
        data.areaDescription = $"The stock market changes the value of dice rolls." +
                               $"<br>Currently, every dice roll it worth {(float)Math.Round(currentStockValue * 100f)}%. " +
                               $"<br>The market is updated every {timeBetweenUpdates} seconds." +
                               $"<br><br>MARKETING: Increases the upper limit of the stock value shift to {currentUpperRangePercentage}%.";
        
        string bottomlineText = $"<br><br>??? (Marketing to unlock: {marketingToUnlockBottomline})";
        
       
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 1))
        {
            float currentLowerRangePercentage = currentLowerRange * 100f;
            if (bottomlineCount == 0)
            {
                currentLowerRangePercentage = 0f;
            }
            bottomlineText = $"<br><br>BOTTOMLINE: Increases the lower limit of the stock value shift to {currentLowerRangePercentage}%";
        }
        
        data.areaDescription += bottomlineText;
        
        return data;
    }
    #endregion

}
