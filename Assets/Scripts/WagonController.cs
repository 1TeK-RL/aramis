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

    private WagonPathing recordingData;
    private float elapsedTime = 0f;
    private bool IsRecording = false;

    private float currentSpeed = 0f;
    private float distanceOnSpline = 0f;

    private bool isPlaced;
    private bool isLeft;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        textDirection.text = "Left";
        isPlaced = false;
        isLeft = true;
    }

    public void StartGameplay(Station startStation)
    {
        currentSpline = splineContainer.Splines[startStation.GetSplineID()];
        SetPosition(startStation.GetDockPosition());
    }

    private void SetPosition(Vector3 startPos)
    {
        if (currentSpline == null || splineContainer == null) return;

        speedSlider.value = 0f;

        Vector3 startWorldPos = startPos;
        rb.position = startWorldPos;

        Vector3 localPos = splineContainer.transform.InverseTransformPoint(startWorldPos);

        SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

        Vector3 newWorldPos = splineContainer.transform.TransformPoint(nearestPoint);

        Vector3 worldTangent = splineContainer.transform.TransformDirection(currentSpline.EvaluateTangent(nearestT)).normalized;
        Quaternion newRot = Quaternion.LookRotation(worldTangent, Vector3.up);

        rb.position = newWorldPos;
        rb.rotation = newRot;

        distanceOnSpline = nearestT * currentSpline.GetLength();

        isPlaced = true;
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

            if (recordingData != null && IsRecording == true)
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
        IsRecording = true;
    }

    public WagonPathing StopRecording()
    {
        IsRecording = false;

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        isPlaced = false;

        textDirection.text = "Left";
        isLeft = true;

        return recordingData;
    }
}
