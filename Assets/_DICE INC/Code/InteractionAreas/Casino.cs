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
    
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Casino;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    [SerializeField] private Transform displayHouseNumbers;
    [SerializeField] private Transform displayPlayerNumbers;
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private Sprite winIcon;
    [SerializeField] private Sprite loseIcon;
    [SerializeField] private TMP_Text outputTMP;
    
    [TitleGroup("Casino")] 
    [Header("Bets")]
    [SerializeField] private int betsCostBase;
    [SerializeField] private float betsCostMult;
    [SerializeField] private int betsMax;
    [Space]
    [ShowInInspector, ReadOnly] private int betsCurrent;
    
    [Header("Stakes")]
    [SerializeField] private int stakesCostBase;
    [SerializeField] private float stakesCostMult;
    [SerializeField] private int stakesMax;
    [Space]
    [SerializeField] private int stakesValueIncrease;
    [ShowInInspector, ReadOnly] private int currentMin;
    [ShowInInspector, ReadOnly] private int currentMax;
    
    [Header("Odds")]
    [SerializeField] private int oddsCostBase;
    [SerializeField] private float oddsCostMult;
    [SerializeField] private int oddsMax;
    [Space]
    [ShowInInspector, ReadOnly] private int oddsCurrent;
    
    [Header("Jackpot")]
    [SerializeField] private int jackpotCostBase;
    [SerializeField] private float jackpotCostMult;
    [SerializeField] private int jackpotMax;
    [Space]
    [SerializeField] private int jackpotPrizeBase;
    [SerializeField] private int jackpotPercentageRange;
    [ShowInInspector, ReadOnly] private int jackpotCurrent = 1;
    
    
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
        
        costs.Add(betsCostBase);
        costs.Add(stakesCostBase);
        costs.Add(oddsCostBase);
        costs.Add(jackpotCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(betsCostMult);
        costs.Add(stakesCostMult);
        costs.Add(oddsCostMult);
        costs.Add(jackpotCostMult);
        
        return costs;
    }

    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(betsMax);
        max.Add(stakesMax);
        max.Add(oddsMax);
        max.Add(jackpotMax);
        
        return max;
    }
    
   
    #endregion
    
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|
    
    
    protected override void RunInteraction(int index)
    {
        int count = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Casino, index);
        
        switch (index)
        {
            case 0: //Bets
                betsCurrent = count;
                CheckProgress();
                break;
            
            case 1: //Stakes
                currentMax = (stakesValueIncrease * count);
                currentMin = (stakesValueIncrease * count);
                break;
            
            case 2: //Odds
                oddsCurrent = count;
                break;
            
            case 3: //Jackpot
                //Jackpot Count is calculated within the jackpot mult roller
                break;
            
         
            
        }
    }
    
    protected override void CheckProgress()
    {
        if (!casinoCycleActive && betsCurrent > 0)
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
        
        while (casinoCycleActive)
        {
            //Start new Cycle
            List<int> houseNumbers = new List<int>();
            List<int> playerNumbers = new List<int>();
            int overallWin = 0;
            
            int jackpotIndex = -1;
            bool jackpotUnlocked = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Casino, 3) > 0;
            
           outputTMP.text = "Next Round!";
           if (printLog) Debug.Log("|--------------CASINO ROUND --------------|");
           int evaluatedBets = betsCurrent;
           
           //If Jackpot is active, define the jackpot index
           if (jackpotUnlocked)
           {
               jackpotIndex = Random.Range(0, betsCurrent);
               if (printLog) Debug.Log($"New Jackpot index: {jackpotIndex}.");
           }
           
            //Make Bets
            for (int i = 0; i < evaluatedBets; i++)
            {
                //Roll and write house numbers
                houseNumbers.Add(Random.Range(currentMin + oddsCurrent, currentMax + 1));
                GameObject numberDisplay = Instantiate(numberPrefab, displayHouseNumbers);
                numberDisplay.GetComponent<TMP_Text>().text = houseNumbers[i].ToString();

                if (i == jackpotIndex) //Create Jackpot marker (!)
                {
                    numberDisplay.GetComponent<TMP_Text>().text = $"{numberDisplay.GetComponent<TMP_Text>().text}!";
                }
                    
                //Roll and write player numbers
                playerNumbers.Add(Random.Range(currentMin + oddsCurrent, currentMax + 1));
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

                //Compare the next house number with player number
                
                if (houseNumber != playerNumber) //Lose
                {
                    displayHouseNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = loseIcon;
                    displayPlayerNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = loseIcon;
                }
                else //Win
                {
                    displayHouseNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = winIcon;
                    displayPlayerNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = winIcon;

                    int prize = houseNumbers[i];
                    
                    if (i == jackpotIndex) //Win Jackpot
                    {
                        jackpotCurrent = RollJackpotMult();
                        if (printLog) Debug.Log($"Jackpot won. Prize: {prize}");
                        prize *= jackpotCurrent;
                        
                    }
                    
                    overallWin += prize;
                }
                
                displayHouseNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                displayPlayerNumbers.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                
                yield return new WaitForSeconds(0.3f);
            }
            
            yield return new WaitForSeconds(1.5f);

            if (overallWin > 0)
            {
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

    int RollJackpotMult()
    {
        int prizeRange = jackpotPrizeBase / jackpotPercentageRange;
        int newJackpotMult = Random.Range(jackpotPrizeBase - prizeRange , jackpotPrizeBase + prizeRange + 1);
        
        return newJackpotMult * CPU.instance.GetAreaInteractorCount(InteractionAreaType.Casino,3);
    }
    
    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = "Casino";
        data.areaDescription = "In the casino, the";

        //Bets TT
        string betsTooltip = "<br><br><b>BETS:</b>";
        
        //StakesCap TT
        string stakesTooltip = $"<br><br>??? (Bets to unlock: {betsToUnlockStakes})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 1))
        {
            
            stakesTooltip = $"<br><br><b>STAKES:</b> Each point.</b>";
        }
        
        //Odds TT
        string oddsTooltip = $"<br><br>??? (Bets to unlock: {betsToUnlockOdds})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 2))
        {
            
            oddsTooltip = $"<br><br><b>ODDS:</b> Each point.</b>";
        }
        
        //Jackpot TT
        string jackpotTooltip = $"<br><br>??? (Bets to unlock: {betsToUnlockJackpot})";
        if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Casino, 3))
        {
            
            jackpotTooltip = $"<br><br><b>JACKPOT:</b> Each point.</b>";
        }
        
        
        data.areaDescription += betsTooltip + stakesTooltip + oddsTooltip + jackpotTooltip;
        
        return data;
    }
   
    #endregion

}
