using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    
    [Header("UI Message Settings")]
    [SerializeField] private Text messageText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeSpeed = 2f;
    private Coroutine currentFade;
    
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
            linePuller = GetComponent<LinePuller>();
        
        if (messageText != null)
        {
            Color color = messageText.color;
            color.a = 0;
            messageText.color = color;
        }
    }

    void Update()
    {
        HandleRaycastHighlight();
        HandleInteraction();
    }

    private void HandleRaycastHighlight()
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        
        bool hitSomething = Physics.Raycast(ray, out hit, interactRange, interactionMask);

        if (hitSomething)
        {
            if (currentLookTarget != hit.transform.gameObject)
            {
                ClearHighlight();
                currentLookTarget = hit.transform.gameObject;
                HighlightTarget(currentLookTarget, true);
                
                // Show hover text for interactable objects
                InteractScript interactScript = currentLookTarget.GetComponent<InteractScript>();
                if (interactScript != null)
                {
                    ShowMessage("Press [LMB] to interact");
                }
                
                // Show hover text for connection points
                ConnectionPoint connectionPoint = currentLookTarget.GetComponent<ConnectionPoint>();
                if (connectionPoint != null && connectionPoint.IsInteractable())
                {
                    ShowMessage("Connection Port");
                    if (GameAudioManager.Instance != null)
                    {
                        GameAudioManager.Instance.PlaySFX(GameSFX.HoverPort);
                    }
                }
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
                return;
            }
            
            // Priority 2: Handle general interactions
            Camera cam = Camera.main;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactRange, interactionMask))
            {
                // Try InteractScript
                InteractScript interactScript = hit.transform.GetComponent<InteractScript>();
                if (interactScript != null)
                {
                    interactScript.DoOnInteract();
                    ShowMessage("Interaction successful!");
                    return;
                }

                // Try ConnectionPoint (backup)
                ConnectionPoint connectionPoint = hit.transform.GetComponent<ConnectionPoint>();
                if (connectionPoint != null)
                {
                    Vector3 position = connectionPoint.GetPosition();
                    Debug.Log($"Tried to interact with connection point at {position}");
                    return;
                }
            }
            
            // Fallback: ItemManager
            ItemManager.GetInstance().GetItemInfo(ItemManager.GetInstance().selectedItemID);
        }
    }

    private void HighlightTarget(GameObject target, bool highlight)
    {
        if (target == null)
            return;

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
        if (currentLookTarget != null && isHighlighted)
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
            return;
            
        isHighlighted = highlight;
        meshRenderer.sharedMaterial = highlight ? highlightMaterial : originalMaterial;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    // Public method to show messages and play error sound
    public void ShowMessage(string message, bool isError = false)
    {
        if (messageText == null)
            return;
        
        if (currentFade != null)
            StopCoroutine(currentFade);
        
        messageText.text = message;
        currentFade = StartCoroutine(FadeInAndOut());
        
        // Play error sound if it's an error message
        if (isError && GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.PlaySFX(GameSFX.ConnectionError);
        }
    }
    
    private IEnumerator FadeInAndOut()
    {
        Color color = messageText.color;
        
        // Fade in
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(0, 1, elapsed / 0.5f);
            messageText.color = color;
            yield return null;
        }
        
        // Stay visible
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(1, 0, elapsed / 0.5f);
            messageText.color = color;
            yield return null;
        }
        
        color.a = 0;
        messageText.color = color;
    }
}
