using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropPaper : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // other objects
    Canvas canvas;
    RectTransform tableSpace;
    PaperSpawner paperSpawner;

    // components
    private RectTransform rectTransform;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        tableSpace = transform.parent.GetComponent<RectTransform>();
        paperSpawner = transform.parent.GetComponent<PaperSpawner>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");

        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

        // Get the allowed position inside the parent
        Vector2 clampedPosition = GetClampedPosition(newPosition);

        // Apply the clamped position
        rectTransform.anchoredPosition = clampedPosition;
    }

    /// <summary>
    /// Clamps the position to keep the child inside the parent.
    /// Allows movement only along an axis if diagonal movement would go out of bounds.
    /// </summary>
    private Vector2 GetClampedPosition(Vector2 targetPosition)
    {
        // Get the world corners of the parent
        Vector3[] parentCorners = new Vector3[4];
        tableSpace.GetWorldCorners(parentCorners);

        // Get the world corners of the child
        Vector3[] childCorners = new Vector3[4];
        rectTransform.GetWorldCorners(childCorners);

        // Convert world positions to local anchored positions
        Vector2 minParent = tableSpace.InverseTransformPoint(parentCorners[0]);
        Vector2 maxParent = tableSpace.InverseTransformPoint(parentCorners[2]);

        Vector2 minChild = tableSpace.InverseTransformPoint(childCorners[0]);
        Vector2 maxChild = tableSpace.InverseTransformPoint(childCorners[2]);

        // Compute the allowed position range
        float minX = minParent.x + (rectTransform.anchoredPosition.x - minChild.x);
        float maxX = maxParent.x - (maxChild.x - rectTransform.anchoredPosition.x);
        float minY = minParent.y + (rectTransform.anchoredPosition.y - minChild.y);
        float maxY = maxParent.y - (maxChild.y - rectTransform.anchoredPosition.y);

        // Check if the movement is outside the boundaries
        bool canMoveX = targetPosition.x >= minX && targetPosition.x <= maxX;
        bool canMoveY = targetPosition.y >= minY && targetPosition.y <= maxY;

        // If diagonal move is not possible, constrain movement to the allowed axis
        if (!canMoveX && !canMoveY)
        {
            return rectTransform.anchoredPosition; // No movement allowed
        }
        else if (!canMoveX)
        {
            return new Vector2(rectTransform.anchoredPosition.x, Mathf.Clamp(targetPosition.y, minY, maxY));
        }
        else if (!canMoveY)
        {
            return new Vector2(Mathf.Clamp(targetPosition.x, minX, maxX), rectTransform.anchoredPosition.y);
        }

        // If both movements are valid, allow the move
        return new Vector2(Mathf.Clamp(targetPosition.x, minX, maxX), Mathf.Clamp(targetPosition.y, minY, maxY));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            paperSpawner.OnPaperClick();
        }
    }
}
