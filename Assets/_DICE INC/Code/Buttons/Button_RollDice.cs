using DICEINC.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_RollDice : Button
{
    void Start()
    {
        isActive = true;
    }
    
    protected override void ButtonAction()
    {
        if (CPU.instance.GetDice() > 0) DiceManager.instance.RollDice();
    }
    
    
}
