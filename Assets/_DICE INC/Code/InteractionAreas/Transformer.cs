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


public class Transformer : InteractionArea
{
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Transformer;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    
    [TitleGroup("Transformer")] 
    [SerializeField] private int diceNeededBase = 100;
    [SerializeField] private int materialProducedBase = 1;
    [ShowInInspector, ReadOnly] private int diceNeededCurrent;
    public int GetDiceNeeded() => diceNeededCurrent;
    [ShowInInspector, ReadOnly] private int materialProducedCurrent;
    [Space] 
    [SerializeField] private int[] unlockLevels; 
    
    
    [Header("Condenser")]
    [ShowInInspector, ReadOnly] private int condenserCurrent;
    [Space]
    [SerializeField] private int condenserCostBase;
    [SerializeField] private float condenserCostMult;
    [SerializeField] private int condenserMax;
    
    [Header("Extruder")]
    [ShowInInspector, ReadOnly] private int extruderCurrent;
    [Space]
    [SerializeField] private int extruderCostBase;
    [SerializeField] private float extruderCostMult;
    [SerializeField] private int extruderMax;
    
        
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        diceNeededCurrent = diceNeededBase;
        materialProducedCurrent = materialProducedBase;
    }

    protected override void OnAreaUnlock()
    {
       
    }

    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(condenserCostBase);
        costs.Add(extruderCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(condenserCostMult);
        costs.Add(extruderCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(condenserMax);
        max.Add(extruderMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Transformer, index);
        
        switch (index)
        {
            case 0: 
                condenserCurrent = count;
                diceNeededCurrent = diceNeededBase - condenserCurrent;
                break;
            
            case 1: 
                extruderCurrent = count;
                break;
            
        }
    }
    
    protected override void CheckProgress()
    {
        //Unlock Interactors based on level
        //Level increases on transformed dice
        for (int i = 0; i < unlockLevels.Length; i++)
        {
            if (level >= unlockLevels[i] &&
                !CPU.instance.GetInteractorUnlockState(thisInteractionAreaType, i))
            {
                UnlockInteractor(i);
            }
        }
        
    }
    

    public void TriggerTransformer(int _material)
    {
        
        int materialsProduced = _material * (extruderCurrent + 1);
        CPU.instance.ChangeResource(Resource.Material, materialsProduced);
        
        if (printLog) Debug.Log($"Transformer: Received {_material} material to produce. With {extruderCurrent} extruders, {materialsProduced} materials produced.");
        
        level++;
        CheckProgress();
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = data.areaTitle = thisInteractionAreaType.ToString();
        data.areaDescription =
            $"The transformer generates <b>{1 * (extruderCurrent + 1)}</b> material for every <b>{diceNeededCurrent}</b> dice produced in the factory.";
                             

        //Condenser
        string condenserTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Transformer, 0))
        {
            condenserTooltip = $"<br><br><b>CONDENSER:</b> Every condenser reduces the amount of dice needed to generate material by <b>1</b>.";
        }
        
        //Extruder
        string extruderTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Transformer, 1))
        {
            extruderTooltip = $"<br><br><b>EXTRUDER:</b> Every Extruder increases the amount of generated materials by <b>1</b>.";
        }
        
        data.areaDescription += condenserTooltip + extruderTooltip;
        
        return data;
    }
    #endregion

}
