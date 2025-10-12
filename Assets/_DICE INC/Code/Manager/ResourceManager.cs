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
        
    }
    
    public void UnlockResource(Resource resource)
    {
        
        switch (resource)
        {
            case Resource.Pips:
                pipsTitleTMP.text = "pips";
                pipsCounterTMP.text = "0";
                pipsTitleTMP.color = colorActive;
                pipsCounterTMP.color = colorActive;
                break;
            
            case Resource.Dice:
                diceTitleTMP.text = "dice";
                diceCounterTMP.text = "0";
                diceTitleTMP.color = colorActive;
                diceCounterTMP.color = colorActive;
                Debug.Log("Resource: Dice are now unlocked");
                CPU.instance.UnlockDice();
                break;
            
            case Resource.Material:
                materialTitleTMP.text = "material";
                materialCounterTMP.text = "0";
                materialTitleTMP.color = colorActive;
                materialCounterTMP.color = colorActive;
                Debug.Log("Resource: Material are now unlocked");
                CPU.instance.UnlockTools();
                break;
            
            case Resource.Luck:
                luckTitleTMP.text = "luck";
                luckCounterTMP.text = "0";
                luckTitleTMP.color = colorActive;
                luckCounterTMP.color = colorActive;
                Debug.Log("Resource: luck are now unlocked");
                CPU.instance.UnlockLuck();
                break;
            
            case Resource.mDICE:
                mDiceTitleTMP.text = "mDICE";
                mDiceCounterTMP.text = "0";
                mDiceTitleTMP.color = colorActive;
                mDiceCounterTMP.color = colorActive;
                Debug.Log("Resource: mDICE are now unlocked");
                CPU.instance.UnlockMDice();
                break;
            
            case Resource.Data:
                dataTitleTMP.text = "data";
                dataCounterTMP.text = "0";
                dataTitleTMP.color = colorActive;
                dataCounterTMP.color = colorActive;
                Debug.Log("Resource: Data are now unlocked");
                CPU.instance.UnlockData();
                break;
            
        }
    }

    public void LockResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Pips:
                pipsTitleTMP.text = "???";
                pipsCounterTMP.text = "";
                pipsTitleTMP.color = colorInactive;
                pipsCounterTMP.color = colorInactive;
                break;
            
            case Resource.Dice:
                diceTitleTMP.text = "???";
                diceCounterTMP.text = "";
                diceTitleTMP.color = colorInactive;
                diceCounterTMP.color = colorInactive;
                break;
            
            case Resource.Material:
                materialTitleTMP.text = "???";
                materialCounterTMP.text = "";
                materialTitleTMP.color = colorInactive;
                materialCounterTMP.color = colorInactive;
                break;
            
            case Resource.Luck:
                luckTitleTMP.text = "???";
                luckCounterTMP.text = "";
                luckTitleTMP.color = colorInactive;
                luckCounterTMP.color = colorInactive;
                break;
            
            case Resource.mDICE:
                mDiceTitleTMP.text = "???";
                mDiceCounterTMP.text = "";
                mDiceTitleTMP.color = colorInactive;
                mDiceCounterTMP.color = colorInactive;
                break;
            
            case Resource.Data:
                dataTitleTMP.text = "???";
                dataCounterTMP.text = "";
                dataTitleTMP.color = colorInactive;
                dataCounterTMP.color = colorInactive;
                break;
            
        }
    }
    

    void UpdateDisplayPips()
    {
        double pipsCurrent = CPU.instance.GetPips();
        pipsCounterTMP.text = Utility.ShortenNumberToString(pipsCurrent);
    }
    
    void UpdateDisplayDice()
    {
        if (!CPU.instance.GetDiceUnlockState()) return;
        
        double diceCurrent = CPU.instance.GetDice();
        diceCounterTMP.text = Utility.ShortenNumberToString(diceCurrent);
    }

    void UpdateDisplayMaterial()
    {
        if (!CPU.instance.GetMaterialUnlockState()) return;
        
        double toolsCurrent = CPU.instance.GetTools();
        materialCounterTMP.text = Utility.ShortenNumberToString(toolsCurrent);
    }
    
    void UpdateDisplayLuck()
    {
        if (!CPU.instance.GetLuckUnlockState()) return;
        
        double luckCurrent = CPU.instance.GetLuck();
        luckCounterTMP.text = Utility.ShortenNumberToString(luckCurrent);
    }
    
    void UpdateDisplayMDice()
    {
        if (!CPU.instance.GetMDiceUnlockState()) return;
        
        double mDiceCurrent = CPU.instance.GetMDice();
        mDiceCounterTMP.text = Utility.ShortenNumberToString(mDiceCurrent);
    }
    
    void UpdateDisplayData()
    {
        if (!CPU.instance.GetDataUnlockState()) return;
        
        double dataCurrent = CPU.instance.GetData();
        dataCounterTMP.text = Utility.ShortenNumberToString(dataCurrent);
    }
}
