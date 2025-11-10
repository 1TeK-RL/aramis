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

    [Header("Settings")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 10f;

    private Spline currentSpline;
    private Rigidbody rb;
    private BoxCollider boxCollider;

    private WagonPathing recordingData;
    private float elapsedTime = 0f;
    private bool isRecording = false;

    private float currentSpeed = 0f;
    private float distanceOnSpline = 0f;

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
        distanceOnSpline = 0f;

        Vector3 startWorldPos = startPos;

        transform.position = startWorldPos;

        Vector3 localPos = splineContainer.transform.InverseTransformPoint(startWorldPos);

        SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

        Vector3 newWorldPos = splineContainer.transform.TransformPoint(nearestPoint);

        Vector3 worldTangent = splineContainer.transform.TransformDirection(currentSpline.EvaluateTangent(nearestT)).normalized;
        Quaternion newRot = Quaternion.LookRotation(worldTangent, Vector3.up);

        transform.SetPositionAndRotation(newWorldPos, newRot);

        distanceOnSpline = nearestT * currentSpline.GetLength();
    }

    private void FixedUpdate()
    {
        if (isPlaced)
        {
            if (currentSpline == null || splineContainer == null || rb == null) return;

            float targetSpeed = speedSlider.value * maxSpeed;

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

            Vector3 worldPos = rb.position;
            Vector3 localPos = splineContainer.transform.InverseTransformPoint(worldPos);

            SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

            float splineLength = currentSpline.GetLength();

            distanceOnSpline = nearestT * splineLength;
            distanceOnSpline += currentSpeed * Time.fixedDeltaTime;

            if (distanceOnSpline > splineLength)
            {
                distanceOnSpline %= splineLength;
            }

            float normalizedDistance = distanceOnSpline / splineLength;

            float3 newLocalPos = currentSpline.EvaluatePosition(normalizedDistance);
            float3 localTangent = currentSpline.EvaluateTangent(normalizedDistance);

            Vector3 newWorldPos = splineContainer.transform.TransformPoint(newLocalPos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent).normalized;

            Quaternion newRot = Quaternion.LookRotation(worldTangent, Vector3.up);

            rb.MovePosition(newWorldPos);
            rb.MoveRotation(newRot);

            
            if (recordingData != null && isRecording == true)
            {
                elapsedTime += Time.fixedDeltaTime;

                recordingData.paths.Add(new WagonPathing.PathPoint
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    elapsedTime = elapsedTime
                });
            }
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
        recordingData = new WagonPathing();
        elapsedTime = 0;
        isRecording = true;
    }

    public WagonPathing StopRecording()
    {
        boxCollider.enabled = false;
        gameObject.SetActive(false);

        isRecording = false;
        isPlaced = false;

        textDirection.text = "Left";
        isLeft = true;

        return recordingData;
    }
}
