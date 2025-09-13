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
    
    

    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Casino;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;

    [SerializeField] private Transform displayHouseNumbers;
    [SerializeField] private Transform displayPlayerNumbers;
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private Sprite winIcon;
    [SerializeField] private Sprite loseIcon;
    [SerializeField] private TMP_Text outputTMP;
    [TitleGroup("Settings")] 
    
    [Header("Bets")]
    [SerializeField] private int costBetsBase;
    [SerializeField] private float costBetsMultiplier;
    [ShowInInspector, ReadOnly] private int currentBets;
    
    [Header("Stakes")]
    [SerializeField] private int costStakesBase;
    [SerializeField] private float costStakesMultiplier;
    [SerializeField] private int stakesNumberIncrease;
    [ShowInInspector, ReadOnly] private int currentMin;
    [ShowInInspector, ReadOnly] private int currentMax;
    
    [Header("Odds")]
    [SerializeField] private int costOddsBase;
    [SerializeField] private float costOddsMultiplier;
    [ShowInInspector, ReadOnly] private int currentOdds;
    
    [Header("Jackpot")]
    [SerializeField] private int costJackpotBase;
    [SerializeField] private float costJackpotMultiplier;
    [SerializeField] private int jackpotPrizeBase;
    [ShowInInspector, ReadOnly] private float currentJackpotMult = 1;
    
    
    [Header("Progress")] 
    [SerializeField] private int betsToUnlockStakes;
    [SerializeField] private int betsToUnlockOdds;
    [SerializeField] private int betsToUnlockJackpot;


   
    
    private bool casinoCycleActive;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
        currentMin = 1;
        currentMax = 20;
        
        outputTMP.text = "";
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(costBetsBase);
        costs.Add(costStakesBase);
        costs.Add(costOddsBase);
        costs.Add(costJackpotBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(costBetsMultiplier);
        costs.Add(costStakesMultiplier);
        costs.Add(costOddsMultiplier);
        costs.Add(costJackpotMultiplier);
        
        return costs;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Casino, index);
        
        switch (index)
        {
            case 0: //Bets
                currentBets = count;
                CheckProgress();
                break;
            
            case 1: //Stakes
                currentMax = (stakesNumberIncrease * count);
                currentMin = (stakesNumberIncrease * count);
                break;
            
            case 2: //Odds
                currentOdds = count;
                break;
            
            case 3: //Jackpot
                currentJackpotMult = (jackpotPrizeBase * count);
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
        if(betCount >= betsToUnlockOdds &&
                !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 2)) UnlockInteractor(2);
        
        //Unlock Jackpot
        if (betCount >= betsToUnlockJackpot &&
                 !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 3)) UnlockInteractor(3);
      
        
    }
    
    private IEnumerator CasinoCycle()
    {
        if (casinoCycleActive) yield return null;
        
        casinoCycleActive = true;
        List<int> houseNumbers = new List<int>();
        List<int> playerNumbers = new List<int>();
        int overallWin = 0;
        int jackpotNumber = -1;
        
        while (casinoCycleActive)
        {
            //Start new Cycle
            houseNumbers.Clear();
            playerNumbers.Clear();
            overallWin = 0;
            
           outputTMP.text = "Next Round!";
           int evaluatedBets = currentBets;
           
           
           if (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Casino,3) > 0) jackpotNumber = Random.Range(0, currentBets);
           
            //Make Bets
            for (int i = 0; i < evaluatedBets; i++)
            {
                houseNumbers.Add(Random.Range(currentMin + currentOdds, currentMax + 1));
                Debug.Log(houseNumbers[i]);
                GameObject numberDisplay = Instantiate(numberPrefab, displayHouseNumbers);
                numberDisplay.GetComponent<TMP_Text>().text = houseNumbers[i].ToString();
                if (i == jackpotNumber)
                    numberDisplay.GetComponent<TMP_Text>().text = $"{numberDisplay.GetComponent<TMP_Text>().text}!";
                
                playerNumbers.Add(Random.Range(currentMin + currentOdds, currentMax + 1));
                numberDisplay = Instantiate(numberPrefab, displayPlayerNumbers);
                numberDisplay.GetComponent<TMP_Text>().text = playerNumbers[i].ToString();
                
                yield return new WaitForSeconds(0.3f);
            }
            
            yield return new WaitForSeconds(1.5f);
            
            //Check Bets
            for (int i = 0; i < evaluatedBets; i++)
            {
                int houseNumber = houseNumbers[i];
                int playerNumber = playerNumbers[i];

                if (houseNumber != playerNumber) //Lose
                {
                    displayHouseNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = loseIcon;
                    displayPlayerNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = loseIcon;
                }
                else //Win
                {
                    displayHouseNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = winIcon;
                    displayPlayerNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = winIcon;
                    overallWin += houseNumbers[i];
                }
                
                displayHouseNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                displayPlayerNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                
                yield return new WaitForSeconds(0.3f);
            }
            
            yield return new WaitForSeconds(1.5f);

            if (overallWin > 0)
            {
                overallWin = (int)(overallWin * currentJackpotMult);
                outputTMP.text = $"You won {overallWin} pips!";
                CPU.instance.ChangeResource(Resource.Pips, overallWin);
            
            }
            
            else outputTMP.text = "You won nothing...";
            
            yield return new WaitForSeconds(2f);

            foreach (Transform display in displayHouseNumbers)
            {
                Destroy(display.gameObject);
            }
            foreach (Transform display in displayPlayerNumbers)
            {
                Destroy(display.gameObject);
            }
            
            yield return new WaitForSeconds(0.5f);
            
        }
        
    }
    
    #endregion

}
