using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class Workshop : InteractionArea
{
    #region |-------------- SETTINGS --------------|
    
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Workshop;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    [SerializeField] private Transform workshopLoadScreen;

    [TitleGroup("Workshop")] 
    [SerializeField] private float timerBase;
    [ShowInInspector, ReadOnly] private float timerCurrent;
    
    [Header("Dicemaker")]
    [SerializeField] private int dicemakerCostBase;
    [SerializeField] private float dicemakerCostMult;
    [SerializeField] private int dicemakerMax;
    [Space]
    [ShowInInspector, ReadOnly] private int dicemakerCurrent;
    
    [Header("Speed")]
    [SerializeField] private int speedCostBase;
    [SerializeField] private float speedCostMult;
    [SerializeField] private int speedMax;
    [Space]
    [SerializeField] private float speedIncrease;
    [ShowInInspector, ReadOnly] private float speedCurrent;
    
    [Header("Efficiency")]
    [SerializeField] private int efficiencyCostBase;
    [SerializeField] private float efficiencyCostMult;
    [SerializeField] private int efficiencyMax;
    [Space]
    [SerializeField] private float efficiencyIncrease;
    private float efficiencyBase = 1f;
    [Space]
    [ShowInInspector, ReadOnly] private float efficiencyCurrent;
    
    [Header("Critical")]
    [SerializeField] private int criticalCostBase;
    [SerializeField] private float criticalCostMult;
    [SerializeField] private int criticalMax;
    [Space]
    [SerializeField] private float criticalChanceBase;
    [SerializeField] private float criticalChanceIncrease;
    [SerializeField] private float criticalValueBase;
    [SerializeField] private float criticalValueIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private float criticalChanceCurrent;
    [ShowInInspector, ReadOnly] private float criticalValueCurrent;
    
    [Header("Overdrive")]
    [SerializeField] private int costOverdriveBase;
    [SerializeField] private float costOverdriveMultiplier;
    [SerializeField] private int overdriveMax;
    [Space]
    [SerializeField] private float overdriveChanceBase;
    [SerializeField] private float overdriveChanceIncrease;
    [SerializeField] private float overdriveValueBase;
    [SerializeField] private float overdriveValueIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private float overdriveChanceCurrent;
    [ShowInInspector, ReadOnly] private float overdriveValueCurrent;
    
    [Header("Progress")] 
    [SerializeField] private int dicemakerToUnlockSpeed;
    [SerializeField] private int dicemakerToUnlockEfficiency;
    [SerializeField] private int dicemakerToUnlockCritical;
    [SerializeField] private int dicemakerToUnlockOverdrive;
    
    #endregion
    
    private bool workshopCycleActive;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        timerCurrent = timerBase;
        efficiencyCurrent = efficiencyBase;
        
        foreach (Transform nextLoader in workshopLoadScreen)
        {
            nextLoader.GetComponent<Image>().DOFade(0, 0);
        }
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(dicemakerCostBase);
        costs.Add(speedCostBase);
        costs.Add(efficiencyCostBase);
        costs.Add(criticalCostBase);
        costs.Add(costOverdriveBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(dicemakerCostMult);
        costs.Add(speedCostMult);
        costs.Add(efficiencyCostMult);
        costs.Add(criticalCostMult);
        costs.Add(costOverdriveMultiplier);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(dicemakerMax);
        max.Add(speedMax);
        max.Add(efficiencyMax);
        max.Add(criticalMax);
        max.Add(overdriveMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, index);
        
        switch (index)
        {
            case 0: //Dicemaker
                dicemakerCurrent = count;
                CheckProgress();
                break;
            
            case 1: //Speed
                speedCurrent = speedIncrease * count;
                timerCurrent = timerBase - speedCurrent;
                timerCurrent = (float)Math.Round(timerCurrent, 2);
                break;
            
            case 2: //Efficiency
                efficiencyCurrent = efficiencyBase + (efficiencyIncrease * count);
                if (printLog) Debug.Log($"Workshop: Efficiency upgraded: Current value:{efficiencyCurrent}");
                break;
            
            case 3: //Critical
                criticalChanceCurrent = criticalChanceBase + (criticalChanceIncrease * count);
                criticalValueCurrent = criticalValueBase + (criticalValueIncrease * count);
                if (printLog) Debug.Log($"Workshop: Critical upgraded: Value:{criticalValueCurrent}, Chance:{criticalChanceCurrent}%");
                break;
            
            case 4: //Overdrive
                overdriveChanceCurrent = overdriveChanceBase + (overdriveChanceIncrease * count);
                overdriveValueCurrent = overdriveValueBase + (overdriveValueIncrease * count);
                if (printLog) Debug.Log($"Workshop: Overdrive upgraded: Value:{overdriveValueCurrent}, Chance:{overdriveChanceCurrent}%");
                break;
            
        }

        
    }
    
    protected override void CheckProgress()
    {
        if (!workshopCycleActive)
        {
            ResourceManager.instance.UnlockResource(Resource.Tools);
            StartCoroutine(WorkShopCycle());
        }
        
        int dicemakerCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, 0);
        
        //Unlock Speed
        if (dicemakerCount >= dicemakerToUnlockSpeed &&
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 1)) UnlockInteractor(1);
        
        //Unlock Efficiency
        if(dicemakerCount >= dicemakerToUnlockEfficiency &&
                !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 2)) UnlockInteractor(2);
        
        //Unlock Critical && Luck
        if (dicemakerCount >= dicemakerToUnlockCritical &&
                 !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 3))
        {
            UnlockInteractor(3);
            ResourceManager.instance.UnlockResource(Resource.Luck);
        }
        
        //Unlock Overdrive
        if (dicemakerCount >= dicemakerToUnlockOverdrive &&
                 !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 4))
        {
            UnlockInteractor(4);
        }
    }
    
    private IEnumerator WorkShopCycle()
    {
        workshopCycleActive = true;
        while (workshopCycleActive)
        {
            float nextProductionTimer = timerCurrent;
            
            if (printLog) Debug.Log("|--------------WORKSHOP PRODUCTION --------------|");
            if (printLog) Debug.Log($"{dicemakerCurrent * efficiencyCurrent} dice will be produced.");
            if (printLog) Debug.Log($"Critical Chance: {criticalChanceCurrent}.");
            if (printLog) Debug.Log($"Overdrive Chance: {overdriveChanceCurrent}.");

            
            //Roll for Overdrive
            if (Utility.Roll(overdriveChanceCurrent))
            {
                nextProductionTimer /= overdriveValueCurrent;
                if (printLog) Debug.Log($"Overdrive Production: current production cycle time is {nextProductionTimer}");
            }
            
            else
            {
                if (printLog) Debug.Log($"Current production cycle time is {nextProductionTimer}");
            }
            
            //Loading Screen (This is the actual Wait Time for production)
            for (int i = 0; i < 10; i++)
            {
                float singleLoadTimer = nextProductionTimer / 10;
                
                workshopLoadScreen.GetChild(i).gameObject.GetComponent<Image>().DOFade(1, singleLoadTimer);
                yield return new WaitForSeconds(singleLoadTimer);
            }
            
            int diceCreated = (int)(dicemakerCurrent * efficiencyCurrent);
            
            //Roll for Critical
            if (Utility.Roll(criticalChanceCurrent))
            {
                diceCreated = (int)(diceCreated * criticalValueCurrent);
                if (printLog) Debug.Log($"Critical Production: {diceCreated} have been produced");
            }
               
            
            CPU.instance.ChangeResource(Resource.Dice, diceCreated);
            
            //Restart
            foreach (Transform nextLoader in workshopLoadScreen)
            {
                nextLoader.GetComponent<Image>().DOFade(0, 0.2f);
            }
            if (printLog) Debug.Log("|--------------WORKSHOP PRODUCTION FINISHED--------------|");
            yield return new WaitForSeconds(0.2f);
            
            
        }
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        int productionCount = (int)(dicemakerCurrent * efficiencyCurrent);
        
        
        data.areaTitle = "Workshop";
        data.areaDescription = $"In the workshop, dicemaker produce dice." +
                               $"<br>Currently, {productionCount} dice are produced every {timerCurrent} seconds." +
                               $"<br><br>DICEMAKER: Produces {dicemakerCurrent} dice.";
        
        
        string speedText = $"<br><br>??? (Dicemaker to unlock: {dicemakerToUnlockSpeed})";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 1))
        {
            speedText = $"<br><br>SPEED: Decreases production time by {speedCurrent}s.";
        }
        
        data.areaDescription += speedText;
        
        string efficiencyText = $"<br><br>??? (Dicemaker to unlock: {dicemakerToUnlockEfficiency})";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 2))
        {
            efficiencyText = $"<br><br>EFFICIENCY: Increases production by: {efficiencyCurrent * 100}%.";
        }
        
        data.areaDescription += efficiencyText;
        
        string criticalText = $"<br><br>??? (Dicemaker to unlock: {dicemakerToUnlockCritical})";
        float currentCriticalIncrease = 0f;
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop,3) > 0) currentCriticalIncrease = (float)Math.Round(((criticalValueCurrent - 1) * 100), 2);
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 3))
        {
            criticalText = 
                $"<br><br>CRITICAL: {criticalChanceCurrent}% chance per cycle to increase production by {currentCriticalIncrease}%.";
        }
        
        data.areaDescription += criticalText;
        
        string overdriveText = $"<br><br>??? (Dicemaker to unlock: {dicemakerToUnlockOverdrive})";
        float currentOverdriveDecrease = 0f;
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop,4) > 0) currentOverdriveDecrease = (float)Math.Round((1/ overdriveValueCurrent - 1) * 100);
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Workshop, 4))
        {
            overdriveText =
                $"<br><br>OVERDRIVE: {overdriveChanceCurrent}% chance per cycle to decrease production time by {currentOverdriveDecrease*(-1)}%.";
        }
        
        data.areaDescription += overdriveText;
        
       
        
        return data;
    }
   
    #endregion

}
