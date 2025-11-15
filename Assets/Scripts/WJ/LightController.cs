using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Lighting Settings")]
    [SerializeField] private Light mainLight;
    [SerializeField] private float darkIntensity = 0.2f;
    [SerializeField] private float brightIntensity = 1.5f;
    [SerializeField] private float transitionSpeed = 1f;
    
    [Header("Optional: Emissive Materials")]
    [SerializeField] private Material[] emissiveMaterials;
    [SerializeField] private Color darkEmissionColor = Color.black;
    [SerializeField] private Color brightEmissionColor = Color.white;
    
    private bool isLit = false;
    private float targetIntensity;
    private float currentIntensity;
    
    void Start()
    {
        if (mainLight == null)
            mainLight = FindObjectOfType<Light>();
        
        currentIntensity = darkIntensity;
        targetIntensity = darkIntensity;
        
        if (mainLight != null)
            mainLight.intensity = darkIntensity;
        
        SetEmissionColor(darkEmissionColor);
    }
    
    void Update()
    {
        if (mainLight != null && Mathf.Abs(currentIntensity - targetIntensity) > 0.01f)
        {
            currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * transitionSpeed);
            mainLight.intensity = currentIntensity;
        }
    }
    
    public void LightUpRoom()
    {
        if (isLit) return;
        
        isLit = true;
        targetIntensity = brightIntensity;
        SetEmissionColor(brightEmissionColor);
        
        Debug.Log("Room lighting up!");
    }
    
    public void DarkenRoom()
    {
        isLit = false;
        targetIntensity = darkIntensity;
        SetEmissionColor(darkEmissionColor);
        
        Debug.Log("Room darkening");
    }
    
    private void SetEmissionColor(Color color)
    {
        if (emissiveMaterials == null || emissiveMaterials.Length == 0)
            return;
        
        foreach (Material mat in emissiveMaterials)
        {
            if (mat != null)
            {
                mat.SetColor("_EmissionColor", color * 2f);
            }
        }
    }
}
