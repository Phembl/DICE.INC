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
        Vector2 lastEntryPosition = new Vector2(0, 0);
        
        while (stockmarketCycleActive)
        {
            
            yield return new WaitForSeconds(timeBetweenUpdates);
           
            if (stockValueEntryHolder.childCount > 19) Destroy(stockValueEntryHolder.GetChild(0).gameObject);
                
            foreach (Transform nextEntryHolder in stockValueEntryHolder)
            {
                Vector2 entryTarget = new Vector2(nextEntryHolder.localPosition.x - 20, nextEntryHolder.localPosition.y);
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
            Vector2 nextEntryPosition = new Vector2(0, (currentStockValue * 100)-100);
            nextEntry.transform.localPosition = nextEntryPosition;
            
            lastEntryPosition = stockValueEntryHolder.GetChild(nextEntry.transform.GetSiblingIndex() - 1).localPosition;
            Vector2 lineEnd = new Vector2(lastEntryPosition.x, lastEntryPosition.y - nextEntryPosition.y);
            nextEntry.GetComponent<Line>().End = lineEnd;
            
            //TODO: Add some form of Zoom Effect so that the stock display can't go through Screen top
            
        }
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        return data;
    }
    #endregion

}
