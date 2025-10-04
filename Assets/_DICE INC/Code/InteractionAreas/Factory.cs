using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class Factory : InteractionArea
{
    #region |-------------- SETTINGS --------------|
    
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Factory;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    [SerializeField] private Transform factoryLoader;
    [SerializeField] private Transformer transformer;

    [TitleGroup("Factory")] 
    [SerializeField] private float timerBase;
    [SerializeField] private float efficiencyBase;
    [ShowInInspector, ReadOnly] private float timerCurrent;
    [ShowInInspector, ReadOnly] private int productionCurrent => (int)((workerCurrent + (AIWorkerCurrent * AIWorkerDiceProduction)) * efficiencyCurrent);
    [ShowInInspector, ReadOnly] private float efficiencyCurrent;
    [ShowInInspector, ReadOnly] private float productionSpeedCurrent;
    
    [Space] 
    [SerializeField] private int[] unlockLevels; 
    
    [Header("Worker")]
    [SerializeField] private int workerCostBase;
    [SerializeField] private float workerCostMult;
    [SerializeField] private int workerMax;
    [Space]
    [ShowInInspector, ReadOnly] private int workerCurrent;

    
    [Header("Conveyor")]
    [SerializeField] private int conveyorCostBase;
    [SerializeField] private float conveyorCostMult;
    [SerializeField] private int conveyorMax;
    [SerializeField] private float conveyorSpeedIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private int conveyorCurrent;

    
    [Header("Tools")]
    [SerializeField] private int toolsCostBase;
    [SerializeField] private float toolsCostMult;
    [SerializeField] private int toolsMax;
    [SerializeField] private float toolsEfficiencyIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private int toolsCurrent;
    
    

    
    [Header("Surplus")]
    [SerializeField] private int surplusCostBase;
    [SerializeField] private float surplusCostMult;
    [SerializeField] private int surplusMax;
    [Space]
    [SerializeField] private float surplusChanceBase;
    [SerializeField] private float surplusValueBase;
    [SerializeField] private float surplusValueIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private int surplusCurrent;
    [ShowInInspector, ReadOnly] private float surplusChanceCurrent;
    [ShowInInspector, ReadOnly] private float surplusValueCurrent;
    
    [Header("Overdrive")]
    [SerializeField] private int overdriveCostBase;
    [SerializeField] private float overdriveCostMultiplier;
    [SerializeField] private int overdriveMax;
    [Space]
    [SerializeField] private float overdriveChanceBase;
    [SerializeField] private float overdriveChanceIncrease;
    [SerializeField] private float overdriveValueBase;
    [Space]
    [ShowInInspector, ReadOnly] private int overdriveCurrent;
    [ShowInInspector, ReadOnly] private float overdriveChanceCurrent;
    [ShowInInspector, ReadOnly] private float overdriveValueCurrent;
    
    [Header("AI Worker")]
    [SerializeField] private int AIWorkerCostBase;
    [SerializeField] private float AIWorkerCostMultiplier;
    [SerializeField] private int AIWorkerMax;
    [SerializeField] private int AIWorkerDiceProduction;
    [Space]
    [ShowInInspector, ReadOnly] private int AIWorkerCurrent;
    
    [Header("Self Learning")]
    [SerializeField] private int selfLearningCostBase;
    [SerializeField] private float selfLearningCostMultiplier;
    [SerializeField] private int selfLearningMax;
    [SerializeField] private float selfLearningEfficiencyIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private int selfLearningCurrent;

    private int diceUntilTransformer;
    
    #endregion
    
    private bool workshopCycleActive;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        timerCurrent = timerBase;
        efficiencyCurrent = efficiencyBase;
        
        foreach (Transform nextLoader in factoryLoader)
        {
            nextLoader.GetComponent<Image>().DOFade(0, 0);
        }
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(workerCostBase);
        costs.Add(conveyorCostBase);
        costs.Add(toolsCostBase);
        costs.Add(surplusCostBase);
        costs.Add(overdriveCostBase);
        costs.Add(AIWorkerCostBase);
        costs.Add(selfLearningCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(workerCostMult);
        costs.Add(conveyorCostMult);
        costs.Add(toolsCostMult);
        costs.Add(surplusCostMult);
        costs.Add(overdriveCostMultiplier);
        costs.Add(AIWorkerCostMultiplier);
        costs.Add(selfLearningCostMultiplier);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(workerMax);
        max.Add(conveyorMax);
        max.Add(toolsMax);
        max.Add(surplusMax);
        max.Add(overdriveMax);
        max.Add(AIWorkerMax);
        max.Add(selfLearningMax);
        
        return max;
    }
    
   
    #endregion
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, index);
        
        switch (index)
        {
            case 0: //Workers
                workerCurrent = count;
                if (!workshopCycleActive) StartCoroutine(WorkShopCycle());
                
                //If AI Workers are unlocked and only one worker is currently there, update AI Worker availability
                if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 5) &&
                    count == 1)
                {
                    GetInteractor(5).CheckAvailability();
                }
                
                GetInteractor(0).UpdateCount(CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,0));
                
                if (printLog) Debug.Log($"Factory: Worker added: Current workers:{workerCurrent}");
                break;
            
            case 1: //Conveyor
                conveyorCurrent = count;
                productionSpeedCurrent = conveyorSpeedIncrease * count;
                timerCurrent = timerBase - productionSpeedCurrent;
                timerCurrent = (float)Math.Round(timerCurrent, 2);
                if (printLog) Debug.Log($"Factory: Conveyor added. Current production speed:{productionSpeedCurrent}. Current production time:{timerCurrent}");
                break;
            
            case 2: //Tools
                toolsCurrent = count;
                efficiencyCurrent = efficiencyBase + (toolsEfficiencyIncrease * count);
                if (printLog) Debug.Log($"Factory: Tools added: Current efficiency value:{efficiencyCurrent}");
                break;
            
            case 3: //Surplus
                surplusCurrent = count;
                surplusValueCurrent = surplusValueBase + (surplusValueIncrease * count);
                if (printLog) Debug.Log($"Factory: Surplus upgraded: Value:{surplusValueCurrent}, Chance:{surplusChanceCurrent}%");
                break;
            
            case 4: //Overdrive
                overdriveCurrent = count;
                overdriveChanceCurrent = overdriveChanceBase + (overdriveChanceIncrease * count);
                if (printLog) Debug.Log($"Factory: Overdrive upgraded: Value:{overdriveValueCurrent}, Chance:{overdriveChanceCurrent}%");
                break;
            
            case 5: // AI Worker
                AIWorkerCurrent = count;
                CPU.instance.IncreaseAreaInteractorCount(InteractionAreaType.Factory,0,-1);
                workerCurrent--;
                
                GetInteractor(5).CheckAvailability();
                //Update the Counter of the worker Interactor
                GetInteractor(0).UpdateCount(CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,0));
                if (printLog) Debug.Log($"Factory: AI Worker added: Current AI workers:{AIWorkerCurrent}");
                break;
            
            case 6: // Self Learning
                selfLearningCurrent = count;
                
                
                if (printLog) Debug.Log($"Factory: Self learning updated: Current self learning:{selfLearningCurrent}");
                break;
            
        }

        CheckProgress();
        
    }
    
    protected override void CheckProgress()
    {
        level =
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 0) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 1) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 2) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 3) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 4) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 5) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 6);

     
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
    
    
    private IEnumerator WorkShopCycle()
    {
        workshopCycleActive = true;
        while (workshopCycleActive)
        {
            float nextProductionTimer = timerCurrent;
            
            if (printLog) Debug.Log("|--------------FACTORY PRODUCTION --------------|");
            if (printLog) Debug.Log($"Production started. Worker will produce: <b>{workerCurrent}</b> dice, AI Worker will produce: <b>{AIWorkerCurrent * AIWorkerDiceProduction}</b> dice.");
            if (printLog) Debug.Log($"Efficiency is: <b>{efficiencyCurrent * 100}</b>%, baseline production will be: <b>{productionCurrent}</b> dice.");
            if (printLog) Debug.Log($"Surplus Chance: <b>{surplusChanceCurrent * 100}</b>%");
            if (printLog) Debug.Log($"Overdrive Chance: <b>{overdriveChanceCurrent * 100}</b>%");

            
            //Roll for Overdrive
            if (Utility.Roll(overdriveChanceCurrent))
            {
                nextProductionTimer /= overdriveValueCurrent;
                if (printLog) Debug.Log($"Overdrive Production: current production cycle time is <b>{nextProductionTimer}</b>.");
            }
            
            else
            {
                if (printLog) Debug.Log($"Current production cycle time is <b>{nextProductionTimer}</b>.");
            }
            
            //Loading Screen (This is the actual Wait Time for production)
            for (int i = 0; i < 10; i++)
            {
                float singleLoadTimer = nextProductionTimer / 10;
                
                factoryLoader.GetChild(i).gameObject.GetComponent<Image>().DOFade(1, singleLoadTimer);
                yield return new WaitForSeconds(singleLoadTimer);
            }
            
            int diceCreated = (int)(productionCurrent);
            
            //Roll for Critical
            if (Utility.Roll(surplusChanceCurrent))
            {
                diceCreated = (int)(diceCreated * surplusValueCurrent);
                if (printLog) Debug.Log($"Surplus Production: <b>{diceCreated}</b> dice have been produced");
            }
            else
            {
                if (printLog) Debug.Log($"Factory: <b>{diceCreated}</b> dice have been produced");
            }
               
            //Check for Transformer interaction
            if (CPU.instance.GetAreaUnlockState(InteractionAreaType.Transformer))
            {
                //Gives every tenth produced die over to the Transformer
                diceUntilTransformer += diceCreated;
                int diceToTransform = diceUntilTransformer / 10;
                diceUntilTransformer %= 10;
                
                //Reduce the number of actually produced dice by the number given to transform
                diceCreated -= diceToTransform;
                
                if (diceToTransform > 0) transformer.TriggerTransformer(diceToTransform);
                
                if (printLog) Debug.Log($"Factory: Transformer receives: <b>{diceToTransform}</b> dice to transform. Dice until Transformer now: {diceUntilTransformer}");
            }
            
            CPU.instance.ChangeResource(Resource.Dice, diceCreated);
            
            //Increase self learning efficiency
            if (selfLearningCurrent > 0)
            {
                efficiencyCurrent += (selfLearningCurrent * selfLearningEfficiencyIncrease) * AIWorkerCurrent;
                if (printLog) Debug.Log($"Factory: Efficiency increased through self learning by <b>{(selfLearningCurrent * selfLearningEfficiencyIncrease) * AIWorkerCurrent}</b>");
            }
          
            
            //Restart
            foreach (Transform nextLoader in factoryLoader)
            {
                nextLoader.GetComponent<Image>().DOFade(0, 0.2f);
            }
            
            
            if (printLog) Debug.Log("|--------------FACTORY PRODUCTION FINISHED--------------|");
            yield return new WaitForSeconds(0.2f);
            
            
        }
        
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        
        data.areaTitle = data.areaTitle = thisInteractionAreaType.ToString();
        data.areaDescription = "In the factory, workers produce dice." +
                               $"<br>Currently, <b>{productionCurrent}</b> dice are produced every <b>{timerCurrent}</b> seconds.";
             
        //Worker
        string workerTooltip = $"<br><br><b>WORKER:</b> Every worker produces 1 dice per cycle.";
        
        //Conveyor
        string conveyorTooltip = $"";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 1))
        {
            conveyorTooltip = $"<br><br><b>CONVEYOR:</b> Every conveyor decreases production time by <b>{conveyorSpeedIncrease}</b> seconds.";
        }
        
        //Tools
        string toolsTooltip = $"";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 2))
        {
            toolsTooltip = $"<br><br><b>TOOLS:</b> Every tool increases production efficiency by: <b>{toolsEfficiencyIncrease * 100}</b>%.";
        }
        
        //Surplus
        string surplusTooltip = $"";
        float currentCriticalIncrease = 0f;
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,3) > 0) currentCriticalIncrease = (float)Math.Round(((surplusValueCurrent - 1) * 100), 2);
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 3))
        {
            surplusTooltip = 
                $"<br><br><b>SURPLUS:</b> <b>{surplusChanceCurrent}</b>% chance per cycle to increase production by <b>{currentCriticalIncrease}</b>%.";
        }
        
        //Overdrive
        string overdriveTooltip = $"";
        float currentOverdriveDecrease = 0f;
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,4) > 0) currentOverdriveDecrease = (float)Math.Round((1/ overdriveValueCurrent - 1) * 100);
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4))
        {
            overdriveTooltip =
                $"<br><br><b>OVERDRIVE:</b> <b>{overdriveChanceCurrent}</b>% chance per cycle to decrease production time by <b>{currentOverdriveDecrease*(-1)}</b>%.";
        }
        
        //AI Worker
        string AIWorkerTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4))
        {
            AIWorkerTooltip =
                $"<br><br><b>AI WORKER:</b> Every AI Worker produces <b>{AIWorkerDiceProduction}</b> dice per second. Costs one worker to purchase";
        }
        
        //Machine Learning
        string machineLearningTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4))
        {
            machineLearningTooltip =
                $"<br><br><b>MACHINE LEARNING:</b> Increases the production efficiency every cycle by <b>{selfLearningEfficiencyIncrease * 100}%</b> per AI worker.";
        }
        
        data.areaDescription += workerTooltip + conveyorTooltip + toolsTooltip + surplusTooltip + overdriveTooltip + AIWorkerTooltip + machineLearningTooltip;
        
       
        
        return data;
    }
   
    #endregion

}
