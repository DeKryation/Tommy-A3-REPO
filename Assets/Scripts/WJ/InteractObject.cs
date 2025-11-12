using UnityEngine;

public class InteractObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 5f;
    public LayerMask interactionMask;
    
    [Header("Highlight Settings")]
    [SerializeField] private Material highlightMaterial;
    private Material originalMaterial;
    private Renderer meshRenderer;
    private GameObject currentLookTarget;
    private bool isHighlighted = false;

    [Header("Script References")]
    [SerializeField] private LinePuller linePuller;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.sharedMaterial;
        }

        if (linePuller == null)
        {
            linePuller = GetComponent<LinePuller>();   
        }
    }

    void Update()
    {
        HandleRaycastHighlight();
        HandleInteraction();
    }

    private void HandleRaycastHighlight()
    {
        RaycastHit hit;
        
        // Use camera for raycasting instead of transform
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        
        bool hitSomething = Physics.Raycast(ray, out hit, interactRange, interactionMask);

        if (hitSomething)
        {
            if (currentLookTarget != hit.transform.gameObject)
            {
                ClearHighlight();
                currentLookTarget = hit.transform.gameObject;
                HighlightTarget(currentLookTarget, true);
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Priority 1: Let LinePuller try to handle the interaction first
            if (linePuller != null && linePuller.TryHandleInteraction())
            {
                Debug.Log("InteractObject: LinePuller handled the interaction");
                return;
            }
            
            // Priority 2: Handle general interactions
            Camera cam = Camera.main;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactRange, interactionMask))
            {
                Debug.Log($"InteractObject: Hit {hit.transform.name}"); // Debug what we hit
                
                // Try InteractScript
                InteractScript interactScript = hit.transform.GetComponent<InteractScript>();
                if (interactScript != null)
                {
                    interactScript.DoOnInteract();
                    Debug.Log("InteractObject: Interacted with InteractScript");
                    return;
                }

                // Try ConnectionPoint (backup)
                ConnectionPoint connectionPoint = hit.transform.GetComponent<ConnectionPoint>();
                if (connectionPoint != null)
                {
                    Vector3 position = connectionPoint.GetPosition();
                    Debug.Log($"InteractObject: Tried to interact with connection point at {position}");
                    return;
                }
            }
            else
            {
                Debug.Log("InteractObject: Raycast hit nothing");
            }
            
            // Fallback: ItemManager
            Debug.Log("InteractObject: No interaction found, using ItemManager");
            ItemManager.GetInstance().GetItemInfo(ItemManager.GetInstance().selectedItemID);
        }
    }

    private void HighlightTarget(GameObject target, bool highlight)
    {
        if (target == null)
        {
            return;
        }

        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer != null && highlightMaterial != null)
        {
            if (highlight && !isHighlighted)
            {
                originalMaterial = targetRenderer.sharedMaterial;
                targetRenderer.sharedMaterial = highlightMaterial;
                isHighlighted = true;
            }
        }

    }

    private void ClearHighlight()
    {
        if (currentLookTarget != null & isHighlighted)
        {
            Renderer targetRenderer = currentLookTarget.GetComponent<Renderer>();
            if (targetRenderer != null && originalMaterial != null)
            {
                targetRenderer.sharedMaterial = originalMaterial;
            }
            isHighlighted = false;
        }
        currentLookTarget = null;
    }

    public void Highlight(bool highlight)
    {
        if (meshRenderer == null || isHighlighted == highlight)
        {
            return;
        }
        isHighlighted = highlight;
        meshRenderer.sharedMaterial = highlight ? highlightMaterial : originalMaterial;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Alex's script
    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         RaycastHit hit;
    //         if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange, mask) && hit.transform.GetComponent<InteractScript>() != null)
    //         {
    //             hit.transform.GetComponent<InteractScript>().DoOnInteract();
    //         }
    //         else
    //         {
    //             ItemManager.GetInstance().GetItemInfo(ItemManager.GetInstance().selectedItemID);
    //         }
    //     }
    // }
}
