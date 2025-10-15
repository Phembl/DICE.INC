using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageInteractor : MonoBehaviour
{
    [SerializeField] private Color colorActive;
    [SerializeField] private Color colorInactive;

    private int msgID;
    private bool isActivated;
    private bool isNew;
    private TMP_Text messageText;
    private Image messageBackground;

    public void InstantiateMessage(int _msgID)
    {
        messageText = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        messageBackground = GetComponent<Image>();
        
        msgID = _msgID;
        isNew = true;
    }
    
    void OnMouseUp()
    {
        if (isActivated) return;
        
        MessageManager.instance.ShowMessage(transform.GetSiblingIndex(), msgID);
    }

    public void ActivateDeactivate(bool active)
    {
        isActivated = active;
        if (isActivated)
        {
            messageText.color = colorInactive;
            messageBackground.color = colorActive;
        }

        if (!isActivated)
        {
            messageText.color = colorActive;
            messageBackground.color = colorInactive;
        }
    }
}
