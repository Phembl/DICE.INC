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
    [SerializeField, FoldoutGroup("References")] private Lab lab;
    [SerializeField, FoldoutGroup("References")] private InteractionArea shop;
    [SerializeField, FoldoutGroup("References")] private InteractionArea workshop;
    [SerializeField, FoldoutGroup("References")] private InteractionArea casino;
    [SerializeField, FoldoutGroup("References")] private InteractionArea diceworld;
    [SerializeField, FoldoutGroup("References")] private InteractionArea stockmarket;
    #endregion
    
    #region |-------------- PROGRESS TRACKING --------------|
    
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
    [SerializeField, FoldoutGroup("Settings")] private int startResearchProgress;
    [Space]
    [SerializeField, FoldoutGroup("Settings")] private int startPips;
    [SerializeField, FoldoutGroup("Settings")] private int startDice;
    [SerializeField, FoldoutGroup("Settings")] private int startTools;
    [SerializeField, FoldoutGroup("Settings")] private int startLuck;
    [SerializeField, FoldoutGroup("Settings")] private int startMDice;
    [SerializeField, FoldoutGroup("Settings")] private int startData;
    
    #endregion
    
    #region |-------------- AREA SETTINGS --------------|
    //Import
    [SerializeField, FoldoutGroup("Import")] private bool shopUnlocked;
    [SerializeField, FoldoutGroup("Import")] private int startDiceShop;
    [SerializeField, FoldoutGroup("Import")] private int startToolsShop;
    [SerializeField, FoldoutGroup("Import")] private int startDataShop;
    
    //Factory
    [SerializeField, FoldoutGroup("Factory")] private bool workshopUnlocked;
    [SerializeField, FoldoutGroup("Factory")] private int startDicemaker;
    [SerializeField, FoldoutGroup("Factory")] private int startSpeed;
    [SerializeField, FoldoutGroup("Factory")] private int startEfficiency;
    [SerializeField, FoldoutGroup("Factory")] private int startCritical;
    [SerializeField, FoldoutGroup("Factory")] private int startOverdrive;
    
    //Transformer
    [SerializeField, FoldoutGroup("Transformer")] private bool casinoUnlocked;
    [SerializeField, FoldoutGroup("Transformer")] private int startBets;
    [SerializeField, FoldoutGroup("Transformer")] private int startStakes;
    [SerializeField, FoldoutGroup("Transformer")] private int startOdds;
    [SerializeField, FoldoutGroup("Transformer")] private int startJackpot;
    
    //DiceWorld
    [SerializeField, FoldoutGroup("Technology")] private bool diceworldUnlocked;
    [SerializeField, FoldoutGroup("Technology")] private int startSides;
    [SerializeField, FoldoutGroup("Technology")] private int startAdvantage;
    [SerializeField, FoldoutGroup("Technology")] private int startHighRoller;
    [SerializeField, FoldoutGroup("Technology")] private int startExplosive;
    
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
    
    void Start()
    {
        ResourceManager.instance.InitializeResourceManager();
        
        #region |-------------- INIT AREAS --------------|
        //Init Import
        Debug.Log("|----- START INIT: Import -----|");
        List <int> shopStartSettings  = new List<int>();
        shopStartSettings.Add(startDiceShop);
        shopStartSettings.Add(startToolsShop);
        shopStartSettings.Add(startDataShop);
        shop.InitializeInteractionArea(shopUnlocked, shopStartSettings);
        Debug.Log("|----- FINISH INIT: Import -----|");
        
        //Init Factory
        Debug.Log("|----- START INIT: Factory -----|");
        List <int> workshopStartSettings  = new List<int>();
        workshopStartSettings.Add(startDicemaker);
        workshopStartSettings.Add(startSpeed);
        workshopStartSettings.Add(startEfficiency);
        workshopStartSettings.Add(startCritical);
        workshopStartSettings.Add(startOverdrive);
        workshopStartSettings.Add(startOverdrive);
        workshopStartSettings.Add(startOverdrive);
        workshop.InitializeInteractionArea(workshopUnlocked, workshopStartSettings);
        Debug.Log("|----- FINISH INIT: Factory -----|");
        
        //Init Transformer
        /*
        Debug.Log("|----- START INIT: Transformer -----|");
        List <int> casinoStartSettings  = new List<int>();
        casinoStartSettings.Add(startBets);
        casinoStartSettings.Add(startStakes);
        casinoStartSettings.Add(startOdds);
        casinoStartSettings.Add(startJackpot);
        casino.InitializeInteractionArea(casinoUnlocked, casinoStartSettings);
        Debug.Log("|----- FINISH INIT: Transformer -----|");
        */
        
        //Init Technology
        Debug.Log("|----- START INIT: Technology -----|");
        List <int> diceworldStartSettings  = new List<int>();
        diceworldStartSettings.Add(startSides);
        diceworldStartSettings.Add(startAdvantage);
        diceworldStartSettings.Add(startHighRoller);
        diceworldStartSettings.Add(startExplosive);
        diceworld.InitializeInteractionArea(diceworldUnlocked, diceworldStartSettings);
        Debug.Log("|----- FINISH INIT: Technology -----|");
        
        //Init Stockmarket
        Debug.Log("|----- START INIT: Stockmarket -----|");
        List <int> stockmarketStartSettings  = new List<int>();
        stockmarketStartSettings.Add(startGrowthStock);
        stockmarketStartSettings.Add(startMarketCap);
        stockmarket.InitializeInteractionArea(stockmarketUnlocked, stockmarketStartSettings);
        Debug.Log("|----- FINISH INIT: Stockmarket -----|");
      
        /*
        //Init Lab
        Debug.Log("|----- START INIT: Lab -----|");
        lab.InitializeLab(startResearchProgress);
        Debug.Log("|----- FINISH INIT: Lab -----|");
        
         */
        
        
        #endregion
       
        
        
        CPU.instance.ChangeDiceRolledTotal(startDiceRollTotal);
        
        if (startPips > 0) CPU.instance.ChangeResource(Resource.Pips, startPips);
        if (startDice > 0) CPU.instance.ChangeResource(Resource.Dice, startDice);
        if (startTools > 0) CPU.instance.ChangeResource(Resource.Material, startTools);
        if (startLuck > 0) CPU.instance.ChangeResource(Resource.Luck, startLuck);
        if (startMDice > 0) CPU.instance.ChangeResource(Resource.mDice, startMDice);
        if (startData > 0) CPU.instance.ChangeResource(Resource.Data, startData);
        
        //UnlockDice
        CPU.instance.UnlockDice();
        shop.UnlockInteractor(0);
    }
    
    
}
