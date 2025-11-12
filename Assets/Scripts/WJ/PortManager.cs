using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ConnectionSet
{
    public string setName = "New Connection";
    public ConnectionType connectionType;
    public ConnectionPoint portA;
    public ConnectionPoint portB;
    public GameObject linePrefab;
    
    [HideInInspector] public GameObject activeLineInstance;
    [HideInInspector] public bool isCompleted = false;
    
    public bool IsValid()
    {
        return portA != null && portB != null && connectionType != null;
    }
    
    public bool ContainsPort(ConnectionPoint port)
    {
        return portA == port || portB == port;
    }
}

public class PortManager : MonoBehaviour
{
    [SerializeField] private List<ConnectionSet> connectionSets = new List<ConnectionSet>();
    [SerializeField] private GameObject defaultLinePrefab;
    
    private static PortManager instance;
    
    public static PortManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PortManager>();
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    
    public ConnectionSet GetSetForPort(ConnectionPoint port)
    {
        foreach (ConnectionSet set in connectionSets)
        {
            if (set.ContainsPort(port))
                return set;
        }
        return null;
    }
    
    public bool CanConnect(ConnectionPoint portA, ConnectionPoint portB)
    {
        foreach (ConnectionSet set in connectionSets)
        {
            if ((set.portA == portA && set.portB == portB) ||
                (set.portA == portB && set.portB == portA))
            {
                return !set.isCompleted;
            }
        }
        return false;
    }
    
    public void CompleteConnection(ConnectionPoint portA, ConnectionPoint portB, GameObject lineInstance)
    {
        foreach (ConnectionSet set in connectionSets)
        {
            if ((set.portA == portA && set.portB == portB) ||
                (set.portA == portB && set.portB == portA))
            {
                set.isCompleted = true;
                set.activeLineInstance = lineInstance;
                
                if (set.portA != null)
                    set.portA.SetInteractable(false);
                if (set.portB != null)
                    set.portB.SetInteractable(false);
                
                Debug.Log("Connection completed: " + set.setName);
                CheckAllComplete();
                return;
            }
        }
    }
    
    public GameObject GetLinePrefab(ConnectionSet set)
    {
        if (set != null && set.linePrefab != null)
            return set.linePrefab;
        return defaultLinePrefab;
    }
    
    private void CheckAllComplete()
    {
        foreach (ConnectionSet set in connectionSets)
        {
            if (!set.isCompleted)
                return;
        }
        Debug.Log("ALL CONNECTIONS COMPLETE!");
    }
    
    [ContextMenu("Reset All Connections")]
    public void ResetAllConnections()
    {
        foreach (ConnectionSet set in connectionSets)
        {
            if (set.activeLineInstance != null)
                Destroy(set.activeLineInstance);
            
            set.isCompleted = false;
            set.activeLineInstance = null;
            
            if (set.portA != null)
                set.portA.SetInteractable(true);
            if (set.portB != null)
                set.portB.SetInteractable(true);
        }
        Debug.Log("All connections reset");
    }
}
