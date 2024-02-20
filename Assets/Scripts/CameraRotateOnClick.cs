using UnityEngine;

public class CameraRotateOnClick : MonoBehaviour
{
    public Camera cameraToRotate; // Assign your camera in the inspector
    public float rotationSpeed = 1.0f; // Speed of the rotation, adjust as necessary

    private void Update()
    {
        // Check if the mouse button is pressed
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            // Get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            // Calculate the screen thresholds
            float leftThreshold = Screen.width * 0.1f;
            float rightThreshold = Screen.width * 0.9f;

            // Rotate camera left if mouse click is within the left 10% of the screen
            if (mousePosition.x < leftThreshold)
            {
                StartCoroutine(RotateCamera(Vector3.up, -90)); // Rotate left
            }
            // Rotate camera right if mouse click is within the right 10% of the screen
            else if (mousePosition.x > rightThreshold)
            {
                StartCoroutine(RotateCamera(Vector3.up, 90)); // Rotate right
            }
        }
    }

    private System.Collections.IEnumerator RotateCamera(Vector3 axis, float angle)
    {
        Quaternion originalRotation = cameraToRotate.transform.rotation;
        Quaternion targetRotation = cameraToRotate.transform.rotation * Quaternion.Euler(axis * angle);
        float time = 0.0f;

        while (time < 1.0f)
        {
            cameraToRotate.transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, time);
            time += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        // Ensure the rotation completes
        cameraToRotate.transform.rotation = targetRotation;
    }
}
