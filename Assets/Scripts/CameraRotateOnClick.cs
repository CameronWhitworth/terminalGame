using UnityEngine;

public class CameraRotateOnClick : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Speed of rotation
    private bool isRotating = false; // Flag to check if the camera is currently rotating
    private bool lookingUp = false; // Flag to check if the camera is looking up
    private Quaternion targetRotation; // Target rotation
    private Quaternion targetRotationOvershoot; // Target rotation for overshooting
    private Quaternion originalRotation; // Original rotation before sway

    // Zoom variables
    public float zoomSpeed = 5f; // Speed of zooming in and out
    public float zoomedFOV = 30f; // FOV when zoomed in
    private float originalFOV; // Original FOV before zooming
    private Camera cameraComponent; // Camera component

    void Start()
    {
        // Initialize originalRotation at start
        originalRotation = transform.rotation;
        cameraComponent = GetComponent<Camera>(); // Get the Camera component
        originalFOV = cameraComponent.fieldOfView; // Store the original FOV

        
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate smoothly to the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationOvershoot, Time.deltaTime * rotationSpeed);

            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.0001f)
            {
                // If very close, complete the rotation to ensure it ends
                transform.rotation = targetRotation;
                isRotating = false;
                originalRotation = transform.rotation; // Update originalRotation after rotation completes
            }
        }
        else
        {
            SwayCamera(); // Allow camera to sway when not rotating
            CheckForRotationInput();
            CheckForZoomInput(); // Check for zoom input
        }
    }

    // New method for handling zoom input
    void CheckForZoomInput()
    {
        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
        }
    }

    void CheckForRotationInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        // Check for rotation input
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            // Determine rotation based on mouse position
            if (mousePosition.y >= Screen.height * 0.9f && !lookingUp)
            {
                lookingUp = true;
                StartTiltRotation(-20);
            }
            else if (mousePosition.y <= Screen.height * 0.1f && lookingUp)
            {
                lookingUp = false;
                StartTiltRotation(20);
            }
            else if (mousePosition.x <= Screen.width * 0.1f && !lookingUp)
            {
                // Left side clicked
                StartRotation(-90);
            }
            else if (mousePosition.x >= Screen.width * 0.9f && !lookingUp)
            {
                // Right side clicked
                StartRotation(90);
            }
        }
    }

    void StartRotation(float angle)
    {
        if (!isRotating)
        {
            float overshot = angle * 1.01f;
            targetRotation = originalRotation * Quaternion.Euler(0, angle, 0);
            targetRotationOvershoot = originalRotation * Quaternion.Euler(0, overshot, 0);
            isRotating = true;
        }
    }

    void StartTiltRotation(float angle)
    {
        if (!isRotating)
        {
            float overshot = angle * 1.01f;
            targetRotation = originalRotation * Quaternion.Euler(angle, 0, 0);
            targetRotationOvershoot = originalRotation * Quaternion.Euler(overshot, 0, 0);
            isRotating = true;
        }
    }

    void SwayCamera()
    {
        if (!isRotating) // Only sway if not already rotating
        {
            Vector3 mousePosition = Input.mousePosition;
            float swayAmount = 2.0f; // Adjust sway amount here
            Quaternion swayRotation = Quaternion.identity;

            if (mousePosition.x <= Screen.width * 0.1f && !lookingUp)
            {
                // Sway left
                swayRotation = Quaternion.Euler(0, -swayAmount, 0);
            }
            else if (mousePosition.x >= Screen.width * 0.9f && !lookingUp)
            {
                // Sway right
                swayRotation = Quaternion.Euler(0, swayAmount, 0);
            }
            else if (mousePosition.y >= Screen.height * 0.9f && !lookingUp)
            {
                // Sway up
                swayRotation = Quaternion.Euler(-swayAmount, 0, 0);
            }
            else if (mousePosition.y <= Screen.height * 0.1f && lookingUp)
            {
                // Sway down
                swayRotation = Quaternion.Euler(swayAmount, 0, 0);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation * swayRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
