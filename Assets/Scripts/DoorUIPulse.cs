using UnityEngine;

public class UIPulse : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 0.08f;
    [SerializeField] private float speed = 6f;
    private Vector3 _base;

    private void OnEnable() { _base = transform.localScale; }
    private void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) * 0.5f + 0.5f) * scaleAmount;
        transform.localScale = _base * (1f + t);
    }
}
