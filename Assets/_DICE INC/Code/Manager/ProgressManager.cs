using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [TitleGroup("ProgressManager")] 
    [SerializeField] private bool printLog;
    [Header("References")]
    [SerializeField] private Import import;
    [SerializeField] private InteractionArea factory;
    [SerializeField] private InteractionArea transformer;
    [SerializeField] private InteractionArea technology;
    [SerializeField] private InteractionArea stockmarket;
    [SerializeField] private InteractionArea datacenter;
    
    [SerializeField, FoldoutGroup("Resources"), Header("Dice")] private int diceCostIncrease1;
    [SerializeField, FoldoutGroup("Resources")] private int diceCostIncrease2;
    [SerializeField, FoldoutGroup("Resources")] private int diceCostIncrease3;
    [SerializeField, FoldoutGroup("Resources")] private int diceCostIncrease4;
    [SerializeField, FoldoutGroup("Resources"), Header("Material")] private int materialCostIncrease1;
    [SerializeField, FoldoutGroup("Resources")] private int materialCostIncrease2;
    [SerializeField, FoldoutGroup("Resources")] private int materialCostIncrease3;
    [SerializeField, FoldoutGroup("Resources")] private int materialCostIncrease4;
    [SerializeField, FoldoutGroup("Resources"), Header("Data")] private int dataCostIncrease1;
    [SerializeField, FoldoutGroup("Resources")] private int dataCostIncrease2;
    [SerializeField, FoldoutGroup("Resources")] private int dataCostIncrease3;
    [SerializeField, FoldoutGroup("Resources")] private int dataCostIncrease4;
    
    [SerializeField, FoldoutGroup("Factory")] private int unlockConveyorLvl;
    [SerializeField, FoldoutGroup("Factory")] private int unlockToolsLvl;
    [SerializeField, FoldoutGroup("Factory")] private int unlockSurplusLvl;
    [SerializeField, FoldoutGroup("Factory")] private int unlockOverdriveLvl;
    [SerializeField, FoldoutGroup("Factory")] private int unlockAIWorkersLvl;
    [SerializeField, FoldoutGroup("Factory")] private int unlockMachineLearningLvl;
    
    public static ProgressManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    #region |-------------- RESEARCH PROGRESS --------------|
    public void ResearchProgress(int researchIndex)
    {
        switch (researchIndex)
        {
            case 0: //Unlock Material & Factory
                factory.UnlockArea();
                ResourceManager.instance.UnlockResource(Resource.Material);
                import.UnlockInteractor(1);
                break;
            
            case 1:
                transformer.UnlockArea();
                break;
            
            case 2:
                technology.UnlockArea();
                ResourceManager.instance.UnlockResource(Resource.mDice);
                break;
            
            case 3:
                stockmarket.UnlockArea();
                ResourceManager.instance.UnlockResource(Resource.Data);
                break;
            
            case 4:
                datacenter.UnlockArea();
                break;
        }
    }
    #endregion
    
    #region |-------------- AREA PROGRESS --------------|
    public void AreaProgress(InteractionAreaType area, int level)
    {
        if (printLog) Debug.Log($"Checking Progress for {area.ToString()} with level {level}.");
        
        switch (area)
        {
            case InteractionAreaType.Factory:
                if (level >= unlockConveyorLvl && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 1)) 
                    factory.UnlockInteractor(1);
                if (level >= unlockToolsLvl && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 2)) 
                    factory.UnlockInteractor(2);
                if (level >= unlockSurplusLvl && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 3)) 
                    factory.UnlockInteractor(3);
                if (level >= unlockOverdriveLvl && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4)) 
                    factory.UnlockInteractor(4);
                if (level >= unlockAIWorkersLvl && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 5))
                    factory.UnlockInteractor(5);
                if (level >= unlockMachineLearningLvl && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 6))
                    factory.UnlockInteractor(6);
                break;
        }
    }
    
    #endregion
    
    #region |-------------- RESOURCE COST PROGRESS --------------|

    public void CheckResourceProgress(Resource resource)
    {
        switch (resource)
        {
            case Resource.Dice:
                CheckDiceProgress();
                break;
            
            case Resource.Material:
                break;
            
            case Resource.Data:
                break;
        }
    }

    private void CheckDiceProgress()
    {
        double purchasedDice = CPU.instance.GetDicePurchased();
    }
    
    
    
    #endregion
}
