using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [SerializeField] private ConnectionType connectionType;
    [SerializeField] private float glowIntensity = 3f;
    
    [Header("Hover Text")]
    [SerializeField] private string customHoverText = "Connection Port";
    
    private Renderer meshRenderer;
    private Collider pointCollider;
    private Material materialInstance;
    private Color baseColor;
    private bool isHighlighted = false;
    private bool isInteractable = true;
    
    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        pointCollider = GetComponent<Collider>();
        
        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
            
            if (connectionType != null)
            {
                baseColor = connectionType.typeColor;
                materialInstance.color = baseColor;
            }
            else
            {
                baseColor = materialInstance.color;
            }
            
            materialInstance.EnableKeyword("_EMISSION");
            materialInstance.SetColor("_EmissionColor", Color.black);
        }
    }
    
    void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
    
    public void Highlight(bool highlight)
    {
        if (!isInteractable || meshRenderer == null || isHighlighted == highlight)
            return;
        
        isHighlighted = highlight;
        
        if (highlight)
        {
            Color glowColor = baseColor * glowIntensity;
            materialInstance.SetColor("_EmissionColor", glowColor);
        }
        else
        {
            materialInstance.SetColor("_EmissionColor", Color.black);
        }
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    public ConnectionType GetConnectionType()
    {
        return connectionType;
    }
    
    public bool IsInteractable()
    {
        return isInteractable;
    }
    
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        
        if (pointCollider != null)
            pointCollider.enabled = interactable;
        
        if (!interactable && isHighlighted)
        {
            isHighlighted = false;
            materialInstance.SetColor("_EmissionColor", Color.black);
        }
    }
    
    // Get custom hover text
    public string GetHoverText()
    {
        return customHoverText;
    }
}
