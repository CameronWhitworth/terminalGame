using UnityEngine;

public class CameraRotateOnClick : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Speed of rotation
    private bool isRotating = false; // Flag to check if the camera is currently rotating
    private bool lookingUp = false; // Flag to check if the camera is currently rotating
    private Quaternion targetRotation; // Target rotation
    private Quaternion targetRotationOvershoot; // Target rotation for overshooting

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
            }
        }
        else
        {
            Vector3 mousePosition = Input.mousePosition;
            // Only allow up/down rotation if looking straight (or nearly straight) forward
            if (Mathf.Abs(transform.eulerAngles.y) < 1.0f || Mathf.Abs(transform.eulerAngles.y - 360) < 1.0f)
            {
                if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
                {
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
                }
            }
            
            // Left or right rotation
            if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
            {
                if (mousePosition.x <= Screen.width * 0.1f && !lookingUp)
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
    }

    void StartRotation(float angle)
    {
        if (!isRotating)
        {
            float overshot = angle * 1.01f;
            targetRotation = transform.rotation * Quaternion.Euler(0, angle, 0);
            targetRotationOvershoot = transform.rotation * Quaternion.Euler(0, overshot, 0);
            isRotating = true;
        }
    }

    void StartTiltRotation(float angle)
    {
        if (!isRotating)
        {
            float overshot = angle * 1.01f;
            targetRotation = transform.rotation * Quaternion.Euler(angle, 0, 0);
            targetRotationOvershoot = transform.rotation * Quaternion.Euler(overshot, 0, 0);
            isRotating = true;
        }
    }
}
