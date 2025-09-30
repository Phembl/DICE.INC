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
    
    
    [TitleGroup("Datacenter")] 
    [SerializeField] private float timeBetweenUpdates;
    [Space]
    
    [Header("Particle cannon")]
    [SerializeField] private int particleCannonCostBase;
    [SerializeField] private float particleCannonCostMult;
    [SerializeField] private int particleCannonMax;
 
    
    [Header("Affinity")]
    [SerializeField] private int affinityCostBase;
    [SerializeField] private float affinityCostMult;
    [SerializeField] private int affinityMax;
    
    [Header("Throughput")]
    [SerializeField] private int throughputCostBase;
    [SerializeField] private float throughputCostMult;
    [SerializeField] private int throughputMax;

    
        
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
        
        costs.Add(particleCannonCostBase);
        costs.Add(affinityCostBase);
        costs.Add(throughputCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(particleCannonCostMult);
        costs.Add(affinityCostMult);
        costs.Add(throughputCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(particleCannonMax);
        max.Add(affinityMax);
        max.Add(throughputMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Datacenter, index);
        
        switch (index)
        {
            case 0: // Particle Cannon
                break;
            
            case 1: // Affinity
                break;
            
            case 2:// Throughput
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
        string value1Tooltip = $"<br><br><b>PARTICLE CANNON:</b> Increases the number of particles shot per shoot, increasing the number of traveling particles.";
        
        
        //Value 2 TT
        string value2Tooltip = $"<br><br>???</b>)";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Datacenter, 1))
        {

            value2Tooltip = $"<br><br><b>AFFINITY:</b> Introduces a chance for a particle to increase its traveling speed after a collision.";
        }
        
        //Value 3 TT
        string value3Tooltip = $"<br><br>???</b>)";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Datacenter, 2))
        {

            value3Tooltip = $"<br><br><b>THROUGHPUT:</b> Increase the number of shots before the area is cleared, increasing the collision chance and frequency.";
        }
        
        data.areaDescription += value1Tooltip + value2Tooltip +  value3Tooltip;
        
        return data;
    }
    #endregion

}
