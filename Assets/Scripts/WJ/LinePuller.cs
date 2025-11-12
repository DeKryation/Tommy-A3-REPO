using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class LinePuller : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask connectionPointLayer;
    [SerializeField] private float maxRaycastDistance = 10f;
    
    [Header("Line Settings")]
    [SerializeField] private float minStretchDistance = 2f;
    [SerializeField] private GameObject linePrefab;
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera playerCamera;
    
    private enum LineState { Idle, Pulling }
    private LineState currentState = LineState.Idle;
    
    private GameObject activeLine;
    private ConnectionPoint startPoint;
    private ConnectionPoint hoveredPoint;
    
    private InputHandler crabMove;
    private InputAction interactAction;
    
    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }
    
    void Start()
    {
        if (playerTransform != null)
        {
            crabMove = playerTransform.GetComponent<InputHandler>();
            if (crabMove != null && crabMove.crabInput != null)
            {
                interactAction = crabMove.crabInput.Player.Interact;
                interactAction.performed += OnInteractPressed;
                Debug.Log("LinePuller: Successfully subscribed to Interact action");
            }
            else
            {
                Debug.LogError("LinePuller: CrabMove or crabInput is null!");
            }
        }
        else
        {
            Debug.LogError("LinePuller: Player Transform not assigned!");
        }
    }
    
    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPressed;
        }
    }
    
    void Update()
    {
        UpdateRaycast();
        
        if (currentState == LineState.Pulling)
            UpdatePullingLine();
    }
    
    void UpdateRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        ConnectionPoint newHovered = null;
        
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, connectionPointLayer))
        {
            newHovered = hit.collider.GetComponent<ConnectionPoint>();
        }
        
        if (newHovered != hoveredPoint)
        {
            if (hoveredPoint != null)
                hoveredPoint.Highlight(false);
            
            hoveredPoint = newHovered;
            
            if (hoveredPoint != null)
                hoveredPoint.Highlight(true);
        }
    }
    
    void OnInteractPressed(InputAction.CallbackContext context)
    {
        Debug.Log("LinePuller: E pressed. Current state: " + currentState + ", Hovered: " + (hoveredPoint != null));
        
        if (currentState == LineState.Idle)
            TryStartPulling();
        else if (currentState == LineState.Pulling)
            TryCompletePulling();
    }
    
    void TryStartPulling()
    {
        if (hoveredPoint == null)
        {
            Debug.Log("LinePuller: Cannot start - no hovered point");
            return;
        }
        
        if (linePrefab == null)
        {
            Debug.LogError("LinePuller: Line prefab not assigned!");
            return;
        }
        
        Debug.Log("LinePuller: Starting to pull line from " + hoveredPoint.name);
        
        startPoint = hoveredPoint;
        currentState = LineState.Pulling;
        
        activeLine = Instantiate(linePrefab);
        UpdateLineTransform(startPoint.GetPosition(), playerTransform.position);
    }
    
    void TryCompletePulling()
    {
        if (hoveredPoint == null)
        {
            CancelPulling();
            return;
        }
        
        float distance = Vector3.Distance(startPoint.GetPosition(), playerTransform.position);
        
        if (distance < minStretchDistance)
        {
            CancelPulling();
            return;
        }
        
        Debug.Log("LinePuller: Completing connection to " + hoveredPoint.name);
        
        UpdateLineTransform(startPoint.GetPosition(), hoveredPoint.GetPosition());
        
        currentState = LineState.Idle;
        startPoint = null;
        activeLine = null;
    }
    
    void CancelPulling()
    {
        Debug.Log("LinePuller: Cancelling pull");
        
        if (activeLine != null)
            Destroy(activeLine);
        
        currentState = LineState.Idle;
        startPoint = null;
        activeLine = null;
    }
    
    void UpdatePullingLine()
    {
        if (activeLine != null)
            UpdateLineTransform(startPoint.GetPosition(), playerTransform.position);
    }
    
    void UpdateLineTransform(Vector3 start, Vector3 end)
    {
        Vector3 midpoint = (start + end) * 0.5f;
        activeLine.transform.position = midpoint;
        
        Vector3 direction = end - start;
        activeLine.transform.rotation = Quaternion.LookRotation(direction);
        
        float distance = direction.magnitude;
        activeLine.transform.localScale = new Vector3(
            activeLine.transform.localScale.x,
            activeLine.transform.localScale.y,
            distance
        );
    }
    
    public void ClearAllLines()
    {
        if (activeLine != null)
            Destroy(activeLine);
        
        currentState = LineState.Idle;
        startPoint = null;
        activeLine = null;
    }
}