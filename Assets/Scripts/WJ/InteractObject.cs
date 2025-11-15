using UnityEngine;
using TMPro;
using System.Collections;

public class InteractObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 5f;
    public LayerMask interactionMask;
    
    [Header("Highlight Settings")]
    [SerializeField] private Material highlightMaterial;
    private Material originalMaterial;
    private GameObject currentLookTarget;
    private bool isHighlighted = false;
    
    [Header("UI Text Settings")]
    [SerializeField] private TextMeshProUGUI hoverText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;
    [SerializeField] private float fadeSpeed = 2f;
    
    [Header("Connection Messages")]
    [SerializeField] private string hoverPortMessage = "Connection Port";
    [SerializeField] private string wrongConnectionMessage = "Wrong connection! Colors must match";
    [SerializeField] private string allConnectionsCompleteMessage = "All ports connected! Restoring lights...";
    
    private Coroutine messageFadeCoroutine;
    
    [Header("Script References")]
    [SerializeField] private LinePuller linePuller;
    [SerializeField] private AudioManager audioManager;

    void Awake()
    {
        if (linePuller == null)
            linePuller = GetComponent<LinePuller>();
        
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        
        // Hide texts at start
        if (hoverText != null)
        {
            hoverText.text = "";
            hoverText.gameObject.SetActive(false);
        }
        
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
            GameObject hitObject = hit.transform.gameObject;
            
            if (currentLookTarget != hitObject)
            {
                ClearHighlight();
                currentLookTarget = hitObject;
                
                // Check for ConnectionPoint
                ConnectionPoint connectionPoint = currentLookTarget.GetComponent<ConnectionPoint>();
                if (connectionPoint != null && connectionPoint.IsInteractable())
                {
                    connectionPoint.Highlight(true);
                    ShowHoverText(hoverPortMessage);
                    
                    if (audioManager != null)
                    {
                        audioManager.PlaySFX(GameSFX.HoverPort);
                    }
                }
                // Check for BreakerInteract
                else
                {
                    BreakerInteract breakerInteract = currentLookTarget.GetComponent<BreakerInteract>();
                    if (breakerInteract != null)
                    {
                        ShowHoverText(breakerInteract.GetHoverText());
                        HighlightWithMaterial(currentLookTarget, true);
                    }
                    // Check for DoorInteract
                    else
                    {
                        DoorInteract doorInteract = currentLookTarget.GetComponent<DoorInteract>();
                        if (doorInteract != null)
                        {
                            ShowHoverText(doorInteract.GetHoverText());
                            HighlightWithMaterial(currentLookTarget, true);
                        }
                        // Check for items
                        else
                        {
                            InteractScript interactScript = currentLookTarget.GetComponent<InteractScript>();
                            if (interactScript != null)
                            {
                                ItemData itemData = currentLookTarget.GetComponent<ItemData>();
                                if (itemData != null)
                                {
                                    ShowHoverText(itemData.objectName);
                                }
                                else
                                {
                                    ShowHoverText("Press [LMB] to interact");
                                }
                                
                                HighlightWithMaterial(currentLookTarget, true);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            ClearHighlight();
            HideHoverText();
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Priority 1: LinePuller for connection points
            if (linePuller != null && linePuller.TryHandleInteraction())
            {
                return;
            }
            
            // Priority 2: InteractScript (includes InteractableItem for items)
            Camera cam = Camera.main;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactRange, interactionMask))
            {
                InteractScript interactScript = hit.transform.GetComponent<InteractScript>();
                if (interactScript != null)
                {
                    interactScript.DoOnInteract();
                    return;
                }
            }
            
            // Fallback: ItemManager
            ItemManager.GetInstance().GetItemInfo(ItemManager.GetInstance().selectedItemID);
        }
    }

    private void HighlightWithMaterial(GameObject target, bool highlight)
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
        if (currentLookTarget != null)
        {
            ConnectionPoint connectionPoint = currentLookTarget.GetComponent<ConnectionPoint>();
            if (connectionPoint != null)
            {
                connectionPoint.Highlight(false);
            }
            else if (isHighlighted)
            {
                Renderer targetRenderer = currentLookTarget.GetComponent<Renderer>();
                if (targetRenderer != null && originalMaterial != null)
                {
                    targetRenderer.sharedMaterial = originalMaterial;
                }
            }
            
            isHighlighted = false;
        }
        currentLookTarget = null;
    }
    
    private void ShowHoverText(string text)
    {
        if (hoverText != null)
        {
            hoverText.gameObject.SetActive(true);
            hoverText.text = text;
        }
    }
    
    private void HideHoverText()
    {
        if (hoverText != null)
        {
            hoverText.text = "";
            hoverText.gameObject.SetActive(false);
        }
    }
    
    // Public method for connection messages
    public void ShowConnectionMessage(string messageType, bool isError = false)
    {
        string message = "";
        
        switch (messageType)
        {
            case "wrong":
                message = wrongConnectionMessage;
                break;
            case "complete":
                message = allConnectionsCompleteMessage;
                break;
        }
        
        ShowMessage(message, isError);
    }
    
    public void ShowMessage(string message, bool isError = false)
    {
        if (messageText == null || string.IsNullOrEmpty(message))
            return;
        
        if (messageFadeCoroutine != null)
            StopCoroutine(messageFadeCoroutine);
        
        messageText.text = message;
        messageFadeCoroutine = StartCoroutine(FadeMessageInAndOut());
        
        if (isError && audioManager != null)
        {
            audioManager.PlaySFX(GameSFX.ConnectionError);
        }
    }
    
    private IEnumerator FadeMessageInAndOut()
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
        yield return new WaitForSeconds(messageDuration);
        
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
    
    public AudioManager GetAudioManager()
    {
        return audioManager;
    }
}
