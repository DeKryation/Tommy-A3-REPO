using UnityEngine;

public class BreakerInteract : InteractScript
{
    [Header("Breaker Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private bool isUnlocked = false;
    [SerializeField] private string hoverTextLocked = "Breaker is locked. Connect all ports first.";
    [SerializeField] private string hoverTextOff = "Press [LMB] to flip breaker ON";
    [SerializeField] private string hoverTextOn = "Breaker is ON";
    [SerializeField] private string activationMessage = "Power restored! Lights are on!";
    
    [Header("Door Rotation")]
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Renderer breakerRenderer;
    
    [Header("Audio")]
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private AudioClip doorSound;
    [SerializeField] private AudioClip lockedSound;
    
    private Quaternion doorClosedRotation;
    private Quaternion doorOpenRotation;
    private bool isDoorMoving = false;
    
    void Start()
    {
        UpdateVisuals();
        
        // Store door rotations
        if (doorTransform != null)
        {
            doorClosedRotation = doorTransform.rotation;
            doorOpenRotation = doorClosedRotation * Quaternion.Euler(0, openAngle, 0);
        }
    }
    
    void Update()
    {
        // Animate door rotation
        if (isDoorMoving && doorTransform != null)
        {
            doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, doorOpenRotation, Time.deltaTime * openSpeed);
            
            if (Quaternion.Angle(doorTransform.rotation, doorOpenRotation) < 0.1f)
            {
                doorTransform.rotation = doorOpenRotation;
                isDoorMoving = false;
            }
        }
    }
    
    public override void DoOnInteract()
    {
        // Check if locked
        if (!isUnlocked)
        {
            // Play locked sound
            if (lockedSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClip(lockedSound);
            }
            
            // Show locked message
            InteractObject interactObject = FindObjectOfType<InteractObject>();
            if (interactObject != null)
            {
                interactObject.ShowMessage("Connect all ports first!");
            }
            
            Debug.Log("Breaker is locked. Connect all ports first.");
            return;
        }
        
        if (isOn)
            return; // Already on
        
        isOn = true;
        
        // Play switch sound
        if (switchSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClip(switchSound);
        }
        
        // Update breaker visuals
        UpdateVisuals();
        
        // Start door rotation
        if (doorTransform != null)
        {
            isDoorMoving = true;
            
            if (doorSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClip(doorSound);
            }
        }
        
        // Show activation message
        InteractObject interactObject2 = FindObjectOfType<InteractObject>();
        if (interactObject2 != null)
        {
            interactObject2.ShowMessage(activationMessage);
        }
        
        // Turn on the lights!
        LightController lightController = FindObjectOfType<LightController>();
        if (lightController != null)
        {
            lightController.LightUpRoom();
        }
        
        Debug.Log("Breaker switched ON - Lights turning on!");
    }
    
    public string GetHoverText()
    {
        if (!isUnlocked)
            return hoverTextLocked;
        
        return isOn ? hoverTextOn : hoverTextOff;
    }
    
    public bool IsOn()
    {
        return isOn;
    }
    
    // Call this from PortManager when all connections complete
    public void UnlockBreaker()
    {
        isUnlocked = true;
        UpdateVisuals();
        Debug.Log("Breaker unlocked!");
    }
    
    private void UpdateVisuals()
    {
        if (breakerRenderer != null && onMaterial != null && offMaterial != null && lockedMaterial != null)
        {
            if (!isUnlocked)
            {
                breakerRenderer.material = lockedMaterial;
            }
            else
            {
                breakerRenderer.material = isOn ? onMaterial : offMaterial;
            }
        }
    }
}
