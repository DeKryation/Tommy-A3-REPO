using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [SerializeField] private ConnectionType connectionType;
    [SerializeField] private Material highlightMaterial;
    
    private Material originalMaterial;
    private Renderer meshRenderer;
    private Collider pointCollider;
    private bool isHighlighted = false;
    private bool isInteractable = true;
    
    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        pointCollider = GetComponent<Collider>();
        
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.sharedMaterial;
            
            if (connectionType != null)
            {
                meshRenderer.material.color = connectionType.typeColor;
            }
        }
    }
    
    public void Highlight(bool highlight)
    {
        if (!isInteractable || meshRenderer == null || isHighlighted == highlight)
            return;
        
        isHighlighted = highlight;
        meshRenderer.sharedMaterial = highlight ? highlightMaterial : originalMaterial;
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
            if (meshRenderer != null)
                meshRenderer.sharedMaterial = originalMaterial;
        }
    }
}
