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
    [Header("DiceManager")]
    [SerializeField] private int diceCostBase;
    [SerializeField] private int diceMax;
    [Header("Material")]
    [SerializeField] private int toolsCostBase;
    [SerializeField] private float toolsCostMult;
    [SerializeField] private int toolsMax;
    [Header("Data")]
    [SerializeField] private int dataCostBase;
    [SerializeField] private float dataCostMult;
    [SerializeField] private int dataMax;
    [Header("Progress")]
    [SerializeField] private int diceCostIncrease1;
    [SerializeField] private int diceCostIncrease2;
    [SerializeField] private int diceCostIncrease3;
    [SerializeField] private int diceCostIncrease4;


    private int diceCostLevel = 1;
    
    #region |-------------- INIT --------------|

    protected override void InitSubClass()
    {
      
    }
    
    protected override List<int> GetCostsBase()
    {
        List<int> costs = new List<int>();
        
        costs.Add(diceCostBase);
        costs.Add(toolsCostBase);
        costs.Add(dataCostBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(1);
        costs.Add(toolsCostMult);
        costs.Add(dataCostMult);
        
        return costs;
    }
    
    protected override List<int> GetValueMax()
    {
        List<int> max = new List<int>();
        
        max.Add(diceMax);
        max.Add(toolsMax);
        max.Add(dataMax);
        
        return max;
    }
    
   
    #endregion
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|


    protected override void RunInteraction(int index)
    {
        
        switch (index)
        {
            case 0: //Buy DiceManager
                CPU.instance.ChangeResource(Resource.Dice,1);
                CheckProgress();
                break;
            
            case 1: //Buy Material
                CPU.instance.ChangeResource(Resource.Material,1);
                break;
            
            case 2: //Buy Data
                CPU.instance.ChangeResource(Resource.Data,1);
                break;
            
        }
    }

    protected override void CheckProgress()
    {
        double totalDicePurchased = 
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Import, 0) + CPU.instance.GetAreaInteractorCount(InteractionAreaType.Import, 1) * 100;
        
        if (totalDicePurchased >= diceCostIncrease1 && diceCostLevel == 1)
                    
        {
            diceCostLevel = 2;
            ShopCostUpdateDice(diceCostLevel);
        }

        else if (totalDicePurchased >= diceCostIncrease2 && diceCostLevel == 2)
        {
            diceCostLevel = 3;
            ShopCostUpdateDice(diceCostLevel);
        }

        else if (totalDicePurchased >= diceCostIncrease3 && diceCostLevel == 3)
        {
            diceCostLevel = 4;
            ShopCostUpdateDice(diceCostLevel);
        }
        
        else if (totalDicePurchased >= diceCostIncrease4 && diceCostLevel == 4)
        {
            diceCostLevel = 5;
            ShopCostUpdateDice(diceCostLevel);
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
