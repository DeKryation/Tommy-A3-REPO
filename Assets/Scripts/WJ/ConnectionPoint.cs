using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    private Material originalMaterial;
    private Renderer meshRenderer;
    private bool isHighlighted = false;
    
    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
            originalMaterial = meshRenderer.sharedMaterial;
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
}