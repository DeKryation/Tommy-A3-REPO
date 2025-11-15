using UnityEngine;

// Add this component to any object you want to be interactable
public class Interactable : MonoBehaviour
{
    [Header("Hover Text")]
    [SerializeField] private string hoverText = "Press [LMB] to interact";
    
    public string GetHoverText()
    {
        return hoverText;
    }
}
