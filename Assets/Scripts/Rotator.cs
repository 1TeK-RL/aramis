using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleObjectRotator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Transform target;            // L'objet à faire tourner
    public float rotationSpeed = 0.3f;  // Sensibilité de rotation

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
        if (!isDragging || target == null)
            return;

        Vector2 cur = eventData.position;
        Vector2 delta = cur - lastPos;
        lastPos = cur;

        // Rotation horizontale (autour de l'axe Y local)
        target.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.Self);
    }
}
