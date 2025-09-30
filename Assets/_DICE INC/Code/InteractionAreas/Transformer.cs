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
    [SerializeField] private float timeBetweenUpdates;
    [Space]
    
    [Header("Condenser")]
    [SerializeField] private int condenserCostBase;
    [SerializeField] private float condenserCostMult;
    [SerializeField] private int condenserMax;
 
    
    [Header("Extruder")]
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
        
        data.areaTitle = "Transformer";
        data.areaDescription = "The transformer,";
                             

        //Condenser
        string condenserTooltip = $"<br><br><b>CONDENSER:</b>";
        
        
        //Extruder
        string extruderTooltip = $"<br><br>???</b>)";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Transformer, 1))
        {

            extruderTooltip = $"<br><br><b>EXTRUDER:</b> ";
        }
        
        data.areaDescription += condenserTooltip + extruderTooltip;
        
        return data;
    }
    #endregion

}
