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
    
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Diceworld;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;

    [SerializeField] private GameObject diceworldDisplay;
        
    [TitleGroup("Settings")] 
    [SerializeField] private TMP_Text rollCounter;
    [ShowInInspector, ReadOnly] private double currentRollCount;
    [SerializeField] private double targetCountBase;
    [SerializeField] private double targetCountMult;
    [ShowInInspector, ReadOnly] private double currentRollTarget;
    
    [Header("Sides")]
    [SerializeField] private int costSidesBase;
    [SerializeField] private float costSidesMultiplier;
    [ShowInInspector, ReadOnly] private int currentSides;
    
    [Header("Advantage")]
    [SerializeField] private int costAdvantageBase;
    [SerializeField] private float costAdvantageMultiplier;
    
    [Header("High Roller")]
    [SerializeField] private int costHighRollerBase;
    [SerializeField] private float costHighRollerMultiplier;
    
    [Header("Explosive")]
    [SerializeField] private int costExplosiveBase;
    [SerializeField] private float costExplosiveMultiplier;
    
    [Header("Progress")] 
    [SerializeField] private int levelToUnlockAdvantage;
    [SerializeField] private int levelToUnlockHighRoller;
    [SerializeField] private int levelToUnlockExplosive;


    private int diceworldLevel = 1;

    private int startDiceRolls;
    
    public static Diceworld instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    
  #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
      
        currentRollTarget = targetCountBase;

        rollCounter.text = $"{currentRollCount:N0}/{currentRollTarget:N0}";
        
        startDiceRolls = CPU.instance.GetDiceRolledTotal();

        diceworldDisplay.transform.DOLocalMoveY(-470, 0);
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(costSidesBase);
        costs.Add(costAdvantageBase);
        costs.Add(costHighRollerBase);
        costs.Add(costExplosiveBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(costSidesMultiplier);
        costs.Add(costAdvantageMultiplier);
        costs.Add(costHighRollerMultiplier);
        costs.Add(costExplosiveMultiplier);
        
        return costs;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, index);
        
        switch (index)
        {
            case 0: //Sides
                CheckProgress();
                break;
            
            case 1: //Advantage
               
                break;
            
            case 2: //HighRoller
                
                break;
            
            case 3: //Explosive
                
                break;
            
           
            
        }
    }

    //Run by DiceManager when dice are rolled
    public void UpdateRollCounter(int lastRolls)
    {
        if (!areaUnlocked) return;
        
        currentRollCount += lastRolls;
        
        if (currentRollCount >= currentRollTarget)
        {
            //Account for overflow
            currentRollCount -= currentRollTarget;
            diceworldLevel++;
            
            currentRollTarget = Math.Round(targetCountBase * Math.Pow(targetCountMult, diceworldLevel));

            CPU.instance.ChangeResource(Resource.mDice, 1);
            
            CheckProgress();
        }
      
        if (currentRollCount > 0) diceworldDisplay.transform.DOLocalMoveY(GetDisplayY(), 0.2f);
        else diceworldDisplay.transform.DOLocalMoveY(-470, 0.5f);
        
        rollCounter.text = $"{currentRollCount:N0}/{currentRollTarget:N0}";

    }

    float GetDisplayY()
    {
        float t = Mathf.InverseLerp(0f, (float)currentRollTarget, (float)currentRollCount);
        float nextDisplayY = Mathf.Lerp(-470f, 0f, t);
        
        return nextDisplayY;
    }
    
    void CheckProgress()
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
