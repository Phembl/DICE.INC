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
    [SerializeField] private DataCenterCA datacenterCA;

    
    
    
    [TitleGroup("Datacenter")] 
    [SerializeField] private float timeBetweenUpdates;
    [Space]
    
    [Header("Generator")]
    [SerializeField] private int generatorCostBase;
    [SerializeField] private float generatorCostMult;
    [SerializeField] private int generatorMax;
    [Space]
    [ShowInInspector, ReadOnly] private int generatorCurrent;
 
    
    [Header("Affinity")]
    [SerializeField] private int affinityCostBase;
    [SerializeField] private float affinityCostMult;
    [SerializeField] private int affinityMax;
    [Space]
    [ShowInInspector, ReadOnly] private int affinityCurrent;
    
    [Header("Throughput")]
    [SerializeField] private int throughputCostBase;
    [SerializeField] private float throughputCostMult;
    [SerializeField] private int throughputMax;
    [Space]
    [ShowInInspector, ReadOnly] private int throughputCurrent;

    private bool datacenterCycleActive;
    
        
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        UnlockInteractor(0); //Unlock Generators
    }

    protected override void OnAreaUnlock()
    {
        datacenterCA.initializeCA();
    }

    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(generatorCostBase);
        costs.Add(affinityCostBase);
        costs.Add(throughputCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(generatorCostMult);
        costs.Add(affinityCostMult);
        costs.Add(throughputCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(generatorMax);
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
            case 0: // Generators
                generatorCurrent = count;

                if (!datacenterCycleActive) StartCoroutine(DataCenterCycle());
                break;
            
            case 1: // Affinity
                affinityCurrent = count;
                break;
            
            case 2:// Throughput
                throughputCurrent = count;
                break;
            
        }
    }

    

       
    
    protected override void CheckProgress()
    {
        
        
        
    }

    private IEnumerator DataCenterCycle()
    {
        datacenterCycleActive = true;


        yield return new WaitForEndOfFrame();
    }
    
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = thisInteractionAreaType.ToString();
        data.areaDescription = "The data center,";
                             

        //Generator
        string value1Tooltip = $"<br><br><b>GENERATOR:</b> Increases the number of particles shot per shoot, increasing the number of traveling particles.";
        
        
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
