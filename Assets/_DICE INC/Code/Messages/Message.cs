using DICEINC.Global;
using UnityEngine;

[CreateAssetMenu(fileName = "Message", menuName = "DICE INC/Message")]
public class Message : ScriptableObject
{
    [SerializeField] private MessageData _messageData;
    
    public MessageData NewMessageData => _messageData;
}
