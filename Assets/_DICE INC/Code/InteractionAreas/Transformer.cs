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
    [SerializeField] private TMP_Text counterTMP;
    
    [TitleGroup("Transformer")] 
    [ShowInInspector, ReadOnly] private int destroyedDice;
    [ShowInInspector, ReadOnly] private int currentFragments;
    [ShowInInspector, ReadOnly] private int fragmentsMin = 1;
    [ShowInInspector, ReadOnly] private int fragmentsMax = 10;
    [SerializeField] private int fragmentsNeededBase = 100;
    [ShowInInspector, ReadOnly] private int fragmentsNeededCurrent = 100;
    [ShowInInspector, ReadOnly] private int materialProduced = 1;
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
                fragmentsNeededCurrent = fragmentsNeededBase - condenserCurrent;
                break;
            
            case 1: 
                extruderCurrent = count;
                materialProduced = 1 + count;
                break;
            
        }
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

    public void TriggerTransformer(int dice)
    {
        if (printLog) Debug.Log($"Transformer: Received {dice} dice to transform.");
        
        destroyedDice += dice;

        //For each Dice, roll fragments
        for (int i = 0; i < dice; i++)
        {
            int fragmentsProduced = Random.Range(fragmentsMin, fragmentsMax);
            currentFragments += fragmentsProduced;
        }
        
        if (printLog) Debug.Log($"Transformer: Current fragments are {currentFragments}.");

        //Check for enough fragments
        if (currentFragments >= fragmentsNeededCurrent)
        {
            if (printLog) Debug.Log($"Transformer: Fragments reached {fragmentsNeededCurrent}. Now producing {materialProduced} material.");
            
            //Produce Material
            currentFragments -= fragmentsNeededCurrent;
            if (currentFragments < 0) currentFragments = 0;
            
            if (printLog) Debug.Log($"Transformer: New current fragments are {currentFragments}.");
            
            CPU.instance.ChangeResource(Resource.Material, materialProduced);
            
            level++;
            CheckProgress();
        }
        
        counterTMP.text = $"{currentFragments}/<br>{fragmentsNeededCurrent}";
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = data.areaTitle = thisInteractionAreaType.ToString();
        data.areaDescription = $"The transformer destroys every tenth dice produced in the factory to generate between <b>{fragmentsMin}</b> and <b>{fragmentsMax}</b> fragments." +
                               $"<br>From <b>{fragmentsNeededCurrent}</b> fragments <b>{materialProduced}</b> material is generated. ";
                             

        //Condenser
        string condenserTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Transformer, 0))
        {
            condenserTooltip = $"<br><br><b>CONDENSER:</b> Every condenser reduced the amount of fragments needed to generate material by <b>1</b>.";
        }
        
        //Extruder
        string extruderTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Transformer, 1))
        {
            extruderTooltip = $"<br><br><b>EXTRUDER:</b> Every Extruder increases the amount of generated materials by <b>100</b>%.";
        }
        
        data.areaDescription += condenserTooltip + extruderTooltip;
        
        return data;
    }
    #endregion

}
