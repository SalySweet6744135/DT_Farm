using UnityEngine;
using Unity.Cinemachine;

public class CameraSystemController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 90f; // Yaw (left/right)

    [Header("Vertical Rotation Settings")]
    [SerializeField] private Transform pitchPivot; // 🎯 Assign CameraPivot here
    [SerializeField] private float verticalRotationSpeed = 45f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;
    private float currentPitch = 0f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoomDistance = 2f;
    [SerializeField] private float maxZoomDistance = 20f;

    [Header("Camera Reference")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private CinemachineThirdPersonFollow followComponent;
    private float currentZoomDistance;

    void Start()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
            if (cinemachineCamera == null)
            {
                Debug.LogError("CameraSystemController: No CinemachineCamera found!");
                return;
            }
        }

        followComponent = cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
        if (followComponent == null)
        {
            Debug.LogError("CameraSystemController: Missing CinemachineThirdPersonFollow!");
            return;
        }

        if (pitchPivot == null)
        {
            Debug.LogError("CameraSystemController: Please assign the pitch pivot (CameraPivot child).");
            return;
        }

        currentZoomDistance = Mathf.Clamp(followComponent.CameraDistance, minZoomDistance, maxZoomDistance);
        currentPitch = pitchPivot.localEulerAngles.x;
    }

    void Update()
    {
        HandleMovement();
        HandleYawRotation();       // Y axis
        HandlePitchRotation();     // X axis (visual only)
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;

        move.y = 0f; // 🛑 Prevent movement from being affected by camera tilt

        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleYawRotation()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.Q)) input = -1f;
        if (Input.GetKey(KeyCode.E)) input = 1f;

        if (input != 0f)
        {
            transform.Rotate(0f, input * rotationSpeed * Time.deltaTime, 0f);
        }
    }

    private void HandlePitchRotation()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.F)) input = 1f;  // Look down
        if (Input.GetKey(KeyCode.R)) input = -1f; // Look up

        if (input != 0f)
        {
            currentPitch += input * verticalRotationSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            pitchPivot.localEulerAngles = new Vector3(currentPitch, 0f, 0f);
        }
    }

    private void HandleZoom()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) input = -1f;
        if (Input.GetKey(KeyCode.DownArrow)) input = 1f;

        if (input != 0f)
        {
            currentZoomDistance += input * zoomSpeed * Time.deltaTime;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);
            followComponent.CameraDistance = currentZoomDistance;
        }
    }
}
