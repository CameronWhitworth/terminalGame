using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuideBook : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform targetTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isMoving = false;
    private bool isAtTarget = false;
    private bool isHovering = false;
    public float moveSpeed = 5f;
    public float hoverSpeed = 5f;
    public float rotationSpeed = 5f;
    public float hoverHeight = 0.2f; // Height the book will hover above its original position.
    private Coroutine hoverCoroutine; // Keep track of the hover coroutine.

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isMoving)
        {
            isAtTarget = !isAtTarget;
            isHovering = false; // Ensure we stop hovering when the book is picked up.
            if (hoverCoroutine != null) // If there's an ongoing hover coroutine, stop it.
            {
                StopCoroutine(hoverCoroutine);
            }
            StartCoroutine(MoveAndRotateBook(isAtTarget ? targetTransform.position : originalPosition,
                                             isAtTarget ? targetTransform.rotation : originalRotation));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAtTarget && !isMoving)
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
        if (!isAtTarget && isHovering)
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
