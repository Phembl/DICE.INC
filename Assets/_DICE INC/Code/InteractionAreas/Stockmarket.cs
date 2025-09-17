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
    [SerializeField] private StockChangeTable changeTable;
    

    [TitleGroup("Stockmarket")] 
    [SerializeField] private float timeBetweenUpdates;
    [Space]
    [SerializeField] private float marketcrashChanceIncreaseBase;
    [ShowInInspector, ReadOnly] private float marketcrashChanceIncreaseCurrent;
    [ShowInInspector, ReadOnly] private float marketcrashChanceCurrent;
    
    [Header("Marketing")]
    [SerializeField] private int marketingCostBase;
    [SerializeField] private float marketingCostMult;
    [SerializeField] private int marketingMax;
    [Space]
    [SerializeField] private float growChanceIncrease;
    [ShowInInspector, ReadOnly] private float growChanceBase = 30f;
    [ShowInInspector, ReadOnly] private float growChanceCurrent;
    [ShowInInspector, ReadOnly] private float marketingGrowth;
    
    [Header("Bottomline")]
    [SerializeField] private int bottomlineCostBase;
    [SerializeField] private float bottomlineCostMult;
    [SerializeField] private int bottomlineMax;
    [Space]
    [SerializeField] private float bottomlineCrashChanceDecrease;
    [ShowInInspector, ReadOnly] private float bottomlineCrashChanceDecreaseCurrent;
    
    [Header("Progress")] 
    [SerializeField] private int marketingToUnlockBottomline;

    
    private bool stockmarketCycleActive;
    private float stockValueCurrent = 1f;
    
   
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        stockValueTMP.text = stockValueCurrent.ToString();
        
        growChanceCurrent = growChanceBase;
        marketcrashChanceIncreaseCurrent = marketcrashChanceIncreaseBase;
        
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
                marketingGrowth = growChanceIncrease * count;
                growChanceCurrent = growChanceBase + marketingGrowth;
                break;
            
            case 1: //Bottomline
                bottomlineCrashChanceDecreaseCurrent = (bottomlineCrashChanceDecrease * count);
                marketcrashChanceIncreaseCurrent = marketcrashChanceIncreaseBase - (marketcrashChanceIncreaseBase * (bottomlineCrashChanceDecreaseCurrent/100));
                break;
            
        }
    }
    
    protected override void CheckProgress()
    {
        
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, 0) >= marketingToUnlockBottomline &&
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
            
            if (printLog) Debug.Log("|-------------- STOCKMARKET UPDATE --------------|");
            if (printLog) Debug.Log($"Current stock value: {stockValueCurrent}");
            if (printLog) Debug.Log($"Current grow chance: {growChanceCurrent}%");
            if (printLog) Debug.Log($"Current market crash chance: {marketcrashChanceCurrent}%");
            if (printLog) Debug.Log($"Current market crash chance growth: {marketcrashChanceIncreaseCurrent}.");
           
            if (stockValueEntryHolder.childCount > 19) Destroy(stockValueEntryHolder.GetChild(0).gameObject);
                
            foreach (Transform nextEntryHolder in stockValueEntryHolder)
            {
                Vector3 entryTarget = new Vector3(nextEntryHolder.localPosition.x - 20, nextEntryHolder.localPosition.y, nextEntryHolder.localPosition.z);
                nextEntryHolder.localPosition = entryTarget;
            }
                
            //Roll for Marketcrash
            if (Utility.Roll(marketcrashChanceCurrent)) //Market Crash
            {
                marketcrashChanceCurrent = 0f;
                float crashDivider = Random.Range(1f, 2f);
                stockValueCurrent /= crashDivider;
                if (printLog) Debug.Log($"Marketcrash! Value has been divided by {crashDivider} and is now {stockValueCurrent}");
            }
            else //No market Crash
            {
                marketcrashChanceCurrent += marketcrashChanceIncreaseCurrent;
                
               //Check if the value growths
                if (Utility.Roll(growChanceCurrent)) //Value Decreases
                {
                    //Roll next stock value within range
                    float nextChange = changeTable.GetChangeValue();
                    stockValueCurrent += nextChange;
                }
                
                
                if (printLog) Debug.Log($"Value now {stockValueCurrent}");
            }
            
         
            
            if (stockValueCurrent <= 0) stockValueCurrent = 0;
            stockValueTMP.text = stockValueCurrent.ToString("F3");
            
            //Next Entry
            nextEntry = Instantiate(stockValueEntryPrefab, stockValueEntryHolder);
            float nextEntryY = (float)Math.Round(stockValueCurrent * 100f);
            Vector3 nextEntryPosition = new Vector3(0, nextEntryY, 0);
            nextEntry.transform.localPosition = nextEntryPosition;
            
            //Draw Line
            Vector3 lastEntryPosition = stockValueEntryHolder.GetChild(nextEntry.transform.GetSiblingIndex() - 1).localPosition;
            Vector3 lineEnd = new Vector3(lastEntryPosition.x, lastEntryPosition.y - nextEntryPosition.y, lastEntryPosition.z);
            nextEntry.GetComponent<Line>().End = lineEnd;
            
            //Update Tooltip if StockMarket TT is open
            if (TooltipManager.instance.GetTooltipStatus() && TooltipManager.instance.GetCurrentInteractionArea() == InteractionAreaType.Stockmarket)
            {
                TooltipManager.instance.UpdateTooltip(InteractionAreaType.Stockmarket);
            }
            //TODO: Add some form of Zoom Effect so that the stock display can't go through Screen top
            
            //TODO: Introduce random shift events, like market crash
            
        }
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
       
        int bottomlineCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, 1);
        
        data.areaTitle = "Stockmarket";
        data.areaDescription = $"The stock market changes the value of dice rolls." +
                               $"<br>Currently, every dice roll it worth <b>{(float)Math.Round(stockValueCurrent * 100f)}%</b>. " +
                               $"<br>The market is updated every <b>{timeBetweenUpdates} seconds.</b>" +
                               $"<br>Current chance for the market to crash:<b>{marketcrashChanceCurrent.ToString("F2")}%</b>." +
                               $"<br><br><b>MARKETING:</b> Increases the chance for stock growth by <b>{marketingGrowth.ToString("F3")}</b>. " +
                               $"<br>Current growth chance:<b>{growChanceCurrent}%</b>.";
        
        //Bottomline TT
        string bottomlineText = $"<br><br>??? (Marketing to unlock: {marketingToUnlockBottomline})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 1))
        {
            string bottomlineCrashChanceDecrease = bottomlineCrashChanceDecreaseCurrent.ToString("F2");
            bottomlineText = $"<br><br><b>BOTTOMLINE:</b> Decreases the growth of market crash chance by <b>{bottomlineCrashChanceDecrease}%.</b>" +
                             $"<br>Current market crash chance growth:<b>{marketcrashChanceIncreaseCurrent.ToString("F4")}</b>.";
        }
        
        data.areaDescription += bottomlineText;
        
        return data;
    }
    #endregion

}
