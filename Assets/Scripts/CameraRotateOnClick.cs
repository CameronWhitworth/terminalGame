using UnityEngine;

public class CameraRotateOnClick : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Speed of rotation
    private bool isRotating = false; // Flag to check if the camera is currently rotating
    private Quaternion targetRotation; // Target rotation
    private Quaternion targetRotationOvershoot; // Target rotation

    void Update()
    {
        if (isRotating)
        {
            // Rotate smoothly to the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationOvershoot, Time.deltaTime * rotationSpeed);
            Debug.Log(Time.deltaTime * rotationSpeed);
            
            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.0000001f) //Thsi causes a snap
            {
                // If very close, complete the rotation to ensure it ends
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
        else
        {
            // Check if the mouse is within 10% of the screen width from the left or right edge
            if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
            {
                Vector3 mousePosition = Input.mousePosition;
                if (mousePosition.x <= Screen.width * 0.1f)
                {
                    // Left side clicked
                    StartRotation(-90, -91);
                }
                else if (mousePosition.x >= Screen.width * 0.9f)
                {
                    // Right side clicked
                    StartRotation(90, 91);
                }
            }
        }
    }

    void StartRotation(float angle, float angleOvershoot)
    {
        if (!isRotating)
        {
            float overshot = angle * 1.01f;
            targetRotation = transform.rotation * Quaternion.Euler(0, angle, 0); // Calculate target rotation
            targetRotationOvershoot = transform.rotation * Quaternion.Euler(0, overshot, 0); // Calculate target rotation
            isRotating = true; // Set the flag to true
        }
    }
}
