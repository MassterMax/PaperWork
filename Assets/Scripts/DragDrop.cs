using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    Vector2 startPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = startPos;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
}
