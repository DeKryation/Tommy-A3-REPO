using UnityEngine;

public class SafeDoor : MonoBehaviour
{
    [Header("Safe Hinge")]
    [SerializeField] private Transform doorTransform; // leave empty if this is on the rotating object
    [SerializeField] private float openAngle = 90f;   // local Y when fully open
    [SerializeField] private float openSpeed = 120f;  // degrees per second

    [Header("Timing")]
    [SerializeField] private float delayBeforeOpen = 0f; // seconds

    private bool _opening;
    private float _startTime;

    private void Awake()
    {
        if (!doorTransform) doorTransform = transform;
    }

    private void OnEnable()
    {
        _startTime = Time.time + Mathf.Max(0f, delayBeforeOpen);
        _opening = true;
    }

    private void Update()
    {
        if (!_opening || Time.time < _startTime) return;

        Vector3 e = doorTransform.localEulerAngles;
        float y = Mathf.MoveTowardsAngle(e.y, openAngle, openSpeed * Time.deltaTime);
        e.y = y;
        doorTransform.localEulerAngles = e;

        if (Mathf.Abs(Mathf.DeltaAngle(y, openAngle)) < 0.1f)
            _opening = false; // safety net if both conditions arent met
    }
}
