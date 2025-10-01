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

    [TitleGroup("Factory")] 
    [ShowInInspector, ReadOnly] private int factoryLevel;
    [Space]
    [SerializeField] private float timerBase;
    [ShowInInspector, ReadOnly] private float timerCurrent;
    [ShowInInspector, ReadOnly] private float productionCurrent;
    
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
    [Space]
    [SerializeField] private float productionSpeedIncrease;
    [ShowInInspector, ReadOnly] private float productionSpeedCurrent;
    
    [Header("Tools")]
    [SerializeField] private int toolsCostBase;
    [SerializeField] private float toolsCostMult;
    [SerializeField] private int toolsMax;
    [Space]
    [SerializeField] private float efficiencyIncrease;
    private float efficiencyBase = 1f;
    [ShowInInspector, ReadOnly] private float efficiencyCurrent = 1f;
    
    [Header("Surplus")]
    [SerializeField] private int surplusCostBase;
    [SerializeField] private float surplusCostMult;
    [SerializeField] private int surplusMax;
    [Space]
    [SerializeField] private float surplusChanceBase;
    [SerializeField] private float surplusChanceIncrease;
    [SerializeField] private float surplusValueBase;
    [SerializeField] private float surplusValueIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private float surplusChanceCurrent;
    [ShowInInspector, ReadOnly] private float surplusValueCurrent;
    
    [Header("Overdrive")]
    [SerializeField] private int costOverdriveBase;
    [SerializeField] private float costOverdriveMultiplier;
    [SerializeField] private int overdriveMax;
    [Space]
    [SerializeField] private float overdriveChanceBase;
    [SerializeField] private float overdriveChanceIncrease;
    [SerializeField] private float overdriveValueBase;
    [SerializeField] private float overdriveValueIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private float overdriveChanceCurrent;
    [ShowInInspector, ReadOnly] private float overdriveValueCurrent;
    
    [Header("AI Worker")]
    [SerializeField] private int costAIWorkerBase;
    [SerializeField] private float costAIWorkerMultiplier;
    [SerializeField] private int AIWorkerMax;
    [SerializeField] private int AIWorkerDiceProduction;
    [Space]
    [ShowInInspector, ReadOnly] private int AIWorkerCurrent;
   
    
    [Header("Self Learning")]
    [SerializeField] private int costSelfLearningBase;
    [SerializeField] private float costSelfLearningMultiplier;
    [SerializeField] private int selfLearningMax;
    
    
    
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
        costs.Add(costOverdriveBase);
        costs.Add(costAIWorkerBase);
        costs.Add(costSelfLearningBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(workerCostMult);
        costs.Add(conveyorCostMult);
        costs.Add(toolsCostMult);
        costs.Add(surplusCostMult);
        costs.Add(costOverdriveMultiplier);
        costs.Add(costAIWorkerMultiplier);
        costs.Add(costSelfLearningMultiplier);
        
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
                productionSpeedCurrent = productionSpeedIncrease * count;
                timerCurrent = timerBase - productionSpeedCurrent;
                timerCurrent = (float)Math.Round(timerCurrent, 2);
                if (printLog) Debug.Log($"Factory: Conveyor added. Current production speed:{productionSpeedCurrent}. Current production time:{timerCurrent}");
                break;
            
            case 2: //Tools
                efficiencyCurrent = efficiencyBase + (efficiencyIncrease * count);
                if (printLog) Debug.Log($"Factory: Tools added: Current efficiency value:{efficiencyCurrent}");
                break;
            
            case 3: //Surplus
                surplusValueCurrent = surplusValueBase + (surplusValueIncrease * count);
                if (printLog) Debug.Log($"Factory: Surplus upgraded: Value:{surplusValueCurrent}, Chance:{surplusChanceCurrent}%");
                break;
            
            case 4: //Overdrive
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
                
                break;
            
            case 6: // Self Learning
                break;
            
        }

        CheckProgress();
        
    }
    
    protected override void CheckProgress()
    {
        productionCurrent = (workerCurrent + (AIWorkerCurrent * AIWorkerDiceProduction)) * efficiencyCurrent;
        
        factoryLevel =
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 0) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 1) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 2) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 3) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 4) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 5) +
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory, 6);

        //Check if last Interactor has not yet been unlocked
        if (!CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 6))
        {
            ProgressManager.instance.AreaProgress(InteractionAreaType.Factory, factoryLevel);
        }
        
    }
    
    private IEnumerator WorkShopCycle()
    {
        workshopCycleActive = true;
        while (workshopCycleActive)
        {
            float nextProductionTimer = timerCurrent;
            
            if (printLog) Debug.Log("|--------------FACTORY PRODUCTION --------------|");
            if (printLog) Debug.Log($"{productionCurrent} dice will be produced.");
            if (printLog) Debug.Log($"Surplus Chance: {surplusChanceCurrent}.");
            if (printLog) Debug.Log($"Overdrive Chance: {overdriveChanceCurrent}.");

            
            //Roll for Overdrive
            if (Utility.Roll(overdriveChanceCurrent))
            {
                nextProductionTimer /= overdriveValueCurrent;
                if (printLog) Debug.Log($"Overdrive Production: current production cycle time is {nextProductionTimer}");
            }
            
            else
            {
                if (printLog) Debug.Log($"Current production cycle time is {nextProductionTimer}");
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
                if (printLog) Debug.Log($"Surplus Production: {diceCreated} have been produced");
            }
               
            
            CPU.instance.ChangeResource(Resource.Dice, diceCreated);
            
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
        
        int productionCount = (int)(workerCurrent * efficiencyCurrent);
        
        
        data.areaTitle = "Factory";
        data.areaDescription = "In the factory, workers produce dice." +
                               $"<br>Currently, <b>{productionCount}</b> dice are produced every <b>{timerCurrent}</b> seconds.";
             
        //Worker
        string workerText = $"<br><br><b>WORKER:</b> Produce <b>{workerCurrent}</b> dice.";
        
        //Conveyor
        string conveyorText = $"<br><br>???)";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 1))
        {
            conveyorText = $"<br><br><b>CONVEYOR:</b> Every conveyor decreases production time by <b>{productionSpeedIncrease}</b> seconds.";
        }
        
        //Tools
        string toolsText = $"<br><br>???)";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 2))
        {
            toolsText = $"<br><br><b>TOOLS:</b> Every tools increases production efficiency by: <b>{efficiencyIncrease * 100}</b>%.";
        }
        
        //Surplus
        string surplusText = $"<br><br>???";
        float currentCriticalIncrease = 0f;
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,3) > 0) currentCriticalIncrease = (float)Math.Round(((surplusValueCurrent - 1) * 100), 2);
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 3))
        {
            surplusText = 
                $"<br><br><b>SURPLUS:</b> <b>{surplusChanceCurrent}</b>% chance per cycle to increase production by <b>{currentCriticalIncrease}</b>%.";
        }
        
        //Overdrive
        string overdriveText = $"<br><br>???)";
        float currentOverdriveDecrease = 0f;
        if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Factory,4) > 0) currentOverdriveDecrease = (float)Math.Round((1/ overdriveValueCurrent - 1) * 100);
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4))
        {
            overdriveText =
                $"<br><br><b>OVERDRIVE:</b> <b>{overdriveChanceCurrent}</b>% chance per cycle to decrease production time by <b>{currentOverdriveDecrease*(-1)}</b>%.";
        }
        
        data.areaDescription += workerText + conveyorText + toolsText + surplusText + overdriveText;
        
       
        
        return data;
    }
   
    #endregion

}
