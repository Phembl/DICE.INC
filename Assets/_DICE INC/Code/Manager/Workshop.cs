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


public class Workshop : InteractionArea
{
    
    
    [SerializeField] private Transform workshopLoadScreen;
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Workshop;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;

    [TitleGroup("Settings")] 
    [SerializeField] private float workshopTimer;
    [ShowInInspector, ReadOnly] private float currentTimer;
    
    [Header("Dicemaker")]
    [SerializeField] private int costDicemakerBase;
    [SerializeField] private float costDicemakerMultiplier;
    
    [Header("Speed")]
    [SerializeField] private int costSpeedBase;
    [SerializeField] private float costSpeedMultiplier;
    [SerializeField] private float speedIncrease;
    
    [Header("Efficiency")]
    [SerializeField] private int costEfficiencyBase;
    [SerializeField] private float costEfficiencyMultiplier;
    [Space]
    [SerializeField] private int efficiencyIncrease;
    [ShowInInspector, ReadOnly] private int currentEfficiency = 1;
    
    [Header("Critical")]
    [SerializeField] private int costCriticalBase;
    [SerializeField] private float costCriticalMultiplier;
    [Space]
    [SerializeField] private float criticalChanceBase;
    [SerializeField] private float criticalChanceMult;
    [SerializeField] private float criticalValueBase;
    [SerializeField] private float criticalValueMult;
    [ShowInInspector, ReadOnly] private float currentCriticalChance;
    [ShowInInspector, ReadOnly] private float currentCriticalValue;
    
    [Header("Overdrive")]
    [SerializeField] private int costOverdriveBase;
    [SerializeField] private float costOverdriveMultiplier;
    [Space]
    [SerializeField] private float overdriveChanceBase;
    [SerializeField] private float overdriveChanceMult;
    [SerializeField] private float overdriveValueBase;
    [SerializeField] private float overdriveValueMult;
    [ShowInInspector, ReadOnly] private float currentOverdriveChance;
    [ShowInInspector, ReadOnly] private float currentOverdriveValue;
    
    [Header("Progress")] 
    [SerializeField] private int dicemakerToUnlockSpeed;
    [SerializeField] private int dicemakerToUnlockEfficiency;
    [SerializeField] private int dicemakerToUnlockCritical;
    [SerializeField] private int dicemakerToUnlockOverdrive;

   
    
    private bool workshopCycleActive;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        currentTimer = workshopTimer;
        
        foreach (Transform nextLoader in workshopLoadScreen)
        {
            nextLoader.GetComponent<Image>().DOFade(0, 0);
        }
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(costDicemakerBase);
        costs.Add(costSpeedBase);
        costs.Add(costEfficiencyBase);
        costs.Add(costCriticalBase);
        costs.Add(costOverdriveBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(costDicemakerMultiplier);
        costs.Add(costSpeedMultiplier);
        costs.Add(costEfficiencyMultiplier);
        costs.Add(costCriticalMultiplier);
        costs.Add(costOverdriveMultiplier);
        
        return costs;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, index);
        
        switch (index)
        {
            case 0: //Dicemaker
                CheckProgress();
                break;
            
            case 1: //Speed
                currentTimer = workshopTimer * math.pow((1 - speedIncrease), count);
                break;
            
            case 2: //Efficiency
                currentEfficiency = (efficiencyIncrease * (count + 1));
                break;
            
            case 3: //Critical
                currentCriticalChance = (float)(criticalChanceBase * Math.Pow(criticalChanceMult, count));
                currentCriticalValue = (float)(criticalValueBase * Math.Pow(criticalValueMult, count));
                Debug.Log("Current critical value: " + currentCriticalValue);
                break;
            
            case 4: //Overdrive
                currentOverdriveChance = (float)(overdriveChanceBase * Math.Pow(overdriveChanceMult, count));
                currentOverdriveValue = (float)(overdriveValueBase * Math.Pow(overdriveValueMult, count));
                Debug.Log("Current overdrive value: " + currentOverdriveValue);
                break;
            
        }
    }
    
    void CheckProgress()
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
            float nextProductionTimer = currentTimer;
            
            //Roll for Overdrive if overdrive has been bought
            if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, 4) > 0)
            {
                
                float overdriveRoll = Random.Range(0, 100);
                if (currentOverdriveChance >= overdriveRoll)
                {
                    nextProductionTimer /= currentOverdriveValue;
                    Debug.Log($"Overdrive Production: current production cycle time is {nextProductionTimer}");
                }
            }
            
            for (int i = 0; i < 10; i++)
            {
                float singleLoadTimer = nextProductionTimer / 10;
                
                workshopLoadScreen.GetChild(i).gameObject.GetComponent<Image>().DOFade(1, singleLoadTimer);
                yield return new WaitForSeconds(singleLoadTimer);
            }
            
            int diceCreated = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, 0) * currentEfficiency;
            
            //Roll for Critical if critical has been bought
            if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, 3) > 0)
            {
                float critRoll = Random.Range(0, 100);
                if (currentCriticalChance >= critRoll)
                {
                    diceCreated = (int)(diceCreated * currentCriticalValue);
                    Debug.Log($"Critical Production: {diceCreated} have been produced");
                }
            }
            
            CPU.instance.ChangeResource(Resource.Dice, diceCreated);
            
            //Restart
            foreach (Transform nextLoader in workshopLoadScreen)
            {
                nextLoader.GetComponent<Image>().DOFade(0, 0.2f);
            }
            
            yield return new WaitForSeconds(0.2f);
        }
        
    }
    
    #endregion

}
