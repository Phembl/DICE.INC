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
using System.Linq;


public abstract class InteractionArea : MonoBehaviour
{
    
    [TitleGroup("General")] 
    [SerializeField] private Transform interactorHolder;
    [SerializeField] private CanvasGroup areaCanvas;
    [Space]
    [SerializeField] protected bool printLog;
    
    protected bool areaUnlocked;
   
    private List<int> costBase = new List<int>();
    private List<float> costMult = new List<float>();
    private List<double> costCurrent = new List<double>();
    private List<int> valueMax = new List<int>();
    private List<bool> interactorUnlockStatus =  new List<bool>();
    private List<Interactor> areaInteractors = new List<Interactor>();

    private InteractionAreaType interactionAreaType;

    
    #region |-------------- INIT --------------|

    public void InitializeInteractionArea(
        bool _isUnlocked,
        List<int> startValues)
    {
    
        if (!_isUnlocked)
        {
            areaCanvas.DOFade(0, 0);

            //Reset Start Settings if Area starts locked
            for (int i = 0; i < startValues.Count; i++)
            {
                startValues[i] = 0;
            }
        }
        
        InitSubClass();

        costBase = GetCostsBase();
        costCurrent = costBase.Select(i => (double)i).ToList(); //Converts Base Cost List to current Cost List
        costMult = GetCostsMult();
        valueMax = GetValueMax();
        
        interactionAreaType = GetInteractionAreaType();
        
        //Get the interactors and set initial UnlockState 
        foreach (Transform nextInteractor in interactorHolder)
        {
            areaInteractors.Add(nextInteractor.gameObject.GetComponent<Interactor>());
            interactorUnlockStatus.Add(false);
        }

        //Initialize every Interactor 
        for (int i = 0; i < interactorHolder.childCount; i++)
        {
            areaInteractors[i].InitializeInteractor(costBase[i], valueMax[i]);
        }

        for (int i = 0; i < startValues.Count; i++)
        {
            //This writes the initial count values of the InteractionAreas Interactors into the CPU
            CPU.instance.InitInteractorCountList(interactionAreaType, startValues[i]);
        }

        for (int i = 0; i < startValues.Count; i++)
        {
            if (startValues[i] > 0)
            {
                //This is for testing and loading
                UnlockInteractor(i);
                UpdateInteractorCost(i);
                
                //Makes sure that an interactor is never initialized with a value higher than its Max
                if (valueMax[i] > 0 && startValues[i] > valueMax[i])
                {
                    Debug.LogWarning($"{name} Interactor with index {i} is initialized with a higher start Count than its max value. Value is set = max.");
                    startValues[i] = valueMax[i];
                }
                areaInteractors[i].UpdateCount(startValues[i]);
                
                RunInteraction(i);
            }
            
        }
        
        
        if (_isUnlocked) UnlockArea();

        CheckProgress();
    }

    protected abstract void InitSubClass();
    protected abstract void CheckProgress();
    protected abstract List<int> GetCostsBase(); 
    protected abstract List<float> GetCostsMult();
    protected abstract List<int> GetValueMax(); 
    protected abstract InteractionAreaType GetInteractionAreaType();
    
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
            string interactorName = interactorHolder.GetChild(index).gameObject.GetComponent<Interactor>().GetInteractorName();
            Debug.LogWarning($"Interactor: {interactorName} is supposed to be unlocked but is already unlocked. Unlock aborted.");
            return;
        }

        interactorUnlockStatus[index] = true;
        CPU.instance.UnlockInteractor(interactionAreaType, index);
        areaInteractors[index].Unlock();
        
    }
    
    private void UpdateInteractorCost(int index)
    {
        if (interactorHolder.GetChild(index).gameObject == null)
        {
            Debug.LogWarning("The cost of an unknown interactor is supposed to be updated. Update is cancelled");
            return;
        }
        
        if (!interactorUnlockStatus[index])
        {
            string interactorName = interactorHolder.GetChild(index).gameObject.GetComponent<Interactor>().GetInteractorName();
            Debug.LogWarning($"The cost of locked interactor: {interactorName} is supposed to be updated. Update is cancelled");
            return; 
        }
        
        if (interactionAreaType == InteractionAreaType.Shop)
        {
            //If the interactor is Shop-> Dice, the cost are updated from the subclass
            if (index == 0) return;
        }
        
        if (interactionAreaType == InteractionAreaType.Diceworld)
        {
            //DiceWorld just increases each cost by one
            costCurrent[index] += 1;
            areaInteractors[index].UpdatePrice(costCurrent[index]);
            return;
        }
        
        int interactorCount = CPU.instance.GetAreaInteractorCount(interactionAreaType, index);
        
        double interactorCost = 
            Math.Round(costBase[index] * Math.Pow(costMult[index], interactorCount));
        
        costCurrent[index] = interactorCost;
        areaInteractors[index].UpdatePrice(costCurrent[index]);
    }

    
    //This is called from the Shop child class with the current Dice cost
    protected void ShopCostUpdateDice(int newCost)
    {
        if (interactionAreaType != InteractionAreaType.Shop) return;
        
        //Dice
        costBase[0] = newCost;
        areaInteractors[0].UpdatePrice((double)costBase[0]);
        costCurrent[0] = (double)costBase[0];
    }
    #endregion
    
    #region |-------------- INTERACTION --------------|
    
    //Called by Shop Interactors
    public void Interaction(int index, Resource costResource)
    {
        //Pay Cost of interaction
        CPU.instance.ChangeResource(costResource, -costCurrent[index]);
        
        //Add 1 to internal count of Interactor
        CPU.instance.IncreaseAreaInteractorCount(interactionAreaType, index);
        
        //Update Counter Display on Interactor
        //Shop Count doesn't need to be updated
        if (interactionAreaType != InteractionAreaType.Shop) 
            areaInteractors[index].UpdateCount(CPU.instance.GetAreaInteractorCount(interactionAreaType, index));
        
        UpdateInteractorCost(index); //Update Cost
        RunInteraction(index);
        
    }
    
    protected abstract void RunInteraction(int index);

    #endregion

}
