using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class MemoryField : MonoBehaviour
{

    [ShowInInspector, ReadOnly] private int fieldID;
    public void SetFieldID(int newFieldID) => fieldID = newFieldID;

    private TMP_Text myLetterTMP;
    public void SetLetter(string newLetter) => myLetterTMP.SetText(newLetter);
    private Image myImage;
    private int myIndex;
    
    private Color colorActive;
    private Color colorInactive;
    private Color colorDark;

    private bool isActive;
    private bool isHovered;
    [ShowInInspector, ReadOnly] private bool isSolved;
    public bool GetSolvedState() => isSolved;
    
    
    void Start()
    {
        colorActive = SettingsManager.instance.colorNormal;
        colorInactive = SettingsManager.instance.colorInactive;
        colorDark = SettingsManager.instance.colorDark;
        
        myLetterTMP = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        myImage = transform.GetChild(2).GetComponent<Image>();
        
        transform.GetChild(1).GetComponent<Image>().color = colorActive;
        
        myIndex = gameObject.transform.GetSiblingIndex();
    }

    public void ActivateDeactivate(bool _isActive)
    {
        isActive = _isActive;
        if (isActive) myImage.color = colorActive;
        else myImage.color = colorInactive;
    }
    
    void OnMouseOver()
    {
        if (isHovered || !isActive || isSolved) return;
        
        isHovered = true;
        
        if (!isSolved) myImage.color = colorInactive;
        
    }

    void OnMouseExit()
    {
        if (!isHovered) return;
        
        isHovered = false;
        
        if (!isSolved) myImage.color = colorActive;
    }

    void OnMouseDown()
    {
        if (!isActive || isSolved) return;

        Lab.instance.ResearchInput(myIndex, fieldID);

    }

    public void SetSolved(bool solved)
    {
        
        if (solved)
        {
            isSolved = true;
            myImage.DOFade(0, 0.2f);
        }

        else
        {
            Color tempColor = new Color(colorActive.r, colorActive.g, colorActive.b, 0f);
            myImage.color = tempColor;
            myImage.DOFade(1, 0.5f)
                .OnComplete(() =>
                {
                    isSolved = false;
                });
        }
    } 
    
   
    

}
