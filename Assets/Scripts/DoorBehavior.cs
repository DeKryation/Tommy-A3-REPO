using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DoorBehavior : MonoBehaviour
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
    [SerializeField] private float attackDuration = 4f;
    [SerializeField] private bool autoStartOnAwake = false; // do not auto start by default

    [Header("Snap Shut on Success")]
    [SerializeField] private bool snapOnSuccess = true;
    [SerializeField] private float snapSpeedDegPerSec = 540f;
    [SerializeField] private bool lockClosedAfterSuccess = true;

    [Header("UI Prompt")]
    [SerializeField] private GameObject mashPrompt;
    [SerializeField] private CanvasGroup mashPromptCg;

    [Header("Camera Turn On Success")]
    [SerializeField] private Transform cameraToTurn;
    [SerializeField] private float camTurnDegrees = 180f;
    [SerializeField] private float camTurnDuration = 0.6f;
    [SerializeField] private bool camTurnInLocalSpace = false;

    [Header("Then Move Forward")]
    [SerializeField] private float moveForwardDistance = 20f;
    [SerializeField] private float moveForwardDuration = 0.8f;
    [SerializeField] private AnimationCurve moveEase = null;

    [Header("Jump-Out Effect")]
    [SerializeField] private float jumpDuration = 0.7f;
    [SerializeField] private float jumpUpHeight = 2.2f;
    [SerializeField] private float jumpForwardExtra = 6f;
    [SerializeField] private float cameraTiltDegrees = 10f;
    [SerializeField] private float fovKick = 15f;
    [SerializeField] private float fovRecoverTime = 0.35f;

    [Header("Controller Interop (optional)")]
    [SerializeField] private MonoBehaviour controllerToDisable;
    [SerializeField] private bool disableControllerDuringSequence = true;

    [Header("On Fail: Reset & Retry")]
    [SerializeField] private bool autoRetryOnFail = true;
    [SerializeField] private float failResetDelay = 0.25f;
    [SerializeField] private float camResetDuration = 0.35f;
    [SerializeField] private float retryDelayAfterReset = 0.35f;

    [Header("Key Requirement")]
    [SerializeField] private int requiredItemID = 4;       // key item id
    [SerializeField] private string playerTag = "Player";  // tag on the player object

    // runtime
    private float _currentAngle;
    private float _targetAngle;
    private float _endTime;
    private bool _active;
    private bool _failed;
    private bool _snappingClosed;
    private bool _lockedClosed;

    private Camera _cam;
    private Coroutine _sequenceCo;

    // per-attempt camera baseline
    private bool _hasCamBaseline;
    private Vector3 _camBasePos;
    private Quaternion _camBaseRotLocal;
    private Quaternion _camBaseRotWorld;
    private float _camBaseFov;

    private void Awake()
    {
        if (!doorTransform) doorTransform = transform;

        if (!cameraToTurn && Camera.main) cameraToTurn = Camera.main.transform;

        if (cameraToTurn)
        {
            _cam = cameraToTurn.GetComponent<Camera>();
            if (!_cam) _cam = cameraToTurn.GetComponentInChildren<Camera>();
        }
    }

    private void Start()
    {
        // start slightly open
        SetAngle(Mathf.Clamp(startAjarAngle, closedAngle, maxOpenAngle));

        // optional auto start (leave false for your use case)
        if (autoStartOnAwake)
        {
            StartQTE(attackDuration);
        }
    }

    // Only start the scenario when player with key collides with door
    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag(playerTag))
            return;

        // Do not restart if already active or finished
        if (_active || _snappingClosed || _lockedClosed)
            return;

        ItemManager itemMgr = ItemManager.GetInstance();
        if (itemMgr != null && itemMgr.selectedItemID == requiredItemID)
        {
            // Player has the correct key selected
            StartQTE(attackDuration);
        }
        else
        {
            itemMgr.StartDialogue("The door is locked.");
            Debug.Log("Door is locked. You need the key with item ID " + requiredItemID + " selected.");
        }
    }

    private void Update()
    {
        if (_lockedClosed) return;

        if (_failed)
        {
            // Fail reset is handled by coroutine
            return;
        }

        if (_snappingClosed)
        {
            _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, closedAngle, snapSpeedDegPerSec * Time.deltaTime);
            ApplyAngle(_currentAngle);
            if (Mathf.Abs(Mathf.DeltaAngle(_currentAngle, closedAngle)) < 0.1f)
            {
                ApplyAngle(closedAngle);
                _snappingClosed = false;
                if (lockClosedAfterSuccess) _lockedClosed = true;
            }
            return;
        }

        if (!_active) return;

        // Enemy opens; player mashes to close
        _targetAngle = Mathf.Clamp(_targetAngle + enemyOpenSpeed * Time.deltaTime, closedAngle, maxOpenAngle);

        bool ePressed = Input.GetKeyDown(KeyCode.E);
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
            ePressed |= Keyboard.current.eKey.wasPressedThisFrame;
#endif
        if (ePressed)
            _targetAngle = Mathf.Clamp(_targetAngle - pressCloseDegrees, closedAngle, maxOpenAngle);

        LerpDoorTowards(_targetAngle);

        if (ApproximatelyGE(_currentAngle, maxOpenAngle, 0.25f))
            Fail();
        if (Time.time >= _endTime)
            Succeed();
    }

    // Public API
    public void StartQTE(float duration)
    {
        _active = true;
        _failed = false;
        _snappingClosed = false;
        _lockedClosed = false;

        // capture camera baseline for this attempt
        if (cameraToTurn)
        {
            _hasCamBaseline = true;
            _camBasePos = cameraToTurn.position;
            _camBaseRotLocal = cameraToTurn.localRotation;
            _camBaseRotWorld = cameraToTurn.rotation;
            if (_cam) _camBaseFov = _cam.fieldOfView;
        }

        _endTime = Time.time + Mathf.Max(0.1f, duration);
        _currentAngle = Mathf.Clamp(startAjarAngle, closedAngle, maxOpenAngle);
        _targetAngle = _currentAngle;
        ApplyAngle(_currentAngle);

        ShowPrompt(true);
    }

    public void CancelQTE()
    {
        _active = false;
    }

    // Internals
    private void LerpDoorTowards(float target)
    {
        _currentAngle = Mathf.LerpAngle(
            _currentAngle,
            target,
            1f - Mathf.Exp(-doorSmoothing * Time.deltaTime));
        ApplyAngle(_currentAngle);
    }

    private void ApplyAngle(float y)
    {
        Vector3 e = doorTransform.localEulerAngles;
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
        if (_failed) return;
        _active = false;
        _failed = true;
        ShowPrompt(false);

        if (_sequenceCo != null)
        {
            StopCoroutine(_sequenceCo);
            _sequenceCo = null;
        }

        if (autoRetryOnFail)
            _sequenceCo = StartCoroutine(FailResetAndRetry());
    }

    private void Succeed()
    {
        _active = false;
        ShowPrompt(false);

        if (snapOnSuccess)
        {
            _snappingClosed = true;
            _targetAngle = closedAngle;
        }
        else
        {
            _targetAngle = closedAngle;
        }

        if (cameraToTurn && camTurnDegrees != 0f && camTurnDuration > 0f)
            _sequenceCo = StartCoroutine(TurnMoveJumpSequence());
    }

    private static bool ApproximatelyGE(float a, float b, float eps)
    {
        return a > b - eps;
    }

    private void ShowPrompt(bool show)
    {
        if (!mashPrompt) return;
        mashPrompt.SetActive(show);
        if (mashPromptCg) mashPromptCg.alpha = show ? 1f : 0f;
    }

    // Camera sequence (success path)
    private System.Collections.IEnumerator TurnMoveJumpSequence()
    {
        if (disableControllerDuringSequence && controllerToDisable)
            controllerToDisable.enabled = false;

        yield return StartCoroutine(TurnCameraCoroutine());

        if (moveForwardDistance != 0f && moveForwardDuration > 0f)
            yield return StartCoroutine(MoveForwardCoroutine(moveForwardDistance, moveForwardDuration));

        if (jumpDuration > 0f)
            yield return StartCoroutine(JumpOutCoroutine());

        if (disableControllerDuringSequence && controllerToDisable)
            controllerToDisable.enabled = true;

        _sequenceCo = null;
    }

    private System.Collections.IEnumerator TurnCameraCoroutine()
    {
        float t = 0f;

        Quaternion startRot = camTurnInLocalSpace ? cameraToTurn.localRotation : cameraToTurn.rotation;
        Vector3 axis = Vector3.up;
        Quaternion delta = Quaternion.AngleAxis(camTurnDegrees, axis);
        Quaternion targetRot = camTurnInLocalSpace ? (startRot * delta) : (delta * startRot);

        while (t < camTurnDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / camTurnDuration);
            Quaternion rot = Quaternion.Slerp(startRot, targetRot, u);

            if (camTurnInLocalSpace) cameraToTurn.localRotation = rot;
            else cameraToTurn.rotation = rot;

            yield return null;
        }

        if (camTurnInLocalSpace) cameraToTurn.localRotation = targetRot;
        else cameraToTurn.rotation = targetRot;
    }

    private System.Collections.IEnumerator MoveForwardCoroutine(float distance, float duration)
    {
        float t = 0f;
        Vector3 startPos = cameraToTurn.position;
        Vector3 dir = cameraToTurn.forward;
        Vector3 endPos = startPos + dir * distance;

        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            float s = moveEase != null ? moveEase.Evaluate(u) : (u * u * (3f - 2f * u)); // smoothstep
            cameraToTurn.position = Vector3.LerpUnclamped(startPos, endPos, s);
            yield return null;
        }

        cameraToTurn.position = endPos;
    }

    private System.Collections.IEnumerator JumpOutCoroutine()
    {
        float t = 0f;

        Vector3 startPos = cameraToTurn.position;
        Vector3 fwd = cameraToTurn.forward;
        Quaternion startRot = cameraToTurn.rotation;

        Vector3 endPos = startPos + fwd * jumpForwardExtra;

        float baseFov = _cam ? _cam.fieldOfView : 60f;
        float targetFov = baseFov + fovKick;

        while (t < jumpDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / jumpDuration);

            float y = 4f * jumpUpHeight * u * (1f - u);
            cameraToTurn.position = Vector3.Lerp(startPos, endPos, u) + Vector3.up * y;

            float pitch = Mathf.Lerp(0f, cameraTiltDegrees, Mathf.SmoothStep(0f, 1f, u));
            float roll = Mathf.Lerp(0f, cameraTiltDegrees * 0.5f, Mathf.SmoothStep(0f, 1f, u));
            cameraToTurn.rotation = startRot * Quaternion.Euler(pitch, 0f, roll);

            if (_cam)
                _cam.fieldOfView = Mathf.Lerp(baseFov, targetFov, Mathf.Sin(u * Mathf.PI));

            yield return null;
        }

        cameraToTurn.rotation = Quaternion.Euler(0f, cameraToTurn.eulerAngles.y, 0f);

        if (_cam && fovRecoverTime > 0f)
        {
            float e = 0f;
            float startFov = _cam.fieldOfView;
            while (e < fovRecoverTime)
            {
                e += Time.deltaTime;
                float k = Mathf.Clamp01(e / fovRecoverTime);
                _cam.fieldOfView = Mathf.Lerp(startFov, baseFov, k);
                yield return null;
            }
            _cam.fieldOfView = baseFov;
        }
    }

    // Fail -> reset -> retry
    private System.Collections.IEnumerator FailResetAndRetry()
    {
        if (failResetDelay > 0f)
            yield return new WaitForSeconds(failResetDelay);

        if (_hasCamBaseline && cameraToTurn)
        {
            float t = 0f;
            Vector3 startPos = cameraToTurn.position;
            Quaternion startRotLocal = cameraToTurn.localRotation;
            Quaternion startRotWorld = cameraToTurn.rotation;
            float startFov = _cam ? _cam.fieldOfView : 60f;

            while (t < camResetDuration)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / camResetDuration);
                float s = u * u * (3f - 2f * u); // smoothstep

                cameraToTurn.position = Vector3.LerpUnclamped(startPos, _camBasePos, s);

                if (camTurnInLocalSpace)
                    cameraToTurn.localRotation = Quaternion.Slerp(startRotLocal, _camBaseRotLocal, s);
                else
                    cameraToTurn.rotation = Quaternion.Slerp(startRotWorld, _camBaseRotWorld, s);

                if (_cam)
                    _cam.fieldOfView = Mathf.Lerp(startFov, _camBaseFov, s);

                yield return null;
            }

            if (camTurnInLocalSpace)
                cameraToTurn.localRotation = _camBaseRotLocal;
            else
                cameraToTurn.rotation = _camBaseRotWorld;

            cameraToTurn.position = _camBasePos;
            if (_cam) _cam.fieldOfView = _camBaseFov;
        }

        SetAngle(Mathf.Clamp(startAjarAngle, closedAngle, maxOpenAngle));
        _failed = false;

        if (retryDelayAfterReset > 0f)
            yield return new WaitForSeconds(retryDelayAfterReset);

        StartQTE(attackDuration);
        _sequenceCo = null;
    }
}