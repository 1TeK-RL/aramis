using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class WagonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private TextMeshProUGUI textDirection;
    [SerializeField] private Transform wagonModelTransform;

    [Header("Settings")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 10f;

    private Spline currentSpline;
    private Rigidbody rb;
    private BoxCollider boxCollider;

    private WagonData recordingData;
    private float elapsedTime = 0f;
    private bool isRecording = false;

    private float currentSpeed = 0f;

    private bool isPlaced;
    private bool isLeft;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        textDirection.text = "Left";
        isPlaced = false;
        isLeft = true;
        boxCollider.enabled = false;
    }

    public void ResetWagon()
    {
        isPlaced = false;
        boxCollider.enabled = false;
        recordingData = null;
        elapsedTime = 0f;
        isRecording = false;
        currentSpeed = 0f;
        speedSlider.value = 0f;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

    public void SetupGameplay(Station startStation)
    {
        currentSpline = splineContainer.Splines[startStation.GetSplineID()];
        SetPosition(startStation.GetDockPosition());
        
    }

    public void StartGameplay()
    {
        isPlaced = true;
        boxCollider.enabled = true;
    }

    private void SetPosition(Vector3 startPos)
    {
        if (currentSpline == null || splineContainer == null) return;

        speedSlider.value = 0f;
        currentSpeed = 0f;

        Vector3 startWorldPos = startPos;

        transform.position = startWorldPos;

        // Convertir en local par rapport au container
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(startWorldPos);

        // Trouver le point le plus proche sur la spline locale
        SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

        // Positionner sur le rail en world space
        Vector3 newWorldPos = splineContainer.transform.TransformPoint(nearestPoint);
        transform.position = newWorldPos;

        // Orientation selon la spline
        Vector3 forwardVector = splineContainer.transform.TransformDirection(Vector3.Normalize(currentSpline.EvaluateTangent(nearestT)));
        Vector3 upVector = splineContainer.transform.TransformDirection(currentSpline.EvaluateUpVector(nearestT));

        Quaternion railRotation = Quaternion.LookRotation(forwardVector, upVector);
        Quaternion axisRemap = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = railRotation * axisRemap;

        isPlaced = true;
    }

    private void FixedUpdate()
    {
        if (!isPlaced) return;

        float targetSpeed = speedSlider.value * maxSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        var native = new NativeSpline(currentSpline);

        // Récupération du point le plus proche sur la spline (en local)
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(native, localPos, out float3 nearest, out float t);

        // Position sur la spline (en world)
        Vector3 worldPos = splineContainer.transform.TransformPoint(nearest);
        transform.position = worldPos;

        // Orientation selon la spline (convertie en world)
        Vector3 forwardVector = splineContainer.transform.TransformDirection(Vector3.Normalize(native.EvaluateTangent(t)));
        Vector3 upVector = splineContainer.transform.TransformDirection(native.EvaluateUpVector(t));

        Quaternion railRotation = Quaternion.LookRotation(forwardVector, upVector);
        Quaternion axisRemap = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = railRotation * axisRemap;

        // Vitesse alignée à la tangente du rail
        rb.linearVelocity = forwardVector * currentSpeed;

        // Enregistrement du mouvement
        if (recordingData != null && isRecording)
        {
            elapsedTime += Time.fixedDeltaTime;
            recordingData.paths.Add(new WagonData.PathPoint
            {
                position = transform.position,
                rotation = transform.rotation,
                elapsedTime = elapsedTime
            });
        }
    }


    public void HitJunction(List<int> rails)
    {
        int targetIndex = isLeft ? rails[0] : rails[1];
        if (splineContainer.Splines[targetIndex] != currentSpline)
        {
            currentSpline = splineContainer.Splines[targetIndex];
        }
    }

    public void ChangeDirection()
    {
        if (isLeft)
        {
            isLeft = false;
            textDirection.text = "Right";
        }
        else
        {
            isLeft = true;
            textDirection.text = "Left";
        }
    }

    public void BeginRecording()
    {
        recordingData = new WagonData();
        elapsedTime = 0;
        isRecording = true;
    }

    public WagonData StopRecording()
    {
        boxCollider.enabled = false;

        isRecording = false;
        isPlaced = false;

        textDirection.text = "Left";
        isLeft = true;
        
        return recordingData;
    }

    
}
