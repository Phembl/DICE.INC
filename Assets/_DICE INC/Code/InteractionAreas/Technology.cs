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

public class Technology : InteractionArea
{
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Technology;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    [SerializeField] private GameObject diceworldDisplay;
    [SerializeField] private TMP_Text rollCounter;
    
    [TitleGroup("Technology")] 
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
    
    
    public static Technology instance;
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
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, index);
        
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
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 1)) 
            UnlockInteractor(1);
        
        if (diceworldLevel >= levelToUnlockHighRoller && 
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 2)) 
            UnlockInteractor(2);
        
        if (diceworldLevel >= levelToUnlockExplosive && 
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 3)) 
            UnlockInteractor(3);
    }
    
    
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = "Technology";
        data.areaDescription = "In the diceworld, the";

        //Extra Sides TT
        string extrasidesTooltip = "<br><br><b>EXTRA SIDES:</b>";
        
        //Advantage TT
        string advantageTooltip = $"<br><br>??? (Level to unlock: {levelToUnlockAdvantage})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 1))
        {
            
            advantageTooltip = $"<br><br><b>ADVANTAGE:</b> Each point.</b>";
        }
        
        //High Roller TT
        string highrollerTooltip = $"<br><br>??? (Level to unlock: {levelToUnlockHighRoller})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 2))
        {
            
            highrollerTooltip = $"<br><br><b>HIGH ROLLER:</b> Each point.</b>";
        }
        
        //Explosive TT
        string explosiveTooltip = $"<br><br>??? (Level to unlock: {levelToUnlockExplosive})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 3))
        {
            
            explosiveTooltip = $"<br><br><b>EXPLOSIVE:</b> Each point.</b>";
        }
        
        
        data.areaDescription += extrasidesTooltip + advantageTooltip + highrollerTooltip + explosiveTooltip;
        
        
        return data;
    }
   
    #endregion
}
