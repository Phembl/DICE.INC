using System.Collections;
using System.Collections.Generic;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lab : MonoBehaviour
{
    [TitleGroup("Lab")] 
    [Header("References")]
    [SerializeField] private Transform labMemoryHolder;
    [SerializeField] private GameObject labButton;
    private TMP_Text labButtonTMP;
    [Space] 

    
    [Header("Settings")]
    [SerializeField] private bool printLog;
    [SerializeField] private int[] researchCost = new int[7];
    [SerializeField] private InteractionAreaType[] researchGoals = new InteractionAreaType[7];
    private List<string> letterPool;
    
    //Research
    private int currentResearchIndex;
    private bool researchIsActive;
    public bool GetResearchIsActive() => researchIsActive;
    private Coroutine researchCostCoroutine;
    private int researchSuccessCounter;

    //Memory Fields & Buttons
    private MemoryField[] memoryFields =  new MemoryField[24];
    private List<MemoryField> memoryFieldsTemp = new List<MemoryField>();
    private string currentButtonText;
    private int[] researchIndexToCompare = new int[2];
    private List<MemoryField> memoryFieldsToCompare = new List<MemoryField>();
    
    public static Lab instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    public void InitializeLab(int startResearchIndex)
    {
        currentResearchIndex = startResearchIndex;
        
        labButtonTMP = labButton.transform.GetChild(0).GetComponent<TMP_Text>();
        
        //Save & Deactivate all memory fields
        for (int i = 0; i < labMemoryHolder.childCount; i++)
        {
            MemoryField nextMemoryField = labMemoryHolder.GetChild(i).gameObject.GetComponent<MemoryField>();
            memoryFields[i] = nextMemoryField;
            nextMemoryField.ActivateDeactivate(false);
        }

        PrepareNextResearch();
    }

    #region |-------------- RESEARCH PREPARATIONS --------------|
    
    void PrepareNextResearch()
    {
        DefineLetterPool();
        
        //MemoryFieldsUsed is used as a temp container to store and remove the memoryFields for randomization
        foreach (MemoryField nextMemory in memoryFields)
        {
            //nextMemory.SetSolved(false);
            memoryFieldsTemp.Add(nextMemory);
        }
        
        for (int i = 0; i < 12; i++)
        {
            //This picks two random memoryFields and gives them the same Value
            int randomPick = Random.Range(0, memoryFieldsTemp.Count);
            memoryFieldsTemp[randomPick].SetFieldID(i);
            memoryFieldsTemp[randomPick].SetLetter(letterPool[i]);
            memoryFieldsTemp.RemoveAt(randomPick);
            
            randomPick = Random.Range(0, memoryFieldsTemp.Count);
            memoryFieldsTemp[randomPick].SetFieldID(i);
            memoryFieldsTemp[randomPick].SetLetter(letterPool[i]);
            memoryFieldsTemp.RemoveAt(randomPick);
        }
        
        //Reset research Compare
        researchIndexToCompare[0] = -1;
        researchIndexToCompare[1] = -1;
        memoryFieldsToCompare.Clear();
        
        
        //Prepare Button_Tooltip
        labButtonTMP.text = 
            $"START RESEARCH:<br><b>{researchGoals[currentResearchIndex].ToString()}</b><br>({researchCost[currentResearchIndex]} PIPS/s)";

        currentButtonText = labButtonTMP.text;
        labButton.gameObject.GetComponent<Interactor_StartResearch>().SetActivity(true);
    }

    void DefineLetterPool()
    {
        switch (currentResearchIndex)
        {
            case 0:
                letterPool = new List<string> {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"};
                break;
            
            case 1:
                letterPool = new List<string> {"A", "a",  "B", "b", "N", "n", "X", "x", "Y", "y", "P", "p"};
                break;
        }
        
    }
    
    #endregion
    
    #region |-------------- ACTIVE RESEARCH --------------|

    public void StartStopResearch()
    {
        if (!researchIsActive) //Start Research
        {
            //Check if there is enough Pips to Start
            if (researchCost[currentResearchIndex] > CPU.instance.GetPips()) return;
            
            labButtonTMP.text = $"Stop Research";
            researchIsActive = true;
            researchCostCoroutine = StartCoroutine(ResearchCost());
            
            foreach (MemoryField nextMemory in memoryFields)
            {
                //Activate all Cards which are not yet researched
                if (!nextMemory.GetSolvedState())
                {
                    if (printLog) Debug.Log($"Activate Memory Field {nextMemory.gameObject.name}");
                    nextMemory.ActivateDeactivate(true);
                }
            }
            
            if (printLog) Debug.Log("|--------------LAB RESEARCH STARTED --------------|");
            
        }

        else //Stop Research
        {
            researchIsActive = false;
            StopCoroutine(researchCostCoroutine); 
            labButtonTMP.text = currentButtonText;
            
            //Deactivate all Cards which are not yet researched
            foreach (MemoryField nextMemory in memoryFields)
            {
                if (!nextMemory.GetSolvedState()) nextMemory.ActivateDeactivate(false);
            }
            
            if (printLog) Debug.Log("|--------------LAB RESEARCH STOPPED--------------|");
        }
    }

    //Send by MemoryCards
    public void ResearchInput(int _index, int _researchID)
    {
        if (researchIndexToCompare[0] == -1)
        {
            //First Card
            researchIndexToCompare[0] = _researchID;
            memoryFields[_index].SetSolved(true);
            memoryFieldsToCompare.Add(memoryFields[_index]);
            Debug.Log($"Research: Current ID: {_researchID}");
        }
        
        else if (researchIndexToCompare[1] == -1)
            
        {
            //Second Card
            researchIndexToCompare[1] = _researchID;
            memoryFields[_index].SetSolved(true);
            memoryFieldsToCompare.Add(memoryFields[_index]);
            Debug.Log($"Research: Second card has ID: {_researchID}");

            if (researchIndexToCompare[0] == researchIndexToCompare[1]) //Correct Pair
            {
                // Solved two cards
                Debug.Log($"Solved Cards with ID: {researchIndexToCompare[0]}");
                ResetActiveMemoryFields();
                researchSuccessCounter++;
                if (researchSuccessCounter == 12) StartCoroutine(FinishResearch());
            }

            else //Wrong Pair
            {
                //Reset research Compare
                Debug.Log($"Failed to solve cards with: {researchIndexToCompare[0]}");
                StartCoroutine(CloseMemoryFields
                    (memoryFieldsToCompare[0], memoryFieldsToCompare[1]));
                ResetActiveMemoryFields();
                
            }
        }

    }
    
    IEnumerator FinishResearch()
    {
        StartStopResearch();
        
        researchSuccessCounter = 0;
        
        //Prep Button_Tooltip
        labButton.gameObject.GetComponent<Interactor_StartResearch>().SetActivity(false);
        labButtonTMP.text = "RESEARCH SUCCESSFUL!";
        yield return new WaitForSeconds(0.5f);
        
        foreach (MemoryField nextMemory in memoryFields)
        {
            nextMemory.SetSolved(false);
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitForSeconds(0.5f);

        ProgressManager.instance.ResearchProgress(currentResearchIndex);
        
        yield return new WaitForSeconds(3f);
        
        currentResearchIndex++;
        PrepareNextResearch();
    }
    
    #endregion
    
    #region |-------------- RESEARCH HELPERS --------------|
    
    void ResetActiveMemoryFields()
    {
        researchIndexToCompare[0] = -1;
        researchIndexToCompare[1] = -1;
        memoryFieldsToCompare.Clear();
    }
    
    IEnumerator CloseMemoryFields(MemoryField _fieldId1, MemoryField _fieldId2)
    {
        yield return new WaitForSeconds(1f);
        _fieldId1.SetSolved(false);
        _fieldId2.SetSolved(false);
    }

    IEnumerator ResearchCost()
    {
        while (researchIsActive)
        {
            if (printLog) Debug.Log("|--------------LAB RESEARCH COST UPDATE--------------|");
            if (printLog) Debug.Log($"Current cost: {researchCost[currentResearchIndex]} per Second.");
            if (researchCost[currentResearchIndex] > CPU.instance.GetPips())
            {
                //Research Cost can't be paid
                StartStopResearch();
                yield break;
            }
            
            else
            {
                CPU.instance.ChangeResource(Resource.Pips, (double)-researchCost[currentResearchIndex]);
            }
            
            yield return new WaitForSeconds(1f);
            
        }
    }
    
    
    #endregion
}
