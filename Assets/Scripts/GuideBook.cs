using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class GuideBook : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform targetTransform;
    private Vector3 originalPosition;
    private Quaternion targetRotation; // Rotation of the book when it's at the target.
    private Quaternion originalRotation;
    private Quaternion currentTargetRotation; // To store the current target rotation (used for smooth rotation back).
    private bool isMoving = false;
    private bool isAtTarget = false;
    private bool isHovering = false;
    private bool isRotatingBack = false; // To check if the book is rotating back to its target rotation.
    public float moveSpeed = 5f;
    public float hoverSpeed = 5f;
    public float rotationSpeed = 5f;
    public float returnRotationSpeed = 100f; // Adjusted for constant speed rotation.
    public float hoverHeight = 0.2f; // Height the book will hover above its original position.
    private Coroutine hoverCoroutine; // Keep track of the hover coroutine.
    private Coroutine rotateBackCoroutine; // Keep track of the rotate back coroutine.
    public DialogueRunner dialogueRunner;
    private InMemoryVariableStorage variableStorage;

    void Start()
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        targetRotation = targetTransform.rotation; // Assume the target rotation is the initial rotation of the targetTransform.
        currentTargetRotation = targetRotation; // Initialize current target rotation.
    }

    void Update()
    {
        // Rotate the book with right mouse button held down
        if (isAtTarget && Input.GetMouseButton(1) && !isRotatingBack && !isMoving)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;
            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(Vector3.right, mouseY, Space.World);
        }

        // Start rotating back when right mouse button is released
        if (isAtTarget && Input.GetMouseButtonUp(1) && !isRotatingBack)
        {
            if (rotateBackCoroutine != null)
            {
                StopCoroutine(rotateBackCoroutine);
            }
            rotateBackCoroutine = StartCoroutine(RotateBackToTargetRotation());
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

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check for left mouse button click
        if (eventData.button == PointerEventData.InputButton.Left && !isMoving && !isRotatingBack)
        {
            isAtTarget = !isAtTarget;
            isHovering = false; // Ensure we stop hovering when the book is picked up.
            if (hoverCoroutine != null) // If there's an ongoing hover coroutine, stop it.
            {
                StopCoroutine(hoverCoroutine);
            }
            StartCoroutine(MoveAndRotateBook(isAtTarget ? targetTransform.position : originalPosition,
                                             isAtTarget ? targetRotation : originalRotation));

            if(isAtTarget && WaitForBookPickedUpValue())
            {
                dialogueRunner.StartDialogue("WellDone");
            }
        }
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
}
