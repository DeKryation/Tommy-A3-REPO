using System;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    HoverPort,
    WrongConnection,
    ConnectionSuccess,
    AllComplete,
    HoverInteractable,
    InteractionSuccess
}

[Serializable]
public class MessageEntry
{
    public MessageType id;
    [TextArea(2, 4)]
    public string messageText;
}

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }
    
    [Header("Message Library")]
    [SerializeField] private List<MessageEntry> messageLibrary = new List<MessageEntry>();
    
    private Dictionary<MessageType, string> _messageLookup;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        BuildLookup();
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void BuildLookup()
    {
        _messageLookup = new Dictionary<MessageType, string>();
        foreach (var entry in messageLibrary)
        {
            if (!_messageLookup.ContainsKey(entry.id))
            {
                _messageLookup.Add(entry.id, entry.messageText);
            }
            else
            {
                Debug.LogWarning($"MessageManager: Duplicate MessageType: {entry.id}");
            }
        }
    }
    
    public string GetMessage(MessageType id)
    {
        if (_messageLookup == null)
            BuildLookup();
        
        if (_messageLookup.TryGetValue(id, out string message))
        {
            return message;
        }
        
        Debug.LogWarning($"MessageManager: Message not found for {id}");
        return $"[Missing: {id}]";
    }
}
