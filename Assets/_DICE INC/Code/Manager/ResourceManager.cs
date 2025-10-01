using System;
using System.IO.Pipes;
using DICEINC.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ResourceManager : MonoBehaviour
{
    
    [TitleGroup("References")] 
    [SerializeField] private Transform resourceHolder;
    
    [TitleGroup("Settings")] 
    [SerializeField] private bool pipsUnlocked;
    [SerializeField] private bool diceUnlocked;
    [SerializeField] private bool materialUnlocked;
    [SerializeField] private bool luckUnlocked;
    [SerializeField] private bool mDiceUnlocked;
    [SerializeField] private bool dataUnlocked;
    
    //TMP components
    private TMP_Text pipsTitleTMP;
    private TMP_Text pipsCounterTMP;
    private TMP_Text diceTitleTMP;
    private TMP_Text diceCounterTMP;
    private TMP_Text materialTitleTMP;
    private TMP_Text materialCounterTMP;
    private TMP_Text luckTitleTMP;
    private TMP_Text luckCounterTMP;
    private TMP_Text mDiceTitleTMP;
    private TMP_Text mDiceCounterTMP;
    private TMP_Text dataTitleTMP;
    private TMP_Text dataCounterTMP;
    
    private Color colorActive;
    private Color colorInactive;

    public static ResourceManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    void OnEnable()
    {
        CPU.OnPipsChanged += UpdateDisplayPips;
        CPU.OnDiceChanged += UpdateDisplayDice;
        CPU.OnMaterialChanged += UpdateDisplayMaterial;
        CPU.OnLuckChanged += UpdateDisplayLuck;
        CPU.OnMDiceChanged += UpdateDisplayMDice;
        CPU.OnDataChanged += UpdateDisplayData;
    }

    public void InitializeResourceManager()
    {
        colorActive = SettingsManager.instance.colorNormal;
        colorInactive = SettingsManager.instance.colorInactive;
        
        pipsTitleTMP = resourceHolder.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        pipsCounterTMP = resourceHolder.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();
        
        diceTitleTMP = resourceHolder.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>();
        diceCounterTMP = resourceHolder.GetChild(1).transform.GetChild(1).GetComponent<TMP_Text>();
        
        materialTitleTMP = resourceHolder.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();
        materialCounterTMP = resourceHolder.GetChild(3).transform.GetChild(1).GetComponent<TMP_Text>();
    
        luckTitleTMP = resourceHolder.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>();
        luckCounterTMP = resourceHolder.GetChild(2).transform.GetChild(1).GetComponent<TMP_Text>();
    
        mDiceTitleTMP = resourceHolder.GetChild(4).transform.GetChild(0).GetComponent<TMP_Text>();
        mDiceCounterTMP = resourceHolder.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>();
    
        dataTitleTMP = resourceHolder.GetChild(5).transform.GetChild(0).GetComponent<TMP_Text>();
        dataCounterTMP = resourceHolder.GetChild(5).transform.GetChild(1).GetComponent<TMP_Text>();
        
        if (pipsUnlocked) UnlockResource(Resource.Pips);
        else LockResource(Resource.Pips);
        
        if (diceUnlocked) UnlockResource(Resource.Dice);
        else LockResource(Resource.Dice);
        
        if (materialUnlocked) UnlockResource(Resource.Material);
        else LockResource(Resource.Material);
        
        if (luckUnlocked) UnlockResource(Resource.Luck);
        else LockResource(Resource.Luck);
        
        if (mDiceUnlocked) UnlockResource(Resource.mDice);
        else LockResource(Resource.mDice);
        
        if (dataUnlocked) UnlockResource(Resource.Data);
        else LockResource(Resource.Data);
        
    }
    
    public void UnlockResource(Resource resource)
    {
        
        switch (resource)
        {
            case Resource.Pips:
                pipsUnlocked = true;
                pipsTitleTMP.text = "pips";
                pipsCounterTMP.text = "0";
                pipsTitleTMP.color = colorActive;
                pipsCounterTMP.color = colorActive;
                break;
            
            case Resource.Dice:
                diceUnlocked = true;
                diceTitleTMP.text = "dice";
                diceCounterTMP.text = "0";
                diceTitleTMP.color = colorActive;
                diceCounterTMP.color = colorActive;
                Debug.Log("Resource: DiceManager are now unlocked");
                CPU.instance.UnlockDice();
                break;
            
            case Resource.Material:
                materialUnlocked = true;
                materialTitleTMP.text = "material";
                materialCounterTMP.text = "0";
                materialTitleTMP.color = colorActive;
                materialCounterTMP.color = colorActive;
                Debug.Log("Resource: Material are now unlocked");
                CPU.instance.UnlockTools();
                break;
            
            case Resource.Luck:
                luckUnlocked = true;
                luckTitleTMP.text = "luck";
                luckCounterTMP.text = "0";
                luckTitleTMP.color = colorActive;
                luckCounterTMP.color = colorActive;
                Debug.Log("Resource: luck are now unlocked");
                CPU.instance.UnlockLuck();
                break;
            
            case Resource.mDice:
                mDiceUnlocked = true;
                mDiceTitleTMP.text = "mDICE";
                mDiceCounterTMP.text = "0";
                mDiceTitleTMP.color = colorActive;
                mDiceCounterTMP.color = colorActive;
                Debug.Log("Resource: mDice are now unlocked");
                CPU.instance.UnlockMDice();
                break;
            
            case Resource.Data:
                dataUnlocked = true;
                dataTitleTMP.text = "data";
                dataCounterTMP.text = "0";
                dataTitleTMP.color = colorActive;
                dataCounterTMP.color = colorActive;
                Debug.Log("Resource: Data are now unlocked");
                CPU.instance.UnlockData();
                break;
            
        }
    }

    void LockResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Pips:
                pipsUnlocked = false;
                pipsTitleTMP.text = "???";
                pipsCounterTMP.text = "";
                pipsTitleTMP.color = colorInactive;
                pipsCounterTMP.color = colorInactive;
                break;
            
            case Resource.Dice:
                diceUnlocked = false;
                diceTitleTMP.text = "???";
                diceCounterTMP.text = "";
                diceTitleTMP.color = colorInactive;
                diceCounterTMP.color = colorInactive;
                break;
            
            case Resource.Material:
                materialUnlocked = false;
                materialTitleTMP.text = "???";
                materialCounterTMP.text = "";
                materialTitleTMP.color = colorInactive;
                materialCounterTMP.color = colorInactive;
                break;
            
            case Resource.Luck:
                luckUnlocked = false;
                luckTitleTMP.text = "???";
                luckCounterTMP.text = "";
                luckTitleTMP.color = colorInactive;
                luckCounterTMP.color = colorInactive;
                break;
            
            case Resource.mDice:
                mDiceUnlocked = false;
                mDiceTitleTMP.text = "???";
                mDiceCounterTMP.text = "";
                mDiceTitleTMP.color = colorInactive;
                mDiceCounterTMP.color = colorInactive;
                break;
            
            case Resource.Data:
                dataUnlocked = false;
                dataTitleTMP.text = "???";
                dataCounterTMP.text = "";
                dataTitleTMP.color = colorInactive;
                dataCounterTMP.color = colorInactive;
                break;
            
        }
    }
    

    void UpdateDisplayPips()
    {
        if (!pipsUnlocked) UnlockResource(Resource.Pips);
        double pipsCurrent = CPU.instance.GetPips();
        pipsCounterTMP.text = Utility.ShortenNumberToString(pipsCurrent);
    }
    
    void UpdateDisplayDice()
    {
        if (!diceUnlocked) UnlockResource(Resource.Dice);
        double diceCurrent = CPU.instance.GetDice();
        diceCounterTMP.text = Utility.ShortenNumberToString(diceCurrent);
    }

    void UpdateDisplayMaterial()
    {
        if (!materialUnlocked) UnlockResource(Resource.Material);
        double toolsCurrent = CPU.instance.GetTools();
        materialCounterTMP.text = Utility.ShortenNumberToString(toolsCurrent);
    }
    
    void UpdateDisplayLuck()
    {
        if (!luckUnlocked) UnlockResource(Resource.Luck);
        double luckCurrent = CPU.instance.GetLuck();
        luckCounterTMP.text = Utility.ShortenNumberToString(luckCurrent);
    }
    
    void UpdateDisplayMDice()
    {
        if (!mDiceUnlocked) UnlockResource(Resource.mDice);
        double mDiceCurrent = CPU.instance.GetMDice();
        mDiceCounterTMP.text = Utility.ShortenNumberToString(mDiceCurrent);
    }
    
    void UpdateDisplayData()
    {
        if (!dataUnlocked) UnlockResource(Resource.Data);
        double dataCurrent = CPU.instance.GetData();
        dataCounterTMP.text = Utility.ShortenNumberToString(dataCurrent);
    }
}
