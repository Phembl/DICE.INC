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
    [SerializeField] private int[] unlockLevels; 
    
    [Header("Generator")]
    [SerializeField] private int generatorCostBase;
    [SerializeField] private float generatorCostMult;
    [SerializeField] private int generatorMax;
    [SerializeField] private int generatorTickDecrease;
    [Space]
    [ShowInInspector, ReadOnly] private int generatorCurrent;
    
    [Header("Crystal")]
    [SerializeField] private int crystalCostBase;
    [SerializeField] private float crystalCostMult;
    [SerializeField] private int crystalMax;
    [Space]
    [ShowInInspector, ReadOnly] private int crystalCurrent;
    
    [Header("Throughput")]
    [SerializeField] private int throughputCostBase;
    [SerializeField] private float throughputCostMult;
    [SerializeField] private int throughputMax;
    [SerializeField] private int throughputTickIncrease;
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
        
    }

    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(generatorCostBase);
        costs.Add(crystalCostBase);
        costs.Add(throughputCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(generatorCostMult);
        costs.Add(crystalCostMult);
        costs.Add(throughputCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(generatorMax);
        max.Add(crystalMax);
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
                datacenterCA.ChangeSpawnTicks(-generatorTickDecrease);
                if (!datacenterCycleActive)
                {
                    datacenterCycleActive = true;
                    datacenterCA.initializeCA();
                }
                break;
            
            case 1: // Crystal
                crystalCurrent = count;
                datacenterCA.ChangeCrystalSpawnChance((float)crystalCurrent / 100);
                break;
            
            case 2:// Throughput
                throughputCurrent = count;
                datacenterCA.ChangeWipeTicks(throughputTickIncrease);
                break;
            
        }
    }
    

    //Send by CA when data collides
    public void IncreaseLevel()
    {
        level++;
        CheckProgress();
    }
    
    protected override void CheckProgress()
    {
      
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
        
        data.areaTitle = thisInteractionAreaType.ToString();
        data.areaDescription = "The data center, generates data particles which generate data on collision." +
                               $"<br>One data particle is generated every <b>{(float)datacenterCA.GetSpawnTicks()/50}</b> seconds." +
                               $"<br>The data pool is wiped every <b>{(float)datacenterCA.GetWipeTicks()/50}</b> seconds.";
                             

        //Generator
        string value1Tooltip = $"<br><br><b>GENERATOR:</b> Every generator decreases the time between data point generation by <b>{(float)generatorTickDecrease/50}</b> seconds.";
        
        
        //Value 2 TT
        string value2Tooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Datacenter, 1))
        {

            value2Tooltip = $"<br><br><b>CRYSTAL:</b> Introduces a chance for data to crystallize after every wipe. Every level increases the crystallization chance by <b>1</b>%." +
                            $"<br>Current crystallization chance: <b>{crystalCurrent}</b>%.";
        }
        
        //Value 3 TT
        string value3Tooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Datacenter, 2))
        {

            value3Tooltip = $"<br><br><b>THROUGHPUT:</b> Every throughput Increase the time between wipes by <b>{(float)throughputTickIncrease/50}</b> seconds.";
        }
        
        data.areaDescription += value1Tooltip + value2Tooltip +  value3Tooltip;
        
        return data;
    }
    #endregion

}
