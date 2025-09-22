using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [TitleGroup("ProgressManager")] 
    [SerializeField] private bool printLog;
    [Header("References")]
    [SerializeField] private Import import;
    [SerializeField] private InteractionArea workshop;
    [SerializeField] private InteractionArea casino;
    [SerializeField] private InteractionArea diceworld;
    
    [SerializeField, FoldoutGroup("Factory")] private int lvlUnlockSpeed;
    [SerializeField, FoldoutGroup("Factory")] private int lvlUnlockEfficiency;
    [SerializeField, FoldoutGroup("Factory")] private int lvlUnlockCritical;
    [SerializeField, FoldoutGroup("Factory")] private int lvlUnlockOverdrive;
    private bool speedUnlocked;
    private bool efficiencyUnlocked;
    private bool criticalUnlocked;
    private bool overdriveUnlocked;
    
    public static ProgressManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    public void ResearchProgress(int researchIndex)
    {
        switch (researchIndex)
        {
            case 0: //Unlock Material & Factory
                workshop.UnlockArea();
                ResourceManager.instance.UnlockResource(Resource.Material);
                import.UnlockInteractor(1);
                break;
            
            case 1:
                casino.UnlockArea();
                break;
            
            case 2:
                diceworld.UnlockArea();
                ResourceManager.instance.UnlockResource(Resource.mDice);
                break;
        }
    }

    public void AreaProgress(InteractionAreaType area, int level)
    {
        if (printLog) Debug.Log($"Checking Progress for {area.ToString()} with level {level}.");
        
        switch (area)
        {
            case InteractionAreaType.Factory:
                if (level >= lvlUnlockSpeed && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 1)) 
                    workshop.UnlockInteractor(1);
                if (level >= lvlUnlockEfficiency && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 2)) 
                    workshop.UnlockInteractor(2);
                if (level >= lvlUnlockCritical && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 3)) 
                    workshop.UnlockInteractor(3);
                if (level >= lvlUnlockOverdrive && !CPU.instance.GetInteractorUnlockState(InteractionAreaType.Factory, 4)) 
                    workshop.UnlockInteractor(4);
                break;
        }
    }
}
