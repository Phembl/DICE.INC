using System;
using System.Collections;
using System.Collections.Generic;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine.PlayerLoop;

public class Diceworld : InteractionArea
{
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Diceworld;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    [SerializeField] private GameObject diceworldDisplay;
    [SerializeField] private TMP_Text rollCounter;
    
    [TitleGroup("Diceworld")] 
    [ShowInInspector, ReadOnly] private int diceworldLevel = 1;
    [ShowInInspector, ReadOnly] private double rollsCurrent;
    [SerializeField] private double rollsGoalBase;
    [SerializeField] private double rollsGoalMult;
    [ShowInInspector, ReadOnly] private double rollsGoalCurrent;
    
    [Header("Sides")]
    [SerializeField] private int sidesCostBase;
    [SerializeField] private float sidesCostMult;
    [SerializeField] private int sidesMax;
    [Space]
    [ShowInInspector, ReadOnly] private int sidesCurrent;
    
    [Header("Advantage")]
    [SerializeField] private int advantageCostBase;
    [SerializeField] private float advantageCostMult;
    [SerializeField] private int advantageMax;
    [Space]
    [ShowInInspector, ReadOnly] private int advantageCurrent;
    
    [Header("High Roller")]
    [SerializeField] private int highrollerCostBase;
    [SerializeField] private float highrollerCostMult;
    [SerializeField] private int highrollerMax;
    [Space]
    [ShowInInspector, ReadOnly] private int highrollerCurrent;
    
    [Header("Explosive")]
    [SerializeField] private int explosiveCostBase;
    [SerializeField] private float explosiveCostMult;
    [SerializeField] private int explosiveMax;
    [Space]
    [ShowInInspector, ReadOnly] private int explosiveCurrent;
    
    [Header("Progress")] 
    [SerializeField] private int levelToUnlockAdvantage;
    [SerializeField] private int levelToUnlockHighRoller;
    [SerializeField] private int levelToUnlockExplosive;
    
    
    public static Diceworld instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    
  #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
      
        rollsGoalCurrent = rollsGoalBase;
        rollCounter.text = $"{rollsCurrent:N0}/{rollsGoalCurrent:N0}";
        
        diceworldDisplay.transform.DOLocalMoveY(-470, 0);
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(sidesCostBase);
        costs.Add(advantageCostBase);
        costs.Add(highrollerCostBase);
        costs.Add(explosiveCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(sidesCostMult);
        costs.Add(advantageCostMult);
        costs.Add(highrollerCostMult);
        costs.Add(explosiveCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(sidesMax);
        max.Add(advantageMax);
        max.Add(highrollerMax);
        max.Add(explosiveMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, index);
        
        switch (index)
        {
            case 0: //Sides
                sidesCurrent = count;
                CheckProgress();
                break;
            
            case 1: //Advantage
                advantageCurrent = count;
                break;
            
            case 2: //HighRoller
                highrollerCurrent = count;
                break;
            
            case 3: //Explosive
                explosiveCurrent = count;
                break;
            
        }
    }

    //Run by DiceManager when dice are rolled
    public void UpdateRollCounter(int lastRolls)
    {
        if (!areaUnlocked) return;
        
        rollsCurrent += lastRolls;
        
        if (rollsCurrent >= rollsGoalCurrent)
        {
            //Account for overflow
            rollsCurrent -= rollsGoalCurrent;
            diceworldLevel++;
            
            rollsGoalCurrent = Math.Round(rollsGoalBase * Math.Pow(rollsGoalMult, diceworldLevel));

            CPU.instance.ChangeResource(Resource.mDice, 1);
            
            CheckProgress();
        }
      
        if (rollsCurrent > 0) diceworldDisplay.transform.DOLocalMoveY(GetDisplayY(), 0.2f);
        else diceworldDisplay.transform.DOLocalMoveY(-470, 0.5f);
        
        rollCounter.text = $"{rollsCurrent:N0}/{rollsGoalCurrent:N0}";

    }

    float GetDisplayY()
    {
        float t = Mathf.InverseLerp(0f, (float)rollsGoalCurrent, (float)rollsCurrent);
        float nextDisplayY = Mathf.Lerp(-470f, 0f, t);
        
        return nextDisplayY;
    }
    
    protected override void CheckProgress()
    {
        if (diceworldLevel >= levelToUnlockAdvantage && 
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Diceworld, 1)) 
            UnlockInteractor(1);
        
        if (diceworldLevel >= levelToUnlockHighRoller && 
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Diceworld, 2)) 
            UnlockInteractor(2);
        
        if (diceworldLevel >= levelToUnlockExplosive && 
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Diceworld, 3)) 
            UnlockInteractor(3);
    }
    
    
    
    #endregion
}
