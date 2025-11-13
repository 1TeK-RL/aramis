using System.Collections;
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
    [SerializeField] private Transform wagonModelTransform;
    [SerializeField] private GameObject BlueDirection;
    [SerializeField] private GameObject RedDirection;

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

        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            SplineUtility.ReverseFlow(splineContainer, i);
        }
    }

    private void Start()
    {
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
        transform.localPosition = Vector3.zero;
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
        isLeft = true;
        RedDirection.SetActive(false);
        BlueDirection.SetActive(true);
    }

    private void SetPosition(Vector3 startPos)
    {
        if (currentSpline == null || splineContainer == null) return;

        speedSlider.value = 0f;
        currentSpeed = 0f;

        Vector3 startWorldPos = startPos;

        transform.position = startWorldPos;

        Vector3 localPos = splineContainer.transform.InverseTransformPoint(startWorldPos);

        SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

        Vector3 newWorldPos = splineContainer.transform.TransformPoint(nearestPoint);
        transform.position = newWorldPos;

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

        Vector3 localPos = splineContainer.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(native, localPos, out float3 nearest, out float t);

        Vector3 worldPos = splineContainer.transform.TransformPoint(nearest);
        transform.position = worldPos;

        Vector3 forwardVector = splineContainer.transform.TransformDirection(Vector3.Normalize(native.EvaluateTangent(t)));
        Vector3 upVector = splineContainer.transform.TransformDirection(native.EvaluateUpVector(t));

        Quaternion railRotation = Quaternion.LookRotation(forwardVector, upVector);
        Quaternion axisRemap = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = railRotation * axisRemap;

        rb.linearVelocity = forwardVector * currentSpeed;

        if (recordingData != null && isRecording)
        {
            elapsedTime += Time.fixedDeltaTime;
            recordingData.paths.Add(new WagonData.PathPoint
            {
                position = transform.localPosition,
                rotation = transform.localRotation,
                elapsedTime = elapsedTime
            });
        }
    }

    public void StopAtStation()
    {
        StartCoroutine(SlowDownRoutine(0.5f));
    }

    private IEnumerator SlowDownRoutine(float duration)
    {
        float startSpeed = currentSpeed;
        float startSlider = speedSlider.value;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            currentSpeed = Mathf.Lerp(startSpeed, 0f, t);
            speedSlider.value = Mathf.Lerp(startSlider, 0f, t);

            yield return null;
        }

        currentSpeed = 0f;
        speedSlider.value = 0f;
        rb.linearVelocity = Vector3.zero;
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
            RedDirection.SetActive(true);
            BlueDirection.SetActive(false);
        }
        else
        {
            isLeft = true;
            RedDirection.SetActive(false);
            BlueDirection.SetActive(true);
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

        isLeft = true;

        return recordingData;
    }

    public void EnableOutline()
    {
        if (CustomizeManager.Instance.GetCurrentWagonIndex() == 0)
        {
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Outline>().enabled = true;
        }
        else
        {
            transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Outline>().enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (CustomizeManager.Instance.GetCurrentWagonIndex() == 0)
        {
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Outline>().enabled = false;
        }
        else
        {
            transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Outline>().enabled = false;
        }
    }
}
