/*
using System;
using System.Collections;
using System.Collections.Generic;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class WorkshopManager : MonoBehaviour
{
    
    [TitleGroup("References")] 
    [SerializeField] private Transform interactorHolder;
    [SerializeField] private Transform workshopLoadScreen;
    [SerializeField] private CanvasGroup areaCanvas;

    [TitleGroup("Workshop")] 
    [SerializeField] private float workshopTimer;
    [ShowInInspector, ReadOnly] private float currentTimer;
    
    [TitleGroup("Dicemaker")]
    [SerializeField] private int costDicemakerBase;
    [SerializeField] private float costDicemakerMultiplier;
    [ShowInInspector, ReadOnly] private double costDicemakerCurrent;
    
    [TitleGroup("Speed")]
    [SerializeField] private int costSpeedBase;
    [SerializeField] private float costSpeedMultiplier;
    [ShowInInspector, ReadOnly] private double costSpeedCurrent;
    [SerializeField] private float speedIncrease;
    
    [TitleGroup("Efficiency")]
    [SerializeField] private int costEfficiencyBase;
    [SerializeField] private float costEfficiencyMultiplier;
    [ShowInInspector, ReadOnly] private double costEfficiencyCurrent;
    [SerializeField] private int efficiencyIncrease;
    [ShowInInspector, ReadOnly] private int currentEfficiency = 1;
    
    [TitleGroup("Critical")]
    [SerializeField] private int costCriticalBase;
    [SerializeField] private float costCriticalMultiplier;
    [ShowInInspector, ReadOnly] private double costCriticalCurrent;
    [SerializeField] private float criticalChanceBase;
    [SerializeField] private float criticalChanceMult;
    [SerializeField] private float criticalValueBase;
    [SerializeField] private float criticalValueMult;
    [ShowInInspector, ReadOnly] private float currentCriticalChance;
    [ShowInInspector, ReadOnly] private float currentCriticalValue;
    
    [TitleGroup("Overdrive")]
    [SerializeField] private int costOverdriveBase;
    [SerializeField] private float costOverdriveMultiplier;
    [ShowInInspector, ReadOnly] private double costOverdriveCurrent;
    [SerializeField] private float overdriveChanceBase;
    [SerializeField] private float overdriveChanceMult;
    [SerializeField] private float overdriveValueBase;
    [SerializeField] private float overdriveValueMult;
    [ShowInInspector, ReadOnly] private float currentOverdriveChance;
    [ShowInInspector, ReadOnly] private float currentOverdriveValue;
    
    [TitleGroup("Progress")] 
    [SerializeField] private int dicemakerToUnlockSpeed;
    [SerializeField] private int dicemakerToUnlockEfficiency;
    [SerializeField] private int dicemakerToUnlockCritical;
    

    private bool areaUnlocked;
   
    private List<bool> interactorUnlockStatus =  new List<bool>();
    private List<Interactor> areaInteractors = new List<Interactor>();

    
    private CPU DataStorage;

    
    public static WorkshopManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
       
    }
    #region |-------------- INIT --------------|

    public void InitializeWorkshop(
        bool _isUnlocked = false,
        int _startDicemaker = 0,
        int _startSpeed = 0,
        int _startEfficiency = 0,
        int _startCritical = 0,
        int _startOverdrive = 0)
    {
        DataStorage = CPU.instance;
        currentTimer = workshopTimer;

        if (!_isUnlocked)
        {
            areaCanvas.alpha = 0;
        }

        //Get the interactors and set initial UnlockState 
        foreach (Transform nextInteractor in interactorHolder)
        {
            areaInteractors.Add(nextInteractor.gameObject.GetComponent<Interactor>());
            interactorUnlockStatus.Add(false);
        }

        areaInteractors[0].InitializeInteractor(costDicemakerBase);
        areaInteractors[1].InitializeInteractor(costSpeedBase);
        areaInteractors[2].InitializeInteractor(costEfficiencyBase);
        areaInteractors[3].InitializeInteractor(costCriticalBase);
        areaInteractors[4].InitializeInteractor(costOverdriveBase);
        
        
        
        #region |-------------- CHECK INITIAL UNLOCKS & VALUES--------------|

        //This is for testing and loading
        //Dicemaker
        if (_startDicemaker > 0)
        {
            DataStorage.ChangeWorkshopValue(0, _startDicemaker);
            UnlockInteractor(0);
            UpdateInteractorCost(0);
            areaInteractors[0].UpdateCount(_startDicemaker);
            
            StartCoroutine(WorkShopCycle());
            CheckProgress();
        }

        if (_startSpeed > 0)
        {
            DataStorage.ChangeWorkshopValue(1, _startSpeed);
            UnlockInteractor(1);
            UpdateInteractorCost(1);
            areaInteractors[1].UpdateCount(_startSpeed);
            
            AddPurchase(1);
        }

        if (_startEfficiency > 0)
        {
            DataStorage.ChangeWorkshopValue(2, _startEfficiency);
            UnlockInteractor(2);
            UpdateInteractorCost(2);
            areaInteractors[2].UpdateCount(_startEfficiency);
            
            AddPurchase(2);
        }

        if (_startCritical > 0)
        {
            DataStorage.ChangeWorkshopValue(3, _startCritical);
            UnlockInteractor(3);
            UpdateInteractorCost(3);
            areaInteractors[3].UpdateCount(_startCritical);
            
            AddPurchase(3);
        }

        if (_startOverdrive > 0)
        {
            DataStorage.ChangeWorkshopValue(4, _startOverdrive);
            UnlockInteractor(4);
            UpdateInteractorCost(4);
            areaInteractors[4].UpdateCount(_startOverdrive);
            
            AddPurchase(4);
        }

        #endregion

    foreach (Transform nextLoader in workshopLoadScreen)
        {
            nextLoader.GetComponent<Image>().DOFade(0, 0);
        }
    
        
    }
    #endregion
    
    #region |-------------- UNLOCK & UPDATE --------------|
    
    public void UnlockArea()
    {
        if (areaUnlocked) return;
        areaUnlocked = true;
        
        UnlockInteractor(0);
        areaCanvas.DOFade(1, 0.5f);
    }
    
    public void UnlockInteractor(int index)
    {
        
        if (index > areaInteractors.Count)
        {
            Debug.LogWarning("An unknown interactor is supposed to be unlocked. Unlock aborted.");
            return;
        }

        if (interactorUnlockStatus[index] == true)
        {
            Debug.LogWarning("An interactor is supposed to be unlocked which is already unlocked. Unlock aborted.");
            return;
        }

        interactorUnlockStatus[index] = true;
        
        areaInteractors[index].Unlock();
        
    }
    
    private void UpdateInteractorCost(int index)
    {

        if (interactorHolder.GetChild(index).gameObject == null)
        {
            Debug.LogWarning("The cost of an unknown interactor is supposed to be updated. Update is cancelled");
            return;
        }
        
        double itemCost = 0;
        
        switch (index)
        {
            case 0: //Dicemaker
                costDicemakerCurrent = (int)(costDicemakerBase * Math.Pow(costDicemakerMultiplier, DataStorage.GetWorkshopDicemaker()));
                itemCost = costDicemakerCurrent;
                break;
            
            case 1: //Speed
                costSpeedCurrent = (int)(costSpeedBase * Math.Pow(costSpeedMultiplier, DataStorage.GetWorkshopSpeed()));
                itemCost = costSpeedCurrent;
                break;
            
            case 2: //Efficiency
                costEfficiencyCurrent = (int)(costEfficiencyBase * Math.Pow(costEfficiencyMultiplier, DataStorage.GetWorkshopEfficiency()));
                itemCost = costEfficiencyCurrent;
                break;
            
            case 3: //Critical
                costCriticalCurrent = (int)(costCriticalBase * Math.Pow(costCriticalMultiplier, DataStorage.GetWorkshopCritical()));
                itemCost = costCriticalCurrent;
                break;
            
            case 4: //Overdrive
                costOverdriveCurrent = (int)(costOverdriveBase * Math.Pow(costOverdriveMultiplier, DataStorage.GetWorkshopOverdrive()));
                itemCost = costOverdriveCurrent;
                break;
                
        }
        
        areaInteractors[index].UpdatePrice(itemCost);

    }
    #endregion
    
    #region |-------------- INTERACTION --------------|
    
    //Called by Shop Interactors
    public void WorkshopInteraction(int _interactorIndex, Resource _costResource)
    {
        int index = _interactorIndex;
        int newCount = -1;
        double cost = -1;
        
        switch (_interactorIndex)
        {
            case 0: //Buy Dicemaker
                cost = costDicemakerCurrent;
                newCount = DataStorage.GetWorkshopDicemaker();
                break;
            
            case 1: //Buy Speed
                cost = costSpeedCurrent;
                newCount = DataStorage.GetWorkshopSpeed();
                break;
            
            case 2: //Buy Efficiency
                cost = costEfficiencyCurrent;
                newCount = DataStorage.GetWorkshopEfficiency();
                break;
          
            case 3: //Buy Critical
                cost = costCriticalCurrent;
                newCount = DataStorage.GetWorkshopCritical();
                break;
            
            case 4: //Overdrive
                cost = costOverdriveCurrent;
                newCount = DataStorage.GetWorkshopOverdrive();
                break;
                
        }
        
        DataStorage.ChangeResource(_costResource, -cost);
        DataStorage.ChangeWorkshopValue(index, 1);
        areaInteractors[index].UpdateCount(newCount);
        UpdateInteractorCost(index);
        AddPurchase(index);
        
        //Overall Unlock progress is usually dependent on first interactor
        if(index == 0) CheckProgress();
    }
    
    private void CheckProgress()
    {
        if (DataStorage.GetWorkshopDicemaker() == dicemakerToUnlockSpeed) UnlockInteractor(1);
        else if(DataStorage.GetWorkshopDicemaker() == dicemakerToUnlockEfficiency) UnlockInteractor(2);
        else if (DataStorage.GetWorkshopDicemaker() == dicemakerToUnlockCritical)
        {
            UnlockInteractor(3);
            ResourceManager.instance.UnlockResource(Resource.Luck);
        }
    }

    private void AddPurchase(int _itemIndex)
    {
        switch (_itemIndex)
        {
            case 0:
                if (DataStorage.GetWorkshopDicemaker() == 1) StartCoroutine(WorkShopCycle());
                break;
            
            case 1:
                currentTimer = workshopTimer * math.pow((1 - speedIncrease), DataStorage.GetWorkshopSpeed());
                break;
            
            case 2:
                currentEfficiency = (efficiencyIncrease * DataStorage.GetWorkshopEfficiency());
                break;
            
            case 3:
                currentCriticalChance = (float)(criticalChanceBase * Math.Pow(criticalChanceMult, DataStorage.GetWorkshopCritical()));
                currentCriticalValue = (float)(criticalValueBase * Math.Pow(criticalValueMult, DataStorage.GetWorkshopCritical()));
                break;
            
        }
    }
    
    private IEnumerator WorkShopCycle()
    {
        bool isRunning = true;
        while (isRunning)
        {
            for (int i = 0; i < 10; i++)
            {
                float singleLoadTimer = currentTimer / 10;
                workshopLoadScreen.GetChild(i).gameObject.GetComponent<Image>().DOFade(1, singleLoadTimer);
                yield return new WaitForSeconds(singleLoadTimer);
            }
            
            int diceCreated = DataStorage.GetWorkshopDicemaker() * currentEfficiency;
            
            //Critical production
            float critRoll = Random.Range(0, 100);
            if (currentCriticalChance >= critRoll)
            {
                diceCreated = (int)(diceCreated * currentCriticalValue);
                Debug.Log($"Critical Production: {diceCreated} have been produced");
            }
            
            DataStorage.ChangeResource(Resource.Dice, diceCreated);
            
            //Restart
            foreach (Transform nextLoader in workshopLoadScreen)
            {
                nextLoader.GetComponent<Image>().DOFade(0, 0.2f);
                
            }
            yield return new WaitForSeconds(0.2f);
        }
        
    }
    
    #endregion

}
*/
