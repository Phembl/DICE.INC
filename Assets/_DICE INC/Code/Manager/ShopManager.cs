/*
using System;
using System.Collections;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    
    [TitleGroup("References")] 
    [SerializeField] private Transform shopHolder;

    [TitleGroup("Settings")] 
    [Header("Dice")]
    [SerializeField] private int costBaseDice;
    [SerializeField] private int costIncreaseDicePurchase1;
    [SerializeField] private int costIncreaseDicePurchase2;
    [SerializeField] private int costIncreaseDicePurchase3;
    [SerializeField] private int costIncreaseDicePurchase4;
    [Header("Tools")]
    [SerializeField] private int costBaseTools;
    [SerializeField] private float costMultTools;
    [Header("Data")]
    [SerializeField] private int costBaseData;
    [SerializeField] private float costMultData;
    
    
    private int costCurrentDice;
    private int costCurrentTools;
    private int costCurrentData;

    
    public static ShopManager instance;
   
    
    private void Awake()
    {
        if  (instance == null) instance = this;
       
    }
    
    public void InitializeShop()
    {
        costCurrentDice = costBaseDice;
        costCurrentTools = costBaseTools;
        costCurrentData = costBaseData;
        
        shopHolder.GetChild(0).gameObject.GetComponent<Interactor>().InitializeInteractor(costCurrentDice);
        shopHolder.GetChild(1).gameObject.GetComponent<Interactor>().InitializeInteractor(costCurrentDice * 100);
        shopHolder.GetChild(2).gameObject.GetComponent<Interactor>().InitializeInteractor(costCurrentTools);
        shopHolder.GetChild(3).gameObject.GetComponent<Interactor>().InitializeInteractor(costCurrentTools * 100);
        shopHolder.GetChild(4).gameObject.GetComponent<Interactor>().InitializeInteractor(costCurrentData);
        shopHolder.GetChild(5).gameObject.GetComponent<Interactor>().InitializeInteractor(costCurrentData * 100);
    }
    
    
    public void UnlockShopItem(ShopItem _shopItem)
    {
        
        if (shopHolder.GetChild((int)_shopItem).gameObject == null) Debug.LogWarning("A shop item tries to be unlocked which does not exist");
        else
        {
            Debug.Log($"Shop: Unlocking shop item: {_shopItem.ToString()}");
            shopHolder.GetChild((int)_shopItem).gameObject.GetComponent<Interactor>().Unlock();
        }
        
    }
    
    //Called by Shop Interactors
    public void ShopInteraction(int _interactorIndex)
    {
        CPU.instance.AddShopPurchase(_interactorIndex);
        
        switch (_interactorIndex)
        {
            case 0: //Buy Dice
                CPU.instance.ChangeResource(Resource.Dice,1);
                CPU.instance.ChangeResource(Resource.Pips,-costCurrentDice);
                GameManager.instance.diceOverall++;
                UpdatePrice(ShopItem.Dice);
                break;
            
            case 1: //Buy 100 Dice
                CPU.instance.ChangeResource(Resource.Dice,100);
                CPU.instance.ChangeResource(Resource.Pips,-(costCurrentDice * 100));
                GameManager.instance.diceOverall += 100;
                UpdatePrice(ShopItem.Dice);
                break;
            
            case 2: //Buy Tools
                CPU.instance.ChangeResource(Resource.Tools,1);
                CPU.instance.ChangeResource(Resource.Pips,-costCurrentTools);
                GameManager.instance.toolsOverall++;
                UpdatePrice(ShopItem.Tools);
                break;
            
            case 3: //Buy 100 Tools
                CPU.instance.ChangeResource(Resource.Tools,100);
                CPU.instance.ChangeResource(Resource.Pips,-(costCurrentTools * 100));
                GameManager.instance.toolsOverall += 100;
                UpdatePrice(ShopItem.Tools);
                break;
            
            case 4:
                CPU.instance.ChangeResource(Resource.Data,1);
                CPU.instance.ChangeResource(Resource.Pips,-costCurrentData);
                GameManager.instance.dataOverall++;
                UpdatePrice(ShopItem.Data);
                break;
            
            case 5:
                CPU.instance.ChangeResource(Resource.Data,100);
                CPU.instance.ChangeResource(Resource.Pips,-(costCurrentData * 100));
                GameManager.instance.dataOverall += 100;
                UpdatePrice(ShopItem.Data);
                break;
                
        }
    }
    
    private void UpdatePrice(ShopItem shopItemType)
    {
        switch (shopItemType)
        {
            case ShopItem.Dice:
                double totalDicePurchased = CPU.instance.GetShopDicePurchased();
                
                if (totalDicePurchased == costIncreaseDicePurchase1)
                    
                {
                    costCurrentDice = 2;
                    shopHolder.GetChild(0).gameObject.GetComponent<Interactor>().UpdatePrice(costCurrentDice);
                }

                else if (totalDicePurchased == costIncreaseDicePurchase2)
                {
                    costCurrentDice = 3;
                    shopHolder.GetChild(0).gameObject.GetComponent<Interactor>().UpdatePrice(costCurrentDice);
                }

                else if (totalDicePurchased == costIncreaseDicePurchase3)
                {
                    costCurrentDice = 4;
                    shopHolder.GetChild(0).gameObject.GetComponent<Interactor>().UpdatePrice(costCurrentDice);
                }
                else if (totalDicePurchased == costIncreaseDicePurchase4)
                {
                    costCurrentDice = 5;
                    shopHolder.GetChild(0).gameObject.GetComponent<Interactor>().UpdatePrice(costCurrentDice);
                }
                
                break;
            
            case ShopItem.Tools:
                costCurrentTools = (int)(costCurrentTools * costMultTools);
                shopHolder.GetChild((int)shopItemType).gameObject.GetComponent<Interactor>().UpdatePrice(costCurrentTools);
                break;
            
            case ShopItem.Data:
                costCurrentData = (int)(costCurrentData * costMultData);
                shopHolder.GetChild((int)shopItemType).gameObject.GetComponent<Interactor>().UpdatePrice(costCurrentData);
                break;
            
        }

    }
    
  


}
*/
