using System.Collections.Generic;
using DG.Tweening;
using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Shop : InteractionArea
{
    
    [ReadOnly] public InteractionAreaType thisInteractionAreaType = InteractionAreaType.Shop;
    protected override InteractionAreaType GetInteractionAreaType() => thisInteractionAreaType;
    
    [TitleGroup("Settings")] 
    [Header("Dice")]
    [SerializeField] private int costDiceBase;
    [Header("Tools")]
    [SerializeField] private int costToolsBase;
    [SerializeField] private float costToolsMultiplier;
    [Header("Data")]
    [SerializeField] private int costDataBase;
    [SerializeField] private float costDataMultiplier;
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
        
        costs.Add(costDiceBase);
        costs.Add(costToolsBase);
        costs.Add(costDataBase);
        
        return costs;
    }
    
    protected override List<float> GetCostsMult()
    {
        List<float> costs = new List<float>();
        
        costs.Add(1);
        costs.Add(costToolsMultiplier);
        costs.Add(costDataMultiplier);
        
        return costs;
    }
    
   
    #endregion
    
    #region |-------------- INDIVIDUAL FUNCTIONS --------------|


    protected override void RunInteraction(int index)
    {
        
        switch (index)
        {
            case 0: //Buy Dice
                CPU.instance.ChangeResource(Resource.Dice,1);
                CheckProgressDice();
                break;
            
            case 1: //Buy Tools
                CPU.instance.ChangeResource(Resource.Tools,1);
                break;
            
            case 2: //Buy Data
                CPU.instance.ChangeResource(Resource.Data,1);
                break;
            
        }
    }

    void CheckProgressDice()
    {
        double totalDicePurchased = 
            CPU.instance.GetAreaInteractorCount(InteractionAreaType.Shop, 0) + CPU.instance.GetAreaInteractorCount(InteractionAreaType.Shop, 1) * 100;
        
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
}
