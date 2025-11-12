using UnityEngine;

public class LinePuller : MonoBehaviour
{
    [SerializeField] private LayerMask connectionPointLayer;
    [SerializeField] private float maxRaycastDistance = 10f;
    [SerializeField] private float minStretchDistance = 2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera playerCamera;
    
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
            Debug.Log("This port is not part of any connection set");
            return;
        }
        
        if (set.isCompleted)
        {
            Debug.Log("This connection is already completed");
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
            
            if (connectionType.lineMaterial != null)
            {
                activeLineRenderer.material = connectionType.lineMaterial;
            }
            else
            {
                activeLineRenderer.startColor = connectionType.typeColor;
                activeLineRenderer.endColor = connectionType.typeColor;
            }
        }
        
        UpdateLineTransform(startPoint.GetPosition(), playerTransform.position);
    }
    
    void TryCompletePulling()
    {
        if (hoveredPoint == null || PortManager.Instance == null || 
            !PortManager.Instance.CanConnect(startPoint, hoveredPoint))
        {
            CancelPulling();
            return;
        }
        
        float distance = Vector3.Distance(startPoint.GetPosition(), hoveredPoint.GetPosition());
        
        if (distance < minStretchDistance)
        {
            CancelPulling();
            return;
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
}
