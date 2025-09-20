using Sirenix.OdinInspector;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [TitleGroup("ProgressManager")] 
    [Header("References")]
    [SerializeField] private Shop shop;
    [SerializeField] private InteractionArea workshop;
    [SerializeField] private InteractionArea casino;
    [SerializeField] private InteractionArea diceworld;
    
    public static ProgressManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    public void ResearchProgress(int researchIndex)
    {
        switch (researchIndex)
        {
            case 0: //Unlock Tools & Workshop
                workshop.UnlockArea();
                shop.UnlockInteractor(1);
                break;
            
            case 1:
                casino.UnlockArea();
                break;
            
            case 2:
                diceworld.UnlockArea();
                break;
        }
    }
}
