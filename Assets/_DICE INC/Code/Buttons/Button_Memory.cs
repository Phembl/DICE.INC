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
    
    // Called by Lab when the two correct icons are found and the button is solved
    [ShowInInspector, ReadOnly] private bool isSolved;
    public bool GetSolvedState() => isSolved;
    public void SetSolvedState(bool solved) => isSolved = solved;

    private Tween moveButton;

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
    
    // Called by Lab when the button should not be pressable
    public void ActivateDeactivate(bool activate)
    {
        isActive = activate;
        if (!activate && myIcon.color.a > 0 && !isSolved) myIcon.DOFade(0, 0.2f);
    }

    // Called by Lab when a button is pressed and its icon should be shown or hidden
    public void ShowIcon(bool _showIcon)
    {
        if (_showIcon)
        {
            isActive = false;
            myIcon.DOFade(1, 0.1f);
        }

        else
        {
           
            myIcon.DOFade(0, 0.4f)
                .OnComplete(() =>
                {
                    isActive = true;
                });
        }
    } 
    
    // Called by Lab when research is started
    public void ShowHide(bool _show)
    {
        if (_show)
        {
            moveButton = gameObject.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuad);

        }
        else
        {
            moveButton = gameObject.transform.DOLocalMoveY(startPosY, 0.5f).SetEase(Ease.InQuad);;
        }
    }
}
