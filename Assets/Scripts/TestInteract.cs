using UnityEngine;

public class TestInteract : InteractScript
{
    ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
    }
    public override void DoOnInteract()
    {
        Debug.Log("yes this works");
        particle.Play();
    }

    public override void DoOnRaycastHit()
    {
        Debug.Log("Raycast hit");
    }
}
