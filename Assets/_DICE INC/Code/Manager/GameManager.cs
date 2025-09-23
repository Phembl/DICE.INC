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
    [SerializeField, FoldoutGroup("References")] private InteractionArea import;
    [SerializeField, FoldoutGroup("References")] private InteractionArea factory;
    [SerializeField, FoldoutGroup("References")] private InteractionArea transformer;
    [SerializeField, FoldoutGroup("References")] private InteractionArea technology;
    [SerializeField, FoldoutGroup("References")] private InteractionArea stockmarket;
    [SerializeField, FoldoutGroup("References")] private InteractionArea datacenter;
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
    [SerializeField, FoldoutGroup("Settings")] private int startMaterial;
    [SerializeField, FoldoutGroup("Settings")] private int startLuck;
    [SerializeField, FoldoutGroup("Settings")] private int startMDice;
    [SerializeField, FoldoutGroup("Settings")] private int startData;
    
    #endregion
    
    #region |-------------- AREA SETTINGS --------------|
    //Import
    [SerializeField, FoldoutGroup("Import")] private bool importUnlocked;
    [SerializeField, FoldoutGroup("Import")] private int startDiceImport;
    [SerializeField, FoldoutGroup("Import")] private int startMaterialImport;
    [SerializeField, FoldoutGroup("Import")] private int startDataImport;
    
    //Factory
    [SerializeField, FoldoutGroup("Factory")] private bool factoryUnlocked;
    [SerializeField, FoldoutGroup("Factory")] private int startDicemaker;
    [SerializeField, FoldoutGroup("Factory")] private int startSpeed;
    [SerializeField, FoldoutGroup("Factory")] private int startEfficiency;
    [SerializeField, FoldoutGroup("Factory")] private int startCritical;
    [SerializeField, FoldoutGroup("Factory")] private int startOverdrive;
    [SerializeField, FoldoutGroup("Factory")] private int startFactory6;
    [SerializeField, FoldoutGroup("Factory")] private int startFactory7;
    
    //Transformer
    [SerializeField, FoldoutGroup("Transformer")] private bool transformerUnlocked;
    [SerializeField, FoldoutGroup("Transformer")] private int startTransformer1;
    [SerializeField, FoldoutGroup("Transformer")] private int startTransformer2;
    
    //DiceWorld
    [SerializeField, FoldoutGroup("Technology")] private bool technologyUnlocked;
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
    [SerializeField, FoldoutGroup("Datacenter")] private int startDatacenter1;
    [SerializeField, FoldoutGroup("Datacenter")] private int startDatacenter2;
    [SerializeField, FoldoutGroup("Datacenter")] private int startDatacenter3;
    
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
        List <int> importStartSettings  = new List<int>();
        importStartSettings.Add(startDiceImport);
        importStartSettings.Add(startMaterialImport);
        importStartSettings.Add(startDataImport);
        import.InitializeInteractionArea(importUnlocked, importStartSettings);
        Debug.Log("|----- FINISH INIT: Import -----|");
        
        //Init Factory
        Debug.Log("|----- START INIT: Factory -----|");
        List <int> factoryStartSettings  = new List<int>();
        factoryStartSettings.Add(startDicemaker);
        factoryStartSettings.Add(startSpeed);
        factoryStartSettings.Add(startEfficiency);
        factoryStartSettings.Add(startCritical);
        factoryStartSettings.Add(startOverdrive);
        factoryStartSettings.Add(startFactory6);
        factoryStartSettings.Add(startFactory7);
        factory.InitializeInteractionArea(factoryUnlocked, factoryStartSettings);
        Debug.Log("|----- FINISH INIT: Factory -----|");
        
        //Init Transformer
        Debug.Log("|----- START INIT: Transformer -----|");
        List <int> transformerStartSettings  = new List<int>();
        transformerStartSettings.Add(startTransformer1);
        transformerStartSettings.Add(startTransformer2);
        transformer.InitializeInteractionArea(transformerUnlocked, transformerStartSettings);
        Debug.Log("|----- FINISH INIT: Transformer -----|");
        
        //Init Technology
        Debug.Log("|----- START INIT: Technology -----|");
        List <int> technologyStartSettings  = new List<int>();
        technologyStartSettings.Add(startSides);
        technologyStartSettings.Add(startAdvantage);
        technologyStartSettings.Add(startHighRoller);
        technologyStartSettings.Add(startExplosive);
        technology.InitializeInteractionArea(technologyUnlocked, technologyStartSettings);
        Debug.Log("|----- FINISH INIT: Technology -----|");
        
        //Init Stockmarket
        Debug.Log("|----- START INIT: Stockmarket -----|");
        List <int> stockmarketStartSettings  = new List<int>();
        stockmarketStartSettings.Add(startGrowthStock);
        stockmarketStartSettings.Add(startMarketCap);
        stockmarket.InitializeInteractionArea(stockmarketUnlocked, stockmarketStartSettings);
        Debug.Log("|----- FINISH INIT: Stockmarket -----|");
        
        //Init Datacenter
        Debug.Log("|----- START INIT: Data Center -----|");
        List <int> datacenterStartSettings  = new List<int>();
        datacenterStartSettings.Add(startDatacenter1);
        datacenterStartSettings.Add(startDatacenter2); 
        datacenterStartSettings.Add(startDatacenter3);
        stockmarket.InitializeInteractionArea(datacenterUnlocked, datacenterStartSettings);
        Debug.Log("|----- FINISH INIT: Data Center -----|");
      
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
        if (startMaterial > 0) CPU.instance.ChangeResource(Resource.Material, startMaterial);
        if (startLuck > 0) CPU.instance.ChangeResource(Resource.Luck, startLuck);
        if (startMDice > 0) CPU.instance.ChangeResource(Resource.mDice, startMDice);
        if (startData > 0) CPU.instance.ChangeResource(Resource.Data, startData);
        
        //UnlockDice
        CPU.instance.UnlockDice();
        import.UnlockInteractor(0);
    }
    
    
}
