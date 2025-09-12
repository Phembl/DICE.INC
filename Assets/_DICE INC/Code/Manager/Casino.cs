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


public class Casino : InteractionArea
{
    
    
    [SerializeField] private Transform workshopLoadScreen;
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Casino;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;

    [TitleGroup("Settings")] 
    
    [Header("Bets")]
    [SerializeField] private int costBetsBase;
    [SerializeField] private float costBetsMultiplier;
    [ShowInInspector, ReadOnly] private int currentBets;
    
    [Header("Stakes")]
    [SerializeField] private int costStakesBase;
    [SerializeField] private float costStakesMultiplier;
    [SerializeField] private int currentMin;
    [SerializeField] private int currentMax;
    
    [Header("LuckyNumber")]
    [SerializeField] private int costLuckyNumberBase;
    [SerializeField] private float costLuckyNumberMultiplier;
    
    [Header("Jackpot")]
    [SerializeField] private int costJackpotBase;
    [SerializeField] private float costJackpotMultiplier;
    
    
    [Header("Progress")] 
    [SerializeField] private int betsToUnlockStakes;
    [SerializeField] private int betsToUnlockLuckyNumber;
    [SerializeField] private int betsToUnlockJackpot;


   
    
    private bool casinoCycleActive;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        currentMin = 1;
        currentMax = 20;
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(costBetsBase);
        costs.Add(costStakesBase);
        costs.Add(costLuckyNumberBase);
        costs.Add(costJackpotBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(costBetsMultiplier);
        costs.Add(costStakesMultiplier);
        costs.Add(costLuckyNumberMultiplier);
        costs.Add(costJackpotMultiplier);
        
        return costs;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Workshop, index);
        
        switch (index)
        {
            case 0: //Bets
                currentBets = currentBets + (1 * count);
                CheckProgress();
                break;
            
            case 1: //Stakes
                
                break;
            
            case 2: //Lucky Number
                
                break;
            
            case 3: //Jackpot
             
                break;
            
         
            
        }
    }
    
    void CheckProgress()
    {
        if (!casinoCycleActive && currentBets > 0)
        {
            StartCoroutine(CasinoCycle());
        }
        
        int betCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Casino, 0);
        
        //Unlock Stakes
        if (betCount >= betsToUnlockStakes &&
            !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 1)) UnlockInteractor(1);
        
        //Unlock Lucky Number
        if(betCount >= betsToUnlockLuckyNumber &&
                !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 2)) UnlockInteractor(2);
        
        //Unlock Jackpot
        if (betCount >= betsToUnlockJackpot &&
                 !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 3)) UnlockInteractor(3);
      
        
    }
    
    private IEnumerator CasinoCycle()
    {
        casinoCycleActive = true;
        while (casinoCycleActive)
        {
            //TODO: Casino Cycle
            yield return new WaitForSeconds(0.2f);
        }
        
    }
    
    #endregion

}
