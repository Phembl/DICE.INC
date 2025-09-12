using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interactor_RollDice : MonoBehaviour
{
    [SerializeField] private bool isActive;
    //Sprites
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteHovered;
    [Space]
    //Colors
    private Color colorNormal;
    private Color colorDark;
    private Color colorInactive;
    
    private TMP_Text title;
    private Image image;
    
    //State Tracking
    private bool isHovered;
    

    void Start()
    {
        colorNormal = Settings.instance.colorNormal;
        colorDark = Settings.instance.colorDark;
        colorInactive = Settings.instance.colorInactive;
        
        image = GetComponent<Image>();
        title = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        
        
        image.sprite = spriteNormal;
        image.color = colorNormal;
        title.color = colorNormal;
        
        CheckIfActive();
        
    }
    
    void OnEnable()
    {
        CPU.OnDiceChanged += CheckIfActive;
    }

    void CheckIfActive()
    {
        double currentDice = CPU.instance.GetDice();
        
        if (currentDice > 0)
        {
            isActive = true;
            image.color = colorNormal;
            title.color = colorNormal;
            title.text = $"roll {currentDice} dice";
        }
        
        else if (currentDice == 0)
        {
            isActive = false;
            image.sprite = spriteNormal;
            image.color = colorInactive;
            title.color = colorInactive;
            title.text = $"no dice";
        }
    }

    void OnMouseOver()
    {
        if (isHovered || !isActive) return;
        
        isHovered = true;
        
        image.sprite = spriteHovered;
        title.color = colorDark;
        
    }

    void OnMouseExit()
    {
        if (!isHovered) return;
        
        isHovered = false;

        if (isActive)
        {
            image.sprite = spriteNormal;
            title.color = colorNormal;
        }
        
    }

    void OnMouseDown()
    {
        if (!isActive) return;

        DiceManager.instance.RollDice();
    }

    public void ShowRollResult(int _diceRolled, int _pipsGenerated)
    {
        title.text = $"You rolled {_diceRolled} dice and won {_pipsGenerated} pips!";
    }
}
