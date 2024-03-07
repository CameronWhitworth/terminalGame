using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class GuideBook : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform targetTransform;
    private Vector3 originalPosition;
    private Quaternion targetRotation;
    private Quaternion originalRotation;
    private Quaternion currentTargetRotation;
    private bool isMoving = false;
    private bool isAtTarget = false;
    private bool isHovering = false;
    private bool isRotatingBack = false;
    public float moveSpeed = 5f;
    public float hoverSpeed = 5f;
    public float rotationSpeed = 5f;
    public float returnRotationSpeed = 100f;
    public float hoverHeight = 0.2f;
    private Coroutine hoverCoroutine;
    private Coroutine rotateBackCoroutine;
    public DialogueRunner dialogueRunner;
    private InMemoryVariableStorage variableStorage;

    private bool isRotatingWithLeftMouse = false;
    private float leftMousePressTime = 0;
    private const float holdThreshold = 0.1f; // Threshold in seconds to differentiate between click and hold

    // Static variable to track the currently picked up item
    private static GuideBook currentlyPickedUpItem = null;

    void Start()
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        targetRotation = targetTransform.rotation;
        currentTargetRotation = targetRotation;
    }

    void Update()
    {
        HandleLeftMouseRotation();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isMoving && !isRotatingBack && !isRotatingWithLeftMouse)
        {
            // Automatically put down the current item if a different one is clicked
            if (currentlyPickedUpItem != null && currentlyPickedUpItem != this)
            {
                currentlyPickedUpItem.PutDownItem();
            }

            ToggleItemState();

            if (isAtTarget && WaitForBookPickedUpValue())
            {
                dialogueRunner.StartDialogue("WellDone");
            }
        }
    }

    private void ToggleItemState()
    {
        isAtTarget = !isAtTarget;
        isHovering = false;
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }
        StartCoroutine(MoveAndRotateBook(isAtTarget ? targetTransform.position : originalPosition,
                                         isAtTarget ? targetRotation : originalRotation));

        // Update the reference to the currently picked up item
        currentlyPickedUpItem = isAtTarget ? this : null;
    }

    public void PutDownItem()
    {
        if (isAtTarget)
        {
            // Trigger the logic to move and rotate the book back to its original position
            StartCoroutine(MoveAndRotateBook(originalPosition, originalRotation));
            isAtTarget = false;
        }
    }

    IEnumerator RotateBackToTargetRotation()
    {
        isRotatingBack = true;

        while (Quaternion.Angle(transform.rotation, currentTargetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, currentTargetRotation, returnRotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = currentTargetRotation; // Ensure the rotation is exactly the target rotation at the end.
        isRotatingBack = false;
    }

    bool WaitForBookPickedUpValue()
    {
        variableStorage.TryGetValue("$waitForBookPickedUp", out bool x);
        return x; // Simplified return statement.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAtTarget && !isMoving && !isRotatingBack)
        {
            isHovering = true;
            if (hoverCoroutine != null) // If there's an ongoing hover coroutine, stop it.
            {
                StopCoroutine(hoverCoroutine);
            }
            hoverCoroutine = StartCoroutine(HoverBook(true));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAtTarget && isHovering && !isRotatingBack)
        {
            isHovering = false;
            if (hoverCoroutine != null) // If there's an ongoing hover coroutine, stop it.
            {
                StopCoroutine(hoverCoroutine);
            }
            hoverCoroutine = StartCoroutine(HoverBook(false));
        }
    }

    IEnumerator MoveAndRotateBook(Vector3 newPosition, Quaternion newRotation)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, newPosition) > 0.01f ||
               Quaternion.Angle(transform.rotation, newRotation) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = newPosition;
        transform.rotation = newRotation;

        isMoving = false;
    }

    IEnumerator HoverBook(bool isHoveringUp)
    {
        Vector3 endPosition = isHoveringUp ? originalPosition + Vector3.up * hoverHeight : originalPosition;
        while (isHovering == isHoveringUp && !isAtTarget && Vector3.Distance(transform.position, endPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, hoverSpeed * Time.deltaTime);
            yield return null;
        }
        // Snap to the end position in case of small discrepancies.
        if (!isAtTarget)
        {
            transform.position = endPosition;
        }
    }

     private void HandleLeftMouseRotation()
    {
        if (isAtTarget && Input.GetMouseButtonDown(0) && !isRotatingBack && !isMoving)
        {
            leftMousePressTime = Time.time; // Start timing the press duration
        }

        if (isAtTarget && Input.GetMouseButton(0) && !isRotatingBack && !isMoving)
        {
            if (Time.time - leftMousePressTime > holdThreshold)
            {
                // It's a hold, enable rotation
                isRotatingWithLeftMouse = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                float mouseX = Input.GetAxis("Mouse X") * (rotationSpeed * 0.35f);
                float mouseY = -Input.GetAxis("Mouse Y") * (rotationSpeed * 0.35f);
                transform.Rotate(Vector3.up, mouseX, Space.World);
                transform.Rotate(Vector3.right, mouseY, Space.World);
            }
        }

        if (isAtTarget && Input.GetMouseButtonUp(0) && isRotatingWithLeftMouse)
        {
            // End rotation
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isRotatingWithLeftMouse = false;
            if (rotateBackCoroutine != null)
            {
                StopCoroutine(rotateBackCoroutine);
            }
            rotateBackCoroutine = StartCoroutine(RotateBackToTargetRotation());
        }
    }
}
