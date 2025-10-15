using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private bool printLog;
    [TitleGroup("References")] 
    [SerializeField] private GameObject messageSubjectPrefab;
    [SerializeField] private Transform messageSubjectContainer;
    [SerializeField] private Message[] allMessages;

    private string[] testMessages = new string[3] { "sddfhjsohfsohfoishfoishoi fhsohoifhsoih dfosfosdfosdjfoi" , "sddfhjsohfso hfoishfoishoifhsohoifhsoih", "sddfhjsohfsohf oishfoishoifhsoh oifhsoih dfosfo sdfosdjfoi dfosfosdfosdjfoidfosfosdfosdjfoi"};
    
    public static MessageManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        CreateMessage(0);
        CreateMessage(1);
        CreateMessage(2);
        CreateMessage(2);
        CreateMessage(2);
        CreateMessage(2);
        CreateMessage(2);
        CreateMessage(2);
        CreateMessage(2);
    }

    public void CreateMessage(int msgID)
    {
        string newSubjectText = allMessages[msgID].NewMessageData.messageSubject;
        string newBodyText = allMessages[msgID].NewMessageData.messageBody;
        
        GameObject newSubject = Instantiate(messageSubjectPrefab, messageSubjectContainer);
        newSubject.GetComponentInChildren<TMP_Text>().text = newSubjectText;
        
        newSubject.GetComponentInChildren<MessageInteractor>().InstantiateMessage(msgID);

        LayoutRebuilder.ForceRebuildLayoutImmediate(newSubject.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    public void OpenMessageWindow()
    {
        UpdateEntryCollider();
    }
    
   void UpdateEntryCollider()
    {
        foreach (Transform message in messageSubjectContainer)
        {
            Vector2 colliderSize = new Vector2(780f, message.GetComponent<RectTransform>().sizeDelta.y);
        
            message.GetComponent<BoxCollider2D>().size = colliderSize;
            message.GetComponent<BoxCollider2D>().offset = new Vector2(colliderSize.x /2f, 0f);
        }
        
    }

    public void ShowMessage(int msgSiblingIndex, int msgID)
    {
        foreach (Transform message in messageSubjectContainer)
        {
            message.GetComponent<MessageInteractor>().ActivateDeactivate(false);
        }
        
        messageSubjectContainer.transform.GetChild(msgSiblingIndex).GetComponent<MessageInteractor>().ActivateDeactivate(true);
    }
}
