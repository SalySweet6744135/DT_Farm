using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera;       // This is the default camera (named "Camera")
    public Camera mainCamera;   // This is the second camera (named "Main Camera")

    private bool usingDefaultCamera = true;

    void Start()
    {
        camera.enabled = true;
        mainCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            usingDefaultCamera = !usingDefaultCamera;

            camera.enabled = usingDefaultCamera;
            mainCamera.enabled = !usingDefaultCamera;
        }
    }
}
