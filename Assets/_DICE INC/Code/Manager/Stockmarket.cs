using System;
using System.Collections;
using System.Collections.Generic;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class Stockmarket : InteractionArea
{
    
    
   
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Stockmarket;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;

    [SerializeField] private TMP_Text stockValueTMP;

    [TitleGroup("Settings")] 
    [SerializeField] private float timeBetweenUpdates = 0.5f;
    [SerializeField] private float startUpperRange;
    [SerializeField] private float startLowerRange;
    
    [Header("Marketing")]
    [SerializeField] private int costMarketingBase;
    [SerializeField] private float costMarketingMultiplier;
    [SerializeField] private float upperRangeIncrease;
    [ShowInInspector, ReadOnly] private float currentUpperRange;
    
    [Header("Bottomline")]
    [SerializeField] private int costBottomlineBase;
    [SerializeField] private float costBottomlineMultiplier;
    [SerializeField] private float lowerRangeIncrease;
    [ShowInInspector, ReadOnly] private float currentLowerRange;
    
    [Header("Progress")] 
    [SerializeField] private int marketingToUnlockBottomline;

    
    private bool stockmarketCycleActive;
    private double currentStockValue = 1f;
    
   
    
    
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
        
        costs.Add(costMarketingBase);
        costs.Add(costBottomlineBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(costMarketingMultiplier);
        costs.Add(costBottomlineMultiplier);
        
        return costs;
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
    
    void CheckProgress()
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
        bool lastTimeUp = true;
        while (stockmarketCycleActive)
        {
            
            float nextValueChange = Random.Range(currentLowerRange, currentUpperRange);
            currentStockValue += nextValueChange;
            stockValueTMP.text = currentStockValue.ToString("F2");
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
        
    }
    
    #endregion

}
