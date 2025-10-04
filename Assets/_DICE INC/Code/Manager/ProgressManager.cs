using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [TitleGroup("ProgressManager")] [SerializeField]
    private bool printLog;

    [Header("References")] [SerializeField]
    private Import import;

    [SerializeField] private InteractionArea factory;
    [SerializeField] private InteractionArea transformer;
    [SerializeField] private InteractionArea technology;
    [SerializeField] private InteractionArea stockmarket;
    [SerializeField] private InteractionArea datacenter;


    public static ProgressManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
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
}

    #endregion
    
   
    
   
