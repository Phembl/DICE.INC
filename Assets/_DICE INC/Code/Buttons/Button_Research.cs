using DICEINC.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_Research : Button
{
    
    void Start()
    {
        isActive = true;
    }

    protected override void ButtonAction()
    {
        Lab.instance.StartStopResearch();
    }
    
}
