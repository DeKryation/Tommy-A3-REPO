using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DoorMashShoveAngleQTE : MonoBehaviour
{
    [Header("Door Hinge")]
    [SerializeField] private Transform doorTransform;   // assign hinge/door transform (auto = this)
    [SerializeField] private float closedAngle = 0f;    // door fully shut (local Y)
    [SerializeField] private float maxOpenAngle = 90f;  // fail if we hit this (local Y)

    [Header("Start State")]
    [SerializeField] private float startAjarAngle = 15f; // starts slightly open

    [Header("Forces (Tug of War)")]
    [SerializeField] private float enemyOpenSpeed = 25f;   // deg/sec pushing the door open
    [SerializeField] private float pressCloseDegrees = 3f; // each E press closes by this much
    [SerializeField] private float doorSmoothing = 12f;    // visual smoothing

    [Header("Win/Lose")]
    [SerializeField] private float attackDuration = 4f; // hold out for this long
    [SerializeField] private bool autoStartOnAwake = true;

    [Header("Snap Shut on Success")]
    [SerializeField] private bool snapOnSuccess = true;
    [SerializeField] private float snapSpeedDegPerSec = 540f; // how fast it slams shut
    [SerializeField] private bool lockClosedAfterSuccess = true;

    [Header("UI Prompt")]
    [SerializeField] private GameObject mashPrompt;   // assign MashEPrompt (the TMP text or image)
    [SerializeField] private CanvasGroup mashPromptCg; // optional: for quick fade

    // runtime
    private float _currentAngle;
    private float _targetAngle;
    private float _endTime;
    private bool _active;
    private bool _failed;
    private bool _snappingClosed;
    private bool _lockedClosed;

    private void Awake()
    {
        if (!doorTransform) doorTransform = transform;
    }

    private void Start()
    {
        SetAngle(Mathf.Clamp(startAjarAngle, closedAngle, maxOpenAngle));
        if (autoStartOnAwake) StartQTE(attackDuration);
    }

    private void Update()
    {
        // If locked closed (after snapping), do nothing
        if (_lockedClosed) return;

        // Keep opening to max after fail
        if (_failed)
        {
            LerpDoorTowards(maxOpenAngle);
            return;
        }

        // Run snap animation after success
        if (_snappingClosed)
        {
            // Move quickly toward fully closed
            _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, closedAngle, snapSpeedDegPerSec * Time.deltaTime);
            ApplyAngle(_currentAngle);

            // Stop snapping when shut
            if (Mathf.Abs(Mathf.DeltaAngle(_currentAngle, closedAngle)) < 0.1f)
            {
                ApplyAngle(closedAngle);
                _snappingClosed = false;
                if (lockClosedAfterSuccess) _lockedClosed = true;
            }
            return;
        }

        if (!_active) return;

        // Enemy constantly pushes door open
        _targetAngle = Mathf.Clamp(_targetAngle + enemyOpenSpeed * Time.deltaTime, closedAngle, maxOpenAngle);

        // Player mashes E to nudge closed
        bool ePressed = Input.GetKeyDown(KeyCode.E);
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
            ePressed |= Keyboard.current.eKey.wasPressedThisFrame;
#endif
        if (ePressed)
            _targetAngle = Mathf.Clamp(_targetAngle - pressCloseDegrees, closedAngle, maxOpenAngle);

        // Apply smoothing so it feels physical
        LerpDoorTowards(_targetAngle);

        // Fail if fully open
        if (ApproximatelyGE(_currentAngle, maxOpenAngle, 0.25f))
            Fail();

        // Win if timer done
        if (Time.time >= _endTime)
            Succeed();
    }

    // --- Public API ---
    public void StartQTE(float duration)
    {
        _active = true;
        _failed = false;
        _snappingClosed = false;
        _lockedClosed = false;

        _endTime = Time.time + Mathf.Max(0.1f, duration);
        _currentAngle = Mathf.Clamp(startAjarAngle, closedAngle, maxOpenAngle);
        _targetAngle = _currentAngle;
        ApplyAngle(_currentAngle);

        ShowPrompt(true);   // <—— show “PRESS E”
    }

    public void CancelQTE() => _active = false;

    // --- Internals ---
    private void LerpDoorTowards(float target)
    {
        _currentAngle = Mathf.LerpAngle(_currentAngle, target, 1f - Mathf.Exp(-doorSmoothing * Time.deltaTime));
        ApplyAngle(_currentAngle);
    }

    private void ApplyAngle(float y)
    {
        var e = doorTransform.localEulerAngles;
        e.y = y;
        doorTransform.localEulerAngles = e;
    }

    private void SetAngle(float y)
    {
        _currentAngle = y;
        _targetAngle = y;
        ApplyAngle(y);
    }

    private void Fail()
    {
        _active = false;
        _failed = true;
        ShowPrompt(false);  // <—— hide on fail
    }

    private void Succeed()
    {
        _active = false;
        ShowPrompt(false);  // <—— hide on success

        if (snapOnSuccess)
        {
            _snappingClosed = true;
            _targetAngle = closedAngle;
        }
        else
        {
            _targetAngle = closedAngle;
        }
    }

    // helper
    private static bool ApproximatelyGE(float a, float b, float eps) => a > b - eps;

    private void ShowPrompt(bool show)
    {
        if (!mashPrompt) return;
        mashPrompt.SetActive(show);
        if (mashPromptCg) mashPromptCg.alpha = show ? 1f : 0f;
    }
}
