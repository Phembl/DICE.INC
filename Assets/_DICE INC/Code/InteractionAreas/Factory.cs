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
    [SerializeField] private TMP_Text tempTransformerTMP;
    [SerializeField] private Sprite[] diceSprites;

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
    [SerializeField] private int surplusValueBase;
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
    [SerializeField] private float overdriveTimePercent;
    [Space]
    [ShowInInspector, ReadOnly] private int overdriveCurrent;
    [ShowInInspector, ReadOnly] private float overdriveChanceCurrent;

    
    [Header("AI Worker")]
    [SerializeField] private int AIWorkerCostBase;
    [SerializeField] private float AIWorkerCostMultiplier;
    [SerializeField] private int AIWorkerMax;
    [SerializeField] private int AIWorkerDiceProduction;
    [Space]
    [ShowInInspector, ReadOnly] private int AIWorkerCurrent;
    
    [Header("Machine Learning")]
    [SerializeField] private int machineLearningCostBase;
    [SerializeField] private float machineLearningCostMultiplier;
    [SerializeField] private int machineLearningMax;
    [SerializeField] private float machineLearningEfficiencyIncrease;
    [Space]
    [ShowInInspector, ReadOnly] private int machineLearningCurrent;

    private int diceUntilTransformer;
    private float percentageDiceTransformer;
    
    #endregion
    
    private bool factoryCycleActive;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        timerCurrent = timerBase;
        efficiencyCurrent = efficiencyBase;
        
        surplusChanceCurrent = surplusChanceBase;
        surplusCurrent = surplusValueBase;
        
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
        costs.Add(machineLearningCostBase);
        
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
        costs.Add(machineLearningCostMultiplier);
        
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
        max.Add(machineLearningMax);
        
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
                if (!factoryCycleActive) StartCoroutine(FactoryCycle());
                
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
                if (printLog) Debug.Log($"Factory: Overdrive upgraded: Chance:{overdriveChanceCurrent}%");
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
                machineLearningCurrent = count;
                
                
                if (printLog) Debug.Log($"Factory: Self learning updated: Current self learning:{machineLearningCurrent}");
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
        //Stops after Overdrive because AI Worker and SelfLearning are unlocked when Data Center is unlocked by Lab
        if (!CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4))
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
        
        //Unlock Material when conveyors are unlocked
        if (level == unlockLevels[1]) ProgressManager.instance.UnlockResource(Resource.Material);
        
        if (AIWorkerCurrent >= 10 && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 6)) UnlockInteractor(6);
    }
    
    
    private IEnumerator FactoryCycle()
    {
        factoryCycleActive = true;
        while (factoryCycleActive)
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
                nextProductionTimer *= overdriveTimePercent;
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
                
                factoryLoader.GetChild(i).gameObject.GetComponent<Image>().sprite = diceSprites[Random.Range(0, diceSprites.Length)];
                factoryLoader.GetChild(i).gameObject.GetComponent<Image>().DOFade(1, singleLoadTimer);
                yield return new WaitForSeconds(singleLoadTimer);
            }
            
            int diceCreated = (int)(productionCurrent);
            
            //Roll for Surplus
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
             
                int diceNeeded = transformer.GetDiceNeeded();
                
                float percentageProduced = (float)diceCreated / diceNeeded;
                percentageProduced = (float)Math.Round(percentageProduced * 100f,2);
                
                percentageDiceTransformer += percentageProduced;
                
                if (percentageDiceTransformer >= 100)
                {
                    //TODO: Update Counter Display.
                    
                    int materialProduced = (int)percentageDiceTransformer / 100;

                    percentageDiceTransformer = ((percentageDiceTransformer / 100) % 1f) * 100;
                    percentageDiceTransformer = (float)Math.Round(percentageDiceTransformer, 2);
                    
                    if (printLog) Debug.Log($"Factory: Transformer receives <b>{materialProduced}</b> material to produce.");
                    transformer.TriggerTransformer(materialProduced);
                    
                }
                
                tempTransformerTMP.text = $"{percentageDiceTransformer}%";
                
            }
            
            CPU.instance.ChangeResource(Resource.Dice, diceCreated);
            
            //Increase self learning efficiency
            if (machineLearningCurrent > 0)
            {
                efficiencyCurrent += (machineLearningCurrent * machineLearningEfficiencyIncrease) * AIWorkerCurrent;
                if (printLog) Debug.Log($"Factory: Efficiency increased through machine learning by <b>{(machineLearningCurrent * machineLearningEfficiencyIncrease) * AIWorkerCurrent}</b>");
            }
          
            //Update Tooltip if Factory TT is open
            if (TooltipManager.instance.GetTooltipStatus() && TooltipManager.instance.GetCurrentInteractionArea() == InteractionAreaType.Factory)
            {
                TooltipManager.instance.UpdateTooltip(InteractionAreaType.Factory);
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
        string workerTooltip = $"<br><br><b>WORKER:</b> Every worker produces 1 dice per cycle." +
                               $"<br>Current workers: <b>{workerCurrent}</b>.";
        
        //Conveyor
        string conveyorTooltip = $"";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 1))
        {
            conveyorTooltip = $"<br><br><b>CONVEYOR:</b> Every conveyor decreases production time by <b>{conveyorSpeedIncrease}</b> seconds." +
                              $"<br>Current time decrease: <b>{conveyorSpeedIncrease * conveyorCurrent}</b> seconds.";
        }
        
        //Tools
        string toolsTooltip = $"";
        
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 2))
        {
            toolsTooltip = $"<br><br><b>TOOLS:</b> Every tool increases production efficiency by: <b>{toolsEfficiencyIncrease * 100}</b>%." +
                           $"<br>Current efficiency: <b>{efficiencyCurrent * 100}</b>%.";
        }
        
        //Surplus
        string surplusTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 3))
        {
            surplusTooltip = 
                $"<br><br><b>SURPLUS:</b> Introduces a <b>{surplusChanceCurrent}</b>% per-cycle chance to increase production. Every point increases surplus production by <b>{surplusValueIncrease * 100}</b>%." +
                $"<br>Current surplus production: <b>{surplusValueCurrent * 100}</b>%.";
        }
        
        //Overdrive
        string overdriveTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4))
        {
            overdriveTooltip =
                $"<br><br><b>OVERDRIVE:</b> Introduces a chance per cycle to decrease production time to <b>{overdriveTimePercent * 100}</b>%. Every point increases the chance by <b>{overdriveChanceIncrease}</b>%." +
                $"<br>Current overdrive chance: <b>{overdriveChanceCurrent}</b>%.";
        }
        
        //AI Worker
        string AIWorkerTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 5))
        {
            AIWorkerTooltip =
                $"<br><br><b>AI WORKER:</b> Every AI Worker produces <b>{AIWorkerDiceProduction}</b> dice per second." +
                $"<br>Costs one worker to purchase.";
        }
        
        //Machine Learning
        string machineLearningTooltip = $"";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 6))
        {
            machineLearningTooltip =
                $"<br><br><b>MACHINE LEARNING:</b> Increases the production efficiency every cycle by <b>{machineLearningEfficiencyIncrease * 100}%</b> per AI worker.";
        }
        
        data.areaDescription += workerTooltip + conveyorTooltip + toolsTooltip + surplusTooltip + overdriveTooltip + AIWorkerTooltip + machineLearningTooltip;
        
       
        
        return data;
    }
   
    #endregion

}
