using UnityEngine;

public class LinePuller : MonoBehaviour
{
    [SerializeField] private LayerMask connectionPointLayer;
    [SerializeField] private float maxRaycastDistance = 10f;
    [SerializeField] private float minStretchDistance = 2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera playerCamera;
    
    [Header("Audio")]
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioSource audioSource;
    
    private enum LineState { Idle, Pulling }
    private LineState currentState = LineState.Idle;
    
    private GameObject activeLine;
    private LineRenderer activeLineRenderer;
    private ConnectionPoint startPoint;
    private ConnectionPoint hoveredPoint;
    
    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    
    void Update()
    {
        UpdateRaycast();
        
        if (currentState == LineState.Pulling)
        {
            UpdatePullingLine();
            
            if (Input.GetMouseButtonDown(1))
            {
                CancelPulling();
            }
        }
    }
    
    void UpdateRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        ConnectionPoint newHovered = null;
        
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, connectionPointLayer))
        {
            ConnectionPoint point = hit.collider.GetComponent<ConnectionPoint>();
            
            if (point != null && point.IsInteractable())
            {
                newHovered = point;
            }
        }
        
        if (newHovered != hoveredPoint)
        {
            if (hoveredPoint != null)
                hoveredPoint.Highlight(false);
            
            hoveredPoint = newHovered;
            
            if (hoveredPoint != null)
            {
                bool shouldHighlight = currentState == LineState.Idle || 
                                       (startPoint != null && PortManager.Instance != null && 
                                        PortManager.Instance.CanConnect(startPoint, hoveredPoint));
                
                if (shouldHighlight)
                    hoveredPoint.Highlight(true);
            }
        }
    }
    
    public bool TryHandleInteraction()
    {
        UpdateRaycast();
        
        if (currentState == LineState.Idle && hoveredPoint != null)
        {
            TryStartPulling();
            return true;
        }
        else if (currentState == LineState.Pulling)
        {
            TryCompletePulling();
            return true;
        }
        
        return false;
    }
    
    void TryStartPulling()
    {
        if (hoveredPoint == null || !hoveredPoint.IsInteractable())
            return;
        
        if (PortManager.Instance == null)
        {
            Debug.LogError("PortManager not found!");
            return;
        }
        
        ConnectionSet set = PortManager.Instance.GetSetForPort(hoveredPoint);
        if (set == null)
        {
            ShowError("This port is not part of any connection");
            return;
        }
        
        if (set.isCompleted)
        {
            ShowError("This connection is already completed");
            return;
        }
        
        ConnectionType connectionType = hoveredPoint.GetConnectionType();
        
        startPoint = hoveredPoint;
        currentState = LineState.Pulling;
        
        GameObject linePrefab = PortManager.Instance.GetLinePrefab(set);
        activeLine = Instantiate(linePrefab);
        activeLineRenderer = activeLine.GetComponent<LineRenderer>();
        
        if (activeLineRenderer != null)
        {
            activeLineRenderer.positionCount = 2;
            
            if (connectionType != null && connectionType.lineMaterial != null)
            {
                activeLineRenderer.material = connectionType.lineMaterial;
            }
            else if (connectionType != null)
            {
                activeLineRenderer.startColor = connectionType.typeColor;
                activeLineRenderer.endColor = connectionType.typeColor;
            }
        }
        
        UpdateLineTransform(startPoint.GetPosition(), playerTransform.position);
    }
    
    void TryCompletePulling()
    {
        if (hoveredPoint == null)
        {
            return; // Just keep pulling
        }
        
        if (PortManager.Instance == null)
        {
            return;
        }
        
        // Check if wrong color - DON'T cancel line
        if (!PortManager.Instance.CanConnect(startPoint, hoveredPoint))
        {
            ShowError("Wrong connection! Colors must match");
            return; // Keep line active
        }
        
        float distance = Vector3.Distance(startPoint.GetPosition(), hoveredPoint.GetPosition());
        
        if (distance < minStretchDistance)
        {
            return; // Just keep pulling
        }
        
        UpdateLineTransform(startPoint.GetPosition(), hoveredPoint.GetPosition());
        
        PortManager.Instance.CompleteConnection(startPoint, hoveredPoint, activeLine);
        
        currentState = LineState.Idle;
        startPoint = null;
        activeLine = null;
        activeLineRenderer = null;
    }
    
    void CancelPulling()
    {
        if (activeLine != null)
            Destroy(activeLine);
        
        currentState = LineState.Idle;
        startPoint = null;
        activeLine = null;
        activeLineRenderer = null;
    }
    
    void UpdatePullingLine()
    {
        if (activeLine != null)
            UpdateLineTransform(startPoint.GetPosition(), playerTransform.position);
    }
    
    void UpdateLineTransform(Vector3 start, Vector3 end)
    {
        if (activeLineRenderer != null)
        {
            activeLineRenderer.SetPosition(0, start);
            activeLineRenderer.SetPosition(1, end);
        }
    }
    
    void ShowError(string message)
    {
        if (errorSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
        
        InteractObject interactObject = GetComponent<InteractObject>();
        if (interactObject != null)
        {
            interactObject.ShowMessage(message);
        }
        
        Debug.Log(message);
    }
}
