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
    [SerializeField] private Transform displayLineHolder;
    [SerializeField] private GameObject stockValueEntryPrefab;
    [SerializeField] private StockChangeTable changeTable;
    
    [TitleGroup("Stock market")] 
    [SerializeField] private float timeBetweenUpdates;
    [Space]
    [SerializeField] private float growChanceBase = 15f;
    [ShowInInspector, ReadOnly] private float growChanceCurrent;
    [Space]
    [SerializeField] private float marketcrashChanceIncreaseBase;
    [ShowInInspector, ReadOnly] private float marketcrashChanceIncreaseCurrent;
    [ShowInInspector, ReadOnly] private float marketcrashChanceCurrent;
    
    [Header("Growth Stock")]
    [SerializeField] private int growthstockCostBase;
    [SerializeField] private float growthstockCostMult;
    [SerializeField] private int growthstockMax;
    [Space]
    [SerializeField] private float growChanceIncrease;

    [ShowInInspector, ReadOnly] private float growthstockCurrent;
    
    [Header("Market Cap")]
    [SerializeField] private int marketcapCostBase;
    [SerializeField] private float marketcapCostMult;
    [SerializeField] private int marketcapMax;
    [Space]
    [SerializeField] private float marketcapIncrease;
    [ShowInInspector, ReadOnly] private float marketcapBase = 2f;
    [ShowInInspector, ReadOnly] private float marketcapCurrent;
    
    private bool stockmarketCycleActive;
    private float stockValueCurrent = 1f;

    private float marketcapDisplayMultBase = 150f;
    private float marketcapDisplayReduction = 50f;
    private float marketcapDisplayMultCurrent;
    
        
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        stockValueTMP.text = stockValueCurrent.ToString();
        
        growChanceCurrent = growChanceBase;
        marketcrashChanceIncreaseCurrent = marketcrashChanceIncreaseBase;
        marketcapCurrent = marketcapBase;
        marketcapDisplayMultCurrent = marketcapDisplayMultBase;
        
        UnlockInteractor(0); //Unlock Growth Stock
    }

    protected override void OnAreaUnlock()
    {
        if (!stockmarketCycleActive)
        {
            StartCoroutine(StockmarketCycle());
        }
    }

    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(growthstockCostBase);
        costs.Add(marketcapCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(growthstockCostMult);
        costs.Add(marketcapCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(growthstockMax);
        max.Add(marketcapMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, index);
        
        switch (index)
        {
            case 0: //Growth Stock
                growthstockCurrent = growChanceIncrease * count;
                growChanceCurrent = growChanceBase + growthstockCurrent;
                CheckProgress();
                break;
            
            case 1: //Market Cap
                marketcapCurrent = marketcapBase + (marketcapIncrease * count);
                IncreaseMarketCap();
                break;
            
        }
    }

    void IncreaseMarketCap()
    {
        int marketcaplevel = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Stockmarket, 1);
        float reductionValue = marketcapDisplayReduction / marketcaplevel;
        foreach (Transform displayLine in displayLineHolder)
        {
            displayLine.localPosition = 
                new Vector3(displayLine.localPosition.x, 
                    displayLine.localPosition.y - reductionValue * (displayLine.GetSiblingIndex() + 1), 
                    0);
        }

        marketcapDisplayMultCurrent = displayLineHolder.GetChild(0).localPosition.y;
        
        if (stockValueEntryHolder.childCount > 0)
        {
            foreach (Transform nextEntryHolder in stockValueEntryHolder)
            {
                nextEntryHolder.localPosition = new Vector3(nextEntryHolder.localPosition.x, 
                    nextEntryHolder.localPosition.y - reductionValue, 
                    0);
            }
        }
    }
    
    protected override void CheckProgress()
    {
        
    }
    
    private IEnumerator StockmarketCycle()
    {
        stockmarketCycleActive = true;

        while (stockmarketCycleActive)
        {

            yield return new WaitForSeconds(timeBetweenUpdates);

            if (printLog) Debug.Log("|-------------- STOCKMARKET UPDATE --------------|");
            if (printLog) Debug.Log($"Current stock value: {stockValueCurrent}");
            if (printLog) Debug.Log($"Current grow chance: {growChanceCurrent}%");
            if (printLog) Debug.Log($"Current market crash chance: {marketcrashChanceCurrent}%");
            if (printLog) Debug.Log($"Current market crash chance growth: {marketcrashChanceIncreaseCurrent}.");

            if (stockValueEntryHolder.childCount > 19) Destroy(stockValueEntryHolder.GetChild(0).gameObject);
            if (stockValueEntryHolder.childCount > 0)
            {
                foreach (Transform nextEntryHolder in stockValueEntryHolder)
                {
                    Vector3 entryTarget = new Vector3(nextEntryHolder.localPosition.x - 19, nextEntryHolder.localPosition.y, nextEntryHolder.localPosition.z);
                    nextEntryHolder.localPosition = entryTarget;
                }
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
                if (Utility.Roll(growChanceCurrent))
                {
                    //Roll next stock value within range
                    float nextChange = changeTable.GetChangeValue();
                    stockValueCurrent += stockValueCurrent * nextChange;
                    
                    //TODO: Build some rescue mechanism in case the stock has crashed to low and can't get up again.
                }
                
                
                if (printLog) Debug.Log($"Value now {stockValueCurrent}");
            }
            
         
            if (stockValueCurrent >= marketcapCurrent) 
            {
                stockValueCurrent = marketcapCurrent; //Market Cap
                
                //If the marketCap interactor is not unlocked, it is unlocked here
                if (!CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 1))
                {
                    UnlockInteractor(1);
                }
            }
            
            else if (stockValueCurrent <= 0) stockValueCurrent = 0; //Not below Zero
            
            stockValueTMP.text = stockValueCurrent.ToString("F3");
            
            //Next Entry
            GameObject nextEntry = Instantiate(stockValueEntryPrefab, stockValueEntryHolder);
            float nextEntryY = (float)Math.Round(stockValueCurrent * marketcapDisplayMultCurrent);
            Vector3 nextEntryPosition = new Vector3(0, nextEntryY, 0);
            nextEntry.transform.localPosition = nextEntryPosition;
            
            //Draw Line
            if (stockValueEntryHolder.childCount > 1)
            {
                Vector3 lastEntryPosition = stockValueEntryHolder.GetChild(nextEntry.transform.GetSiblingIndex() - 1).localPosition;
                Vector3 lineEnd = new Vector3(lastEntryPosition.x, lastEntryPosition.y - nextEntryPosition.y, lastEntryPosition.z);
                nextEntry.GetComponent<Line>().End = lineEnd;
            }
            
            
            //Update Tooltip if StockMarket TT is open
            if (TooltipManager.instance.GetTooltipStatus() && TooltipManager.instance.GetCurrentInteractionArea() == InteractionAreaType.Stockmarket)
            {
                TooltipManager.instance.UpdateTooltip(InteractionAreaType.Stockmarket);
            }
            
        }
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = "Stockmarket";
        data.areaDescription = "The stock market changes the value of dice rolls." +
                               $"<br>Currently, every dice roll it worth <b>{(float)Math.Round(stockValueCurrent * 100f)}%</b>. " +
                               $"<br>The market is updated every <b>{timeBetweenUpdates} seconds.</b>" +
                               $"<br>Current chance for the market to crash: <b>{marketcrashChanceCurrent.ToString("F2")}%</b>.";

        //Growth Stock TT
        string growthstockTooltip = $"<br><br><b>GROWTH STOCK:</b> Each point increases the chance for growth by <b>{growChanceIncrease.ToString("F2")}%</b>. " +
                                    $"<br>Current growth chance: <b>{growChanceCurrent}%</b>.";
        
        
        //Market Cap TT
        string marketcapTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 1))
        {
            
            marketcapTooltip = $"<br><br><b>MARKET CAP:</b> Each point increases the maximum stock value by <b>{marketcapIncrease*100}%.</b>" +
                             $"<br>Current value max: <b>{marketcapCurrent}</b>.";
        }
        
        data.areaDescription += growthstockTooltip + marketcapTooltip;
        
        return data;
    }
    #endregion

}
