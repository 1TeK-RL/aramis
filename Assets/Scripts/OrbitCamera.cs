using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public float distance = 5f;
    public float rotationSpeed = 120f;

    private float currentAngle = 0f;

    void Start()
    {
        if (target == null) return;

        // Place la caméra correctement au départ
        Vector3 dir = (transform.position - target.position).normalized;
        transform.position = target.position + dir * distance;
        transform.LookAt(target);
    }

    void Update()
    {
        // Clic droit maintenu
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");

            currentAngle += mouseX * rotationSpeed * Time.deltaTime;

            // Rotation autour de l'objet (axe Y)
            transform.RotateAround(
                target.position,
                Vector3.up,
                mouseX * rotationSpeed * Time.deltaTime
            );

            transform.LookAt(target);
        }
    }
}
