using DICEINC.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_RollDice : MonoBehaviour
{
    [SerializeField] private Sprite spritePressed;
    
    private Sprite spriteUnpressed;
    
    //Components
    private Image bgImage;
    
    //State Tracking
    private bool isActive;
    

    void Start()
    {
        bgImage = GetComponent<Image>();
        spriteUnpressed = bgImage.sprite;
      
    }

    void OnMouseDown()
    {
        bgImage.sprite = spritePressed;
        
    }

    void OnMouseUp()
    {
        bgImage.sprite = spriteUnpressed;
        
        if (CPU.instance.GetDice() > 0) Dice.instance.RollDice();
        
    }
    
    
}
