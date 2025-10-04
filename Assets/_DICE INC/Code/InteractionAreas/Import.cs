using System.Collections.Generic;
using DG.Tweening;
using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Import : InteractionArea
{
    [TitleGroup("References")] 
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Import;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    
    [TitleGroup("Import")] 
    [Header("Dice")]
    [SerializeField] private int diceCostBase;
    //[SerializeField] private float diceCostIncreaseFactor;
    [SerializeField] private int diceCostIncreasePurchase;
    [SerializeField] private float diceCostIncreasePurchaseFactor;
    [Space]
    [ShowInInspector, ReadOnly] private double diceCostCurrent;
    [ShowInInspector, ReadOnly] private double diceCostIncreaseNext;
    [ShowInInspector, ReadOnly] private int diceCostLevel;
    
    [Header("Material")]
    [SerializeField] private int materialCostBase;
    [SerializeField] private float materialCostIncreaseFactor;
    [SerializeField] private int materialCostIncreasePurchase;
    [SerializeField] private float materialCostIncreasePurchaseFactor;
    [Space]
    [ShowInInspector, ReadOnly] private double materialCostCurrent;
    [ShowInInspector, ReadOnly] private double materialCostIncreaseNext;
    [ShowInInspector, ReadOnly] private int materialCostLevel;
    
    [Header("Data")]
    [SerializeField] private int dataCostBase;
    [SerializeField] private float dataCostIncreaseFactor;
    [SerializeField] private int dataCostIncreasePurchase;
    [SerializeField] private float dataCostIncreasePurchaseFactor;
    [Space]
    [ShowInInspector, ReadOnly] private double dataCostCurrent;
    [ShowInInspector, ReadOnly] private double dataCostIncreaseNext;
    [ShowInInspector, ReadOnly] private int dataCostLevel;
    
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
      CheckDiceCost();
      CheckMaterialCost();
      CheckDataCost();
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(diceCostBase);
        costs.Add(materialCostBase);
        costs.Add(dataCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(1);
        costs.Add(1);
        costs.Add(1);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(-1);
        max.Add(-1);
        max.Add(-1);
        
        return max;
    }
    
   
    #endregion
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|


    protected override void RunInteraction(int index)
    {
        
        switch (index)
        {
            case 0: //Buy Dice
                CPU.instance.PurchaseResource(Resource.Dice);
                CheckDiceCost();
                break;
            
            case 1: //Buy Material
                CPU.instance.PurchaseResource(Resource.Material);
                CheckMaterialCost();
                break;
            
            case 2: //Buy Data
                CPU.instance.PurchaseResource(Resource.Data);
                CheckDataCost();
                break;
            
        }
        
    }

  
    protected override void CheckProgress()
    {
        
    }

    void CheckDiceCost()
    {
        double dicePurchased = CPU.instance.GetDicePurchased();

        if (dicePurchased >= diceCostIncreaseNext)
        {
            if (printLog) Debug.Log($"Import: Updating Dice Cost. Current cost: {diceCostCurrent}");
            
            if (diceCostLevel == 0)
            {
                diceCostCurrent = 1;
                diceCostIncreaseNext = diceCostIncreasePurchase;
            }
            else
            {
                //Dice Cost is always incremented by 1
                diceCostCurrent = diceCostBase + (1 * diceCostLevel);
                diceCostIncreaseNext = diceCostIncreasePurchase * (diceCostIncreasePurchaseFactor * diceCostLevel);
            }
            
            ImportCostUpdate(Resource.Dice, diceCostCurrent);
            diceCostLevel++;
           
            
            if (printLog) Debug.Log($"Import: Updating Dice Cost. New cost: {diceCostCurrent}");
        }
    }
    
    void CheckMaterialCost()
    {
        double materialPurchased = CPU.instance.GetMaterialPurchased();

        if (materialPurchased >= materialCostIncreaseNext)
        {
            if (printLog) Debug.Log($"Import: Updating Material Cost. Current cost: {materialCostCurrent}");
            
            if (materialCostLevel == 0)
            {
                materialCostCurrent = materialCostBase;
                materialCostIncreaseNext = materialCostIncreasePurchase;
            }
            else
            {
                materialCostCurrent = materialCostBase * (materialCostIncreaseFactor * materialCostLevel);
                materialCostIncreaseNext = materialCostIncreasePurchase * (materialCostIncreasePurchaseFactor * materialCostLevel);
            }
            
            ImportCostUpdate(Resource.Material, materialCostCurrent);
            materialCostLevel++;
            
            if (printLog) Debug.Log($"Import: Updating Material Cost. New cost: {materialCostCurrent}");
        }
    }
    
    void CheckDataCost()
    {
        double dataPurchased = CPU.instance.GetDataPurchased();

        if (dataPurchased >= dataCostIncreaseNext)
        {
            if (printLog) Debug.Log($"Import: Updating Data Cost. Current cost: {dataCostCurrent}");
            
            if (dataCostLevel == 0)
            {
                dataCostCurrent = dataCostBase;
                dataCostIncreaseNext = dataCostIncreasePurchase;
            }
            else
            {
                dataCostCurrent = dataCostBase * (dataCostIncreaseFactor * dataCostLevel);
                dataCostIncreaseNext = dataCostIncreasePurchase * (dataCostIncreasePurchaseFactor * dataCostLevel);
            }
            
            ImportCostUpdate(Resource.Data, dataCostCurrent);
            dataCostLevel++;
            
            if (printLog) Debug.Log($"Import: Updating Data Cost. New cost: {dataCostCurrent}");
        }
    }

  

    #endregion
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = "Import";
        data.areaDescription = "The import,";
        
        return data;
    }
    
    #endregion
}
