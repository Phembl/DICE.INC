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
using RNGNeeds;
using UnityEngine.PlayerLoop;

public class Technology : InteractionArea
{
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Technology;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    [SerializeField] private GameObject technologyDisplay;
    [SerializeField] private TMP_Text rollCounter;
    [SerializeField] private DiceTable diceTable;
    
    [TitleGroup("Technology")] 
    [ShowInInspector, ReadOnly] private double rollsCurrent;
    [SerializeField] private double rollsGoalBase;
    [SerializeField] private double rollsGoalMult;
    [ShowInInspector, ReadOnly] private double rollsGoalCurrent;
    [Space] 
    [SerializeField] private int[] unlockLevels; 
    
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
    
    [Header("Weight")]
    [SerializeField] private int weightCostBase;
    [SerializeField] private float weightCostMult;
    [SerializeField] private int weightMax;
    [Space]
    [SerializeField] private float weightMod;
    [ShowInInspector, ReadOnly] private int weightCurrent;
    
    [Header("Explosive")]
    [SerializeField] private int explosiveCostBase;
    [SerializeField] private float explosiveCostMult;
    [SerializeField] private int explosiveMax;
    [Space]
    [ShowInInspector, ReadOnly] private int explosiveCurrent;

    //Display Update
    private Coroutine displayMovement;
    private bool isUpgrading;
    
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
        
        technologyDisplay.transform.DOLocalMoveY(-579, 0);
        
        diceTable.InitializeDiceTable(CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 0), weightMod);
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(sidesCostBase);
        costs.Add(advantageCostBase);
        costs.Add(weightCostBase);
        costs.Add(explosiveCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(sidesCostMult);
        costs.Add(advantageCostMult);
        costs.Add(weightCostMult);
        costs.Add(explosiveCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(sidesMax);
        max.Add(advantageMax);
        max.Add(weightMax);
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
                diceTable.AddSide();
                break;
            
            case 1: //Advantage
                advantageCurrent = count;
                break;
            
            case 2: //Weight
                weightCurrent = count;
                diceTable.AdjustWeight();
                break;
            
            case 3: //Explosive
                explosiveCurrent = count;
                break;
            
        }
        
        CheckProgress();
    }

    //Run by DiceManager when dice are rolled
    public void UpdateRollCounter(int lastRolls)
    {
        if (!areaUnlocked || isUpgrading) return;
        
        rollsCurrent += lastRolls;
        
        //Make sure that rolls do not overflow
        if (rollsCurrent >= rollsGoalCurrent)
        {
            isUpgrading = true;
            rollsCurrent = rollsGoalCurrent;
        }
        
        //Move Display
        rollCounter.text = $"{rollsCurrent:N0}/{rollsGoalCurrent:N0}";
        if (displayMovement != null) StopCoroutine(displayMovement);
        StartCoroutine(DisplayMovement());
        
    }

    IEnumerator DisplayMovement()
    {
        technologyDisplay.transform.DOLocalMoveY(GetDisplayY(), 0.25f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.25f);
        
        if (isUpgrading)
        {
            yield return new WaitForSeconds(0.25f);
            //Reset display
            rollCounter.text = $"Goal reached.";
            yield return new WaitForSeconds(0.25f);
            technologyDisplay.transform.DOLocalMoveY(-579, 2f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(2.2f);
            
            //Add Resources
            CPU.instance.ChangeResource(Resource.mDICE, 1);
            level++;
            CheckProgress();
            
            //New rolls Goal
            rollsGoalCurrent = Math.Round(rollsGoalBase * Math.Pow(rollsGoalMult, level));
            rollsCurrent = 0;
            
            rollCounter.text = $"{rollsCurrent:N0}/{rollsGoalCurrent:N0}";
            
            isUpgrading = false;
        }
    }

    float GetDisplayY()
    {
        float t = Mathf.InverseLerp(0f, (float)rollsGoalCurrent, (float)rollsCurrent);
        float nextDisplayY = Mathf.Lerp(-579f, 0f, t);
        
        return nextDisplayY;
    }
    
    protected override void CheckProgress()
    {
        //Unlock Interactors based on level
        for (int i = 0; i < unlockLevels.Length; i++)
        {
            if (level >= unlockLevels[i] &&
                !CPU.instance.GetInteractorUnlockState(thisInteractionAreaType, i))
            {
                UnlockInteractor(i);
            }
        }
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = data.areaTitle = thisInteractionAreaType.ToString();
        data.areaDescription = $"Technology is used to improve dice performance. To do that, dice rolls are evaluated to generate mega-dice(mDICE) which can be used to increase different dice-aspects." +
                               $"<br><br>Rolls needed to generate the next mDICE: <b>{rollsGoalCurrent - rollsCurrent}</b>." +
                               $"<br><br><b>WARNING: Overflowing rolls can not be taken into consideration as roll data needs to be evaluated first!</b>";

        //Extra Sides TT
        string extrasidesTooltip = "";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 0))
        {
            extrasidesTooltip = $"<br><br><b>SIDES</b>: Adds a side to each dice. Currently all dice have <b>{sidesCurrent + 6}</b> sides.";
        }
        
        //Advantage TT
        string advantageTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 1))
        {
            advantageTooltip = $"<br><br><b>ADVANTAGE:</b> Adds an extra dice to every roll. The highest result will be used. Currently <b>{advantageCurrent}</b> extra dice are rolled.";
        }
        
        //High Roller TT
        string highrollerTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 2))
        {
            
            highrollerTooltip = $"<br><br><b>WEIGHT:</b> Weights each dice towards higher results by <b>{weightMod * 100}%</b>.</b>";
        }
        
        //Explosive TT
        string explosiveTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Technology, 3))
        {
            explosiveTooltip = $"<br><br><b>EXPLOSIVE:</b> Introduces a chance that a dice explodes when rolled, generating <b>2-6</b> extra dice. Current explosion chance per roll: <b>{explosiveCurrent}%</b></b>";
        }
        
        
        data.areaDescription += extrasidesTooltip + advantageTooltip + highrollerTooltip + explosiveTooltip;
        
        
        return data;
    }
   
    #endregion
}
