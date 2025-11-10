using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleObjectRotator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField, Range(0, 1)] private float rotationSpeed = 0.3f;
    [SerializeField] private Transform target;

    private bool isDragging;
    private Vector2 lastPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || target == null) return;

        Vector2 cur = eventData.position;
        Vector2 delta = cur - lastPos;
        lastPos = cur;

        target.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.Self);
    }
}
