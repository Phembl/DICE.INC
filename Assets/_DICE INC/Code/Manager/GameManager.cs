using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DICEINC.Global;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region |-------------- REFERENCES --------------|
    [SerializeField, FoldoutGroup("References")] private CPU cpu;
    [SerializeField, FoldoutGroup("References")] private InteractionArea shop;
    [SerializeField, FoldoutGroup("References")] private InteractionArea workshop;
    [SerializeField, FoldoutGroup("References")] private InteractionArea casino;
    [SerializeField, FoldoutGroup("References")] private InteractionArea diceworld;
    [SerializeField, FoldoutGroup("References")] private InteractionArea stockmarket;
    #endregion
    
    #region |-------------- PROGRESS TRACKING --------------|
    
    [Header("Resource")]
    [ReadOnly, FoldoutGroup("Tracking")] public double pipsOverall => cpu.GetPipsTotal();
    [ReadOnly, FoldoutGroup("Tracking")] public double diceOverall=> cpu.GetDiceTotal();
    [ReadOnly, FoldoutGroup("Tracking")] public double toolsOverall=> cpu.GetToolsTotal();
    [ReadOnly, FoldoutGroup("Tracking")] public double luckOverall=> cpu.GetLuckTotal();
    [ReadOnly, FoldoutGroup("Tracking")] public double MDiceOverall=> cpu.GetMDiceTotal();
    [ReadOnly, FoldoutGroup("Tracking")] public double dataOverall=> cpu.GetDataTotal();
    
    #endregion
    
    #region |-------------- START RESOURCES --------------|
    
    [Header("Start Resources")]
    [SerializeField, FoldoutGroup("Settings")] private int startDiceRollTotal;
    [Space]
    [SerializeField, FoldoutGroup("Settings")] private int startPips;
    [SerializeField, FoldoutGroup("Settings")] private int startDice;
    [SerializeField, FoldoutGroup("Settings")] private int startTools;
    [SerializeField, FoldoutGroup("Settings")] private int startLuck;
    [SerializeField, FoldoutGroup("Settings")] private int startMDice;
    [SerializeField, FoldoutGroup("Settings")] private int startData;
    
    #endregion
    
    #region |-------------- AREA SETTINGS --------------|
    //Shop
    [SerializeField, FoldoutGroup("Shop")] private bool shopUnlocked;
    [SerializeField, FoldoutGroup("Shop")] private int startDiceShop;
    [SerializeField, FoldoutGroup("Shop")] private int startToolsShop;
    [SerializeField, FoldoutGroup("Shop")] private int startDataShop;
    
    //Workshop
    [SerializeField, FoldoutGroup("Workshop")] private bool workshopUnlocked;
    [SerializeField, FoldoutGroup("Workshop")] private int startDicemaker;
    [SerializeField, FoldoutGroup("Workshop")] private int startSpeed;
    [SerializeField, FoldoutGroup("Workshop")] private int startEfficiency;
    [SerializeField, FoldoutGroup("Workshop")] private int startCritical;
    [SerializeField, FoldoutGroup("Workshop")] private int startOverdrive;
    
    //Casino
    [SerializeField, FoldoutGroup("Casino")] private bool casinoUnlocked;
    [SerializeField, FoldoutGroup("Casino")] private int startBets;
    [SerializeField, FoldoutGroup("Casino")] private int startStakes;
    [SerializeField, FoldoutGroup("Casino")] private int startOdds;
    [SerializeField, FoldoutGroup("Casino")] private int startJackpot;
    
    //DiceWorld
    [SerializeField, FoldoutGroup("Diceworld")] private bool diceworldUnlocked;
    [SerializeField, FoldoutGroup("Diceworld")] private int startSides;
    [SerializeField, FoldoutGroup("Diceworld")] private int startAdvantage;
    [SerializeField, FoldoutGroup("Diceworld")] private int startHighRoller;
    [SerializeField, FoldoutGroup("Diceworld")] private int startExplosive;
    
    //Stockmarket
    [SerializeField, FoldoutGroup("Stockmarket")] private bool stockmarketUnlocked;
    [SerializeField, FoldoutGroup("Stockmarket")] private int startGrowthStock;
    [SerializeField, FoldoutGroup("Stockmarket")] private int startMarketCap;
    
    //DataCenter
    [SerializeField, FoldoutGroup("Datacenter")] private bool datacenterUnlocked;
    
    //AutoShoper
    [SerializeField, FoldoutGroup("Auto-shoper")] private bool autoshoperUnlocked;
    
    #endregion
    
   
    public static GameManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    void OnEnable()
    {
        CPU.OnDiceRollTotalChanged += UpdateProgress;
    }
    
    void Start()
    {
        ResourceManager.instance.InitializeResourceManager();
        
        #region |-------------- INIT AREAS --------------|
        //Init Shop
        Debug.Log("|----- START INIT: Shop -----|");
        List <int> shopStartSettings  = new List<int>();
        shopStartSettings.Add(startDiceShop);
        shopStartSettings.Add(startToolsShop);
        shopStartSettings.Add(startDataShop);
        shop.InitializeInteractionArea(shopUnlocked, shopStartSettings);
        Debug.Log("|----- FINISH INIT: Shop -----|");
        
        //Init Workshop
        Debug.Log("|----- START INIT: Workshop -----|");
        List <int> workshopStartSettings  = new List<int>();
        workshopStartSettings.Add(startDicemaker);
        workshopStartSettings.Add(startSpeed);
        workshopStartSettings.Add(startEfficiency);
        workshopStartSettings.Add(startCritical);
        workshopStartSettings.Add(startOverdrive);
        workshop.InitializeInteractionArea(workshopUnlocked, workshopStartSettings);
        Debug.Log("|----- FINISH INIT: Workshop -----|");
        
        //Init Workshop
        Debug.Log("|----- START INIT: Casino -----|");
        List <int> casinoStartSettings  = new List<int>();
        casinoStartSettings.Add(startBets);
        casinoStartSettings.Add(startStakes);
        casinoStartSettings.Add(startOdds);
        casinoStartSettings.Add(startJackpot);
        casino.InitializeInteractionArea(casinoUnlocked, casinoStartSettings);
        Debug.Log("|----- FINISH INIT: Casino -----|");
        
        //Init Diceworld
        Debug.Log("|----- START INIT: Diceworld -----|");
        List <int> diceworldStartSettings  = new List<int>();
        diceworldStartSettings.Add(startSides);
        diceworldStartSettings.Add(startAdvantage);
        diceworldStartSettings.Add(startHighRoller);
        diceworldStartSettings.Add(startExplosive);
        diceworld.InitializeInteractionArea(diceworldUnlocked, diceworldStartSettings);
        Debug.Log("|----- FINISH INIT: Diceworld -----|");
        
        //Init Stockmarket
        Debug.Log("|----- START INIT: Stockmarket -----|");
        List <int> stockmarketStartSettings  = new List<int>();
        stockmarketStartSettings.Add(startGrowthStock);
        stockmarketStartSettings.Add(startMarketCap);
        stockmarket.InitializeInteractionArea(stockmarketUnlocked, stockmarketStartSettings);
        Debug.Log("|----- FINISH INIT: Stockmarket -----|");
      
        #endregion
        
        CPU.instance.ChangeDiceRolledTotal(startDiceRollTotal);
        
        if (startPips > 0) CPU.instance.ChangeResource(Resource.Pips, startPips);
        if (startDice > 0) CPU.instance.ChangeResource(Resource.Dice, startDice);
        if (startTools > 0) CPU.instance.ChangeResource(Resource.Tools, startTools);
        if (startLuck > 0) CPU.instance.ChangeResource(Resource.Luck, startLuck);
        if (startMDice > 0) CPU.instance.ChangeResource(Resource.mDice, startMDice);
        if (startData > 0) CPU.instance.ChangeResource(Resource.Data, startData);
        
        if (workshopUnlocked) shop.UnlockInteractor(1);
    }

    void UpdateProgress()
    {
        int currentDiceRollTotal = CPU.instance.GetDiceRolledTotal();

        if (currentDiceRollTotal >= 0 && !CPU.instance.GetDiceUnlockState())
        {
            CPU.instance.UnlockDice();
            shop.UnlockInteractor(0);
        }

        /*
        if (currentDiceRollTotal >= 10 && !workshopUnlocked)
        {
            workshopUnlocked = true;
            workshop.UnlockArea();
            shop.UnlockInteractor(1);
        }
        */

     

    }
    
}
