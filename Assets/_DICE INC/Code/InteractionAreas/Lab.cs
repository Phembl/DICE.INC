using System.Collections;
using System.Collections.Generic;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Lab : MonoBehaviour
{
    [TitleGroup("Lab")] 
    [Header("References")]
    [SerializeField] private Transform labMemoryHolder;
    //[SerializeField] private GameObject labButton;
    [SerializeField] private TMP_Text labTMP;
    [SerializeField] private Sprite[] memoryIcons;
    [Space] 
    
    [Header("Settings")]
    [SerializeField] private bool printLog;
    [SerializeField] private int[] researchCost = new int[7];
    [SerializeField] private InteractionAreaType[] researchGoals = new InteractionAreaType[7];
    private List<string> letterPool;
    private List<Sprite> iconPool;
    
    private Tweener textChange;
    
    //Research
    private int currentResearchIndex;
    private bool researchIsActive;
    private bool researchIsPrepared;
    public bool GetResearchIsActive() => researchIsActive;
    private Coroutine researchCostCoroutine;
    private int researchSuccessCounter;
    private int currentResearchOverallCost;

    //Memory Fields & Buttons
    private Transform[] buttonObjects = new Transform[16];
    private Button_Memory[] memoryFields =  new Button_Memory[16];
    private List<Button_Memory> memoryFieldsTemp = new List<Button_Memory>();
    private string currentButtonText;
    private int[] researchIndexToCompare = new int[2];
    private List<Button_Memory> memoryFieldsToCompare = new List<Button_Memory>();
    
    public static Lab instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    public void InitializeLab(int startResearchIndex)
    {
        currentResearchIndex = startResearchIndex;
        currentResearchOverallCost = 0;
        
        //Save & Deactivate all memory fields
        for (int i = 0; i < labMemoryHolder.childCount; i++)
        {
            buttonObjects[i] = labMemoryHolder.GetChild(i).transform.GetChild(0);
            memoryFields[i] = buttonObjects[i].GetComponent<Button_Memory>();
            memoryFields[i].ActivateDeactivate(false);
        }

        PrepareNextResearch();
    }

    #region |-------------- RESEARCH PREPARATIONS --------------|
    
    void PrepareNextResearch()
    {
        researchIsPrepared = true;
        DefineIconPool();
        
        //MemoryFieldsUsed is used as a temp container to store and remove the memoryFields for randomization
        foreach (Button_Memory nextMemory in memoryFields)
        {
            //nextMemory.SetSolved(false);
            memoryFieldsTemp.Add(nextMemory);
        }
        
        for (int i = 0; i < 8; i++)
        {
            //This picks two random memoryFields and gives them the same Value
            int randomPick = Random.Range(0, memoryFieldsTemp.Count);
            memoryFieldsTemp[randomPick].SetFieldID(i);
            memoryFieldsTemp[randomPick].SetIcon(iconPool[i]);
            memoryFieldsTemp.RemoveAt(randomPick);
            
            randomPick = Random.Range(0, memoryFieldsTemp.Count);
            memoryFieldsTemp[randomPick].SetFieldID(i);
            memoryFieldsTemp[randomPick].SetIcon(iconPool[i]);
            memoryFieldsTemp.RemoveAt(randomPick);
        }
        
        //Reset research Compare
        researchIndexToCompare[0] = -1;
        researchIndexToCompare[1] = -1;
        memoryFieldsToCompare.Clear();
        
        WriteResearchText($"RESEARCH: <b>{researchGoals[currentResearchIndex].ToString()}</b><br>COST: <b>{researchCost[currentResearchIndex]} PIPS/s</b>");

        researchIsPrepared = false;
    }

    void WriteResearchText(string text)
    {
        if (textChange != null) textChange.Kill();
        
        textChange = labTMP.DOFade(0f, 0.2f)
            .OnComplete(() =>
        {
            labTMP.text = text;
            textChange = labTMP.DOFade(1f, 1f);
        });
       
       
    }
    
    void DefineIconPool()
    {
        iconPool = new List<Sprite>();
        
        for (int i = 0; i < 8; i++)
        {
            iconPool.Add(memoryIcons[i + (8 * currentResearchIndex)]);
        }
    }
    
    #endregion
    
    #region |-------------- ACTIVE RESEARCH --------------|

    public void StartStopResearch()
    {
        if (researchIsPrepared) return;
        
        if (!researchIsActive) //Start Research
        {
            //Check if there is enough Pips to Start
            if (researchCost[currentResearchIndex] > CPU.instance.GetPips()) return;
            
            researchIsActive = true;
            researchIsPrepared = true;
            StartCoroutine(ActivateMemoryButtons());
            
            if (printLog) Debug.Log("|--------------LAB RESEARCH STARTED --------------|");
            
        }

        else //Stop Research
        {
            researchIsActive = false;
            researchIsPrepared = true;
            StopCoroutine(researchCostCoroutine); 
            
            StartCoroutine(DeactivateMemoryButtons());
            WriteResearchText($"RESEARCH: <b>{researchGoals[currentResearchIndex].ToString()}</b><br>COST: <b>{researchCost[currentResearchIndex]} PIPS/s</b>");
            
            if (printLog) Debug.Log("|--------------LAB RESEARCH STOPPED--------------|");
        }
    }

    private IEnumerator ActivateMemoryButtons()
    {
        WriteResearchText($"PREPARING RESEARCH");
        
        foreach (Button_Memory nextMemory in memoryFields)
        {
            //Show and activate all Memory Buttons which are not yet researched
            if (!nextMemory.GetSolvedState())
            {
                nextMemory.ShowHide(true);
                
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        yield return new WaitForSeconds(0.5f);

        foreach (Button_Memory nextMemory in memoryFields)
        {
            if (!nextMemory.GetSolvedState()) nextMemory.ActivateDeactivate(true);
        }

        researchIsPrepared = false;
        researchCostCoroutine = StartCoroutine(ResearchCost());
    }

    private IEnumerator DeactivateMemoryButtons()
    {
        //Deactivate Memory Buttons and Hide those which are not solved
        foreach (Button_Memory nextMemory in memoryFields)
        {
            nextMemory.ActivateDeactivate(false);
            if (!nextMemory.GetSolvedState()) nextMemory.ShowHide(false);
        }
        
        yield return new WaitForSeconds(0.5f);

        researchIsPrepared = false;
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
            if (printLog) Debug.Log($"Research: Current ID: {_researchID}");
        }
        
        else if (researchIndexToCompare[1] == -1)
            
        {
            //Second Card
            researchIndexToCompare[1] = _researchID;
            memoryFields[_index].SetSolved(true);
            memoryFieldsToCompare.Add(memoryFields[_index]);
            if (printLog) Debug.Log($"Research: Second card has ID: {_researchID}");

            if (researchIndexToCompare[0] == researchIndexToCompare[1]) //Correct Pair
            {
                // Solved two cards
                if (printLog) Debug.Log($"Solved Cards with ID: {researchIndexToCompare[0]}");
                ResetActiveMemoryFields();
                researchSuccessCounter++;
                if (researchSuccessCounter == 8) StartCoroutine(ResearchSuccess());
            }

            else //Wrong Pair
            {
                //Reset research Compare
                if (printLog) Debug.Log($"Failed to solve cards with: {researchIndexToCompare[0]}");
                StartCoroutine(CloseMemoryFields
                    (memoryFieldsToCompare[0], memoryFieldsToCompare[1]));
                ResetActiveMemoryFields();
                
            }
        }

    }
    
    IEnumerator ResearchSuccess()
    {
        researchIsActive = false;
        researchIsPrepared = true;
        StopCoroutine(researchCostCoroutine); 
        
        WriteResearchText($"research success!<br><b>{researchGoals[currentResearchIndex].ToString()}</b> unlocked.");
        
        researchSuccessCounter = 0;
        
        yield return new WaitForSeconds(2f);
        
        foreach (Button_Memory nextMemory in memoryFields)
        {
            nextMemory.ActivateDeactivate(false);
            nextMemory.ShowHide(false);
            nextMemory.SetSolved(false);
        }
        
        yield return new WaitForSeconds(1f);

        ProgressManager.instance.ResearchProgress(currentResearchIndex);
        
        yield return new WaitForSeconds(5f);
        
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
    
    IEnumerator CloseMemoryFields(Button_Memory _fieldId1, Button_Memory _fieldId2)
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
                
                currentResearchOverallCost += researchCost[currentResearchIndex];
                labTMP.text = $"Overall cost: \n<b>{currentResearchOverallCost} PIPS</b>";
            }
            
            yield return new WaitForSeconds(1f);
            
        }
    }
    
    
    #endregion
    
    
    #region |-------------- TOOLTIP --------------|

    public TooltipData GetTooltipData()
    {
        TooltipData data = new TooltipData();
        
        data.areaTitle = "Laboratory";
        data.areaDescription = "The laboratory,";
        
        
        return data;
    }
    #endregion
}
