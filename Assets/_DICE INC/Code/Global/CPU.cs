using System;
using System.Collections.Generic;
using UnityEngine;
using DICEINC.Global;

public class CPU : MonoBehaviour
{
    [SerializeField] private bool printLog;
    
    #region |-------------- RESOURCE TRACKING --------------|
    
    private double pipsCurrent = 1;
    private double diceCurrent;
    private double materialCurrent;
    private double luckCurrent;
    private double mDiceCurrent;
    private double dataCurrent;
    
    private double pipsTotal;
    private double diceTotal;
    private double materialTotal;
    private double luckTotal;
    private double mDiceTotal;
    private double dataTotal;

    private bool diceUnlocked;
    private bool materialUnlocked;
    private bool luckUnlocked;
    private bool mDiceUnlocked; 
    private bool dataUnlocked;
    
    public void UnlockDice() => diceUnlocked = true;
    public void UnlockTools() => materialUnlocked = true;
    public void UnlockLuck() => luckUnlocked = true;
    public void UnlockMDice() => mDiceUnlocked = true;
    public void UnlockData() => dataUnlocked = true;
    
    public double GetPips() => pipsCurrent;
    public double GetDice() => diceCurrent;
    public double GetTools() => materialCurrent;
    public double GetLuck() => luckCurrent;
    public double GetMDice() => mDiceCurrent;
    public double GetData() => dataCurrent;
    
    public double GetPipsTotal() => pipsTotal;
    public double GetDiceTotal() => diceTotal;
    public double GetToolsTotal() => materialTotal;
    public double GetLuckTotal() => luckTotal;
    public double GetMDiceTotal() => mDiceTotal;
    public double GetDataTotal() => dataTotal;
    
    public static event Action OnPipsChanged;
    public static event Action OnDiceChanged;
    public static event Action OnMaterialChanged;
    public static event Action OnLuckChanged;
    public static event Action OnMDiceChanged;
    public static event Action OnDataChanged;

    public void ChangeResource(Resource resource, double change)
    {
        switch (resource)
        {
            case Resource.Pips:
                pipsCurrent += change;
                if (change > 0) pipsTotal += change;
                if (pipsCurrent < 0) pipsCurrent = 0;
                OnPipsChanged?.Invoke();
                if (printLog) Debug.Log($"Pips changed to {pipsCurrent}");
                break;
            
            case Resource.Dice:
                diceCurrent += change;
                if (change > 0) diceTotal += change;
                if (diceCurrent < 0) diceCurrent = 0;
                OnDiceChanged?.Invoke();
                if (printLog) Debug.Log($"Dice changed to {diceCurrent}");
                break;
            
            case Resource.Material:
                materialCurrent += change;
                if (change > 0) materialTotal += change;
                if (materialCurrent < 0) materialCurrent = 0;
                OnMaterialChanged?.Invoke();
                if (printLog) Debug.Log($"Material changed to {materialCurrent}");
                break;
            
            case Resource.Luck:
                luckCurrent += change;
                if (change > 0) luckTotal += change;
                if (luckCurrent < 0) luckCurrent = 0;
                OnLuckChanged?.Invoke();
                if (printLog) Debug.Log($"Luck changed to {luckCurrent}");
                break;
            
            case Resource.mDice:
                mDiceCurrent += change;
                if (change > 0) mDiceTotal += change;
                if (mDiceCurrent < 0) mDiceCurrent = 0;
                OnMDiceChanged?.Invoke();
                if (printLog) Debug.Log($"MDice changed to {mDiceCurrent}");
                break;
            
            case Resource.Data:
                dataCurrent += change;
                if (change > 0) dataTotal += change;
                if (dataCurrent < 0) dataCurrent = 0;
                OnDataChanged?.Invoke();
                if (printLog) Debug.Log($"Data changed to {dataCurrent}");
                break;
        }
        
    }
    
    public bool GetDiceUnlockState() => diceUnlocked;
    public bool GetMaterialUnlockState() => materialUnlocked;
    public bool GetLuckUnlockState() => luckUnlocked;
    public bool GetMDiceUnlockState() => mDiceUnlocked;
    public bool GetDataUnlockState() => dataUnlocked;
    
    #endregion
    
    #region |-------------- AREA TRACKING --------------|

    private bool[] areaUnlockStates = new bool[]{false, true, true, false, false, false, false, false, false, false};
    public void SetAreaUnlockState(InteractionAreaType area)
    {
        areaUnlockStates[(int)area] = true;
    }
    
    #endregion
    #region |-------------- INTERACTION TRACKING --------------|
    
    private List<int> importInteractorCount = new List<int>();
    private List<bool> importInteractorUnlockStates = new List<bool>();
    private List<int> factoryInteractorCount = new List<int>();
    private List<bool> factoryInteractorUnlockStates = new List<bool>();
    private List<int> transformerInteractorCount = new List<int>();
    private List<bool> transfomerInteractorUnlockStates = new List<bool>();
    private List<int> technologyInteractorCount = new List<int>();
    private List<bool> technologyInteractorUnlockStates = new List<bool>();
    private List<int> stockmarketInteractorCount = new List<int>();
    private List<bool> stockmarketInteractorUnlockStates = new List<bool>();
    private List<int> datacenterInteractorCount = new List<int>();
    private List<bool> datacenterInteractorUnlockStates = new List<bool>();

    public void InitInteractorCountList(InteractionAreaType interactionAreaType, int count)
    {
        switch (interactionAreaType)
        {
            case InteractionAreaType.Import:
                importInteractorCount.Add(count);
                importInteractorUnlockStates.Add(false);
                break;
            
            case InteractionAreaType.Factory:
                factoryInteractorCount.Add(count);
                factoryInteractorUnlockStates.Add(false);
                break;
            
            case InteractionAreaType.Transformer:
                transformerInteractorCount.Add(count);
                transfomerInteractorUnlockStates.Add(false);
                break;
            
            case InteractionAreaType.Technology:
                technologyInteractorCount.Add(count);
                technologyInteractorUnlockStates.Add(false);
                break;
            
            case InteractionAreaType.Stockmarket:
                stockmarketInteractorCount.Add(count);
                stockmarketInteractorUnlockStates.Add(false);
                break;
            
            case InteractionAreaType.Datacenter:
                datacenterInteractorCount.Add(count);
                datacenterInteractorUnlockStates.Add(false);
                break;
                
        }
    }
    
    public void IncreaseAreaInteractorCount(InteractionAreaType interactionAreaType, int index, int increase = 1)
    {
        switch (interactionAreaType)
        {
            case InteractionAreaType.Import:
                importInteractorCount[index] += increase;
                break;
            
            case InteractionAreaType.Factory:
                factoryInteractorCount[index] += increase;
                break;
            
            case InteractionAreaType.Transformer:
                transformerInteractorCount[index] += increase;
                break;
            
            case InteractionAreaType.Technology:
                technologyInteractorCount[index] += increase;
                break;
            
            case InteractionAreaType.Stockmarket:
                stockmarketInteractorCount[index] += increase;
                break;
            
            case InteractionAreaType.Datacenter:
                datacenterInteractorCount[index] += increase;
                break;
            
            default:
                Debug.LogWarning(interactionAreaType + " is undefined.");
                break;
        }
    }

    public int GetAreaInteractorCount(InteractionAreaType interactionAreaType, int index)
    {
        int count = -1;
        
        switch (interactionAreaType)
        {
            case InteractionAreaType.Import:
                count = importInteractorCount[index];
                break;
            
            case InteractionAreaType.Factory:
                count = factoryInteractorCount[index];
                break;
            
            case InteractionAreaType.Transformer:
                count = transformerInteractorCount[index];
                break;
            
            case InteractionAreaType.Technology:
                count = technologyInteractorCount[index];
                break;
            
            case InteractionAreaType.Stockmarket:
                count = stockmarketInteractorCount[index];
                break;
            
            case InteractionAreaType.Datacenter:
                count = datacenterInteractorCount[index];
                break;
            
            default:
                Debug.LogWarning(interactionAreaType + " is undefined.");
                break;
            
        }
        
        return count;
    }

    public void UnlockInteractor(InteractionAreaType interactionAreaType, int index)
    {
        switch (interactionAreaType)
        {
            case InteractionAreaType.Import:
                importInteractorUnlockStates[index] = true;
                break;
            
            case InteractionAreaType.Factory:
                factoryInteractorUnlockStates[index] = true;
                break;
            
            case InteractionAreaType.Transformer:
                transfomerInteractorUnlockStates[index] = true;
                break;
            
            case InteractionAreaType.Technology:
                technologyInteractorUnlockStates[index] = true;
                break;
            
            case InteractionAreaType.Stockmarket:
                stockmarketInteractorUnlockStates[index] = true;
                break;
            
            case InteractionAreaType.Datacenter:
                datacenterInteractorUnlockStates[index] = true;
                break;
            
            default:
                Debug.LogWarning(interactionAreaType + " is undefined.");
                break;
        }
    }

    public bool GetInteractorUnlockState(InteractionAreaType interactionAreaType, int index)
    {
        bool unlockState = false;
        
        switch (interactionAreaType)
        {
            case InteractionAreaType.Import:
                unlockState = importInteractorUnlockStates[index];
                break;
            
            case InteractionAreaType.Factory:
                unlockState = factoryInteractorUnlockStates[index];
                break;
            
            case InteractionAreaType.Transformer:
                unlockState = transfomerInteractorUnlockStates[index];
                break;
            
            case InteractionAreaType.Technology:
                unlockState = technologyInteractorUnlockStates[index];
                break;
            
            case InteractionAreaType.Stockmarket:
                unlockState = stockmarketInteractorUnlockStates[index];
                break;
            
            case InteractionAreaType.Datacenter:
                unlockState = datacenterInteractorUnlockStates[index];
                break;
            
            default:
                Debug.LogWarning(interactionAreaType + " is undefined.");
                break;
                
        }
        
        return unlockState;
    }
    
    
    #endregion
    
    #region |-------------- STATISTICS TRACKING --------------|
    
    //Statistics Tracking
    //Dice Rolled
    private int diceRolledTotal;
    
    public int GetDiceRolledTotal() => diceRolledTotal;
    
    public void ChangeDiceRolledTotal(int change)
    {
        diceRolledTotal += change;
        OnDiceRollTotalChanged?.Invoke();
    }
    
    public static event Action OnDiceRollTotalChanged;
    
    //Dice Roll StockValue (unlocked through StockMarket)

    private float diceRollStockValue = 1;
    
    public float GetDiceRollStockValue() => diceRollStockValue;
    
    public void ChangeDiceRollStockValue(float currentValue)
    {
        diceRollStockValue = currentValue;
        OnDiceRollStockValueChanged?.Invoke();
    }
    
    public static event Action OnDiceRollStockValueChanged;
    

    #endregion
    
    
    public static CPU instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
        
    }

  
}
