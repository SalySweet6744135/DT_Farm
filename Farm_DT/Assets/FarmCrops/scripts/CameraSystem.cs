using UnityEngine;
using Unity.Cinemachine;

namespace CodeMonkey.CameraSystem
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private Transform followTarget;

        [Header("Move Settings")]
        [SerializeField] private float moveSpeed = 20f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotateSpeed = 100f;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float zoomMinY = 10f;
        [SerializeField] private float zoomMaxY = 50f;

        private Vector3 targetOffset;

        private void Awake()
        {
            if (followTarget == null)
            {
                Debug.LogError("Follow Target is not assigned!");
                enabled = false;
                return;
            }

            targetOffset = followTarget.localPosition;
        }

        private void Update()
        {
            HandleMove();
            HandleRotate();
            HandleZoom();
        }

        private void HandleMove()
        {
            Vector3 input = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) input.z += 1f;
            if (Input.GetKey(KeyCode.S)) input.z -= 1f;
            if (Input.GetKey(KeyCode.A)) input.x -= 1f;
            if (Input.GetKey(KeyCode.D)) input.x += 1f;

            Vector3 moveDir = transform.forward * input.z + transform.right * input.x;
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }

        private void HandleRotate()
        {
            float rotation = 0f;

            if (Input.GetKey(KeyCode.Q)) rotation += 1f;
            if (Input.GetKey(KeyCode.E)) rotation -= 1f;

            transform.eulerAngles += new Vector3(0f, rotation * rotateSpeed * Time.deltaTime, 0f);
        }

        private void HandleZoom()
        {
            float zoomChange = 0f;

            if (Input.GetKey(KeyCode.UpArrow)) zoomChange -= zoomSpeed;
            if (Input.GetKey(KeyCode.DownArrow)) zoomChange += zoomSpeed;

            targetOffset.y += zoomChange * Time.deltaTime;
            targetOffset.y = Mathf.Clamp(targetOffset.y, zoomMinY, zoomMaxY);

            followTarget.localPosition = Vector3.Lerp(followTarget.localPosition, targetOffset, Time.deltaTime * 10f);
        }
    }
}
