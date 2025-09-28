using DG.Tweening;
using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Button_Memory : Button
{
    
    [ShowInInspector, ReadOnly] private int fieldID;
    public void SetFieldID(int newFieldID) => fieldID = newFieldID;

    [SerializeField] private Image myIcon;
    public void SetIcon(Sprite newIcon) => myIcon.sprite = newIcon;
    
    private int myIndex;
    private float startPosY;
    
    
    [ShowInInspector, ReadOnly] private bool isSolved;
    public bool GetSolvedState() => isSolved;

    void Start()
    {
        //Needs to go one layer up because it is childed to mask
        myIndex = gameObject.transform.parent.GetSiblingIndex();
        startPosY = transform.localPosition.y;
        myIcon.DOFade(0, 0f);
    }
    
    protected override void ButtonAction()
    {
        Lab.instance.ResearchInput(myIndex, fieldID);
    }
    
    public void SetSolved(bool solved)
    {
        
        if (solved)
        {
            isSolved = true;
            isActive = false;
            myIcon.DOFade(1, 0.1f);
        }

        else
        {
           
            myIcon.DOFade(0, 0.5f)
                .OnComplete(() =>
                {
                    isSolved = false;
                    isActive = true;
                });
        }
    } 
    
    public void ActivateDeactivate(bool activate) => isActive = activate;

    public void ShowHide(bool show)
    {
        if (show)
        {
            gameObject.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuad);

        }
        else
        {
            gameObject.transform.DOLocalMoveY(startPosY, 0.5f).SetEase(Ease.InQuad);;
        }
    }
}
