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


public class Datacenter : InteractionArea
{
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Datacenter;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    
    
    [TitleGroup("Transformer")] 
    [SerializeField] private float timeBetweenUpdates;
    [Space]
    
    [Header("VALUE1")]
    [SerializeField] private int value1CostBase;
    [SerializeField] private float value1CostMult;
    [SerializeField] private int value1Max;
 
    
    [Header("VALUE2")]
    [SerializeField] private int value2CostBase;
    [SerializeField] private float value2CostMult;
    [SerializeField] private int value2Max;
    
    [Header("VALUE3")]
    [SerializeField] private int value3CostBase;
    [SerializeField] private float value3CostMult;
    [SerializeField] private int value3Max;

    
        
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
        
        costs.Add(value1CostBase);
        costs.Add(value2CostBase);
        costs.Add(value3CostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(value1CostMult);
        costs.Add(value2CostMult);
        costs.Add(value3CostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(value1Max);
        max.Add(value2Max);
        max.Add(value3Max);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Datacenter, index);
        
        switch (index)
        {
            case 0: 
                break;
            
            case 1: 
                break;
            
        }
    }

    

       
    
    protected override void CheckProgress()
    {
        
        
        
    }
    
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = "Data center";
        data.areaDescription = "The data center,";
                             

        //Value 1TT
        string value1Tooltip = $"<br><br><b>VALUE1:</b>";
        
        
        //Value 2 TT
        string value2Tooltip = $"<br><br>???</b>)";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Datacenter, 1))
        {

            value2Tooltip = $"<br><br><b>Value 2:</b> ";
        }
        
        //Value 3 TT
        string value3Tooltip = $"<br><br>???</b>)";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Datacenter, 2))
        {

            value3Tooltip = $"<br><br><b>Value 3:</b> ";
        }
        
        data.areaDescription += value1Tooltip + value2Tooltip +  value3Tooltip;
        
        return data;
    }
    #endregion

}
