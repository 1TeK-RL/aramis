using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using NUnit.Framework;
using System.Collections.Generic;

public class TrainController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody rb;
    public Slider speedSlider;
    public SplineContainer splineContainer;
    [SerializeField] private Transform startPosition;
    [SerializeField] private TMPro.TextMeshProUGUI DirectionButtonText;

    private Spline currentSpline;

    [Header("Paramètres du train")]
    public float maxSpeed = 20f;
    public float acceleration = 10f;

    private float currentSpeed = 0f;
    private float distanceOnSpline = 0f;

    private bool IsLeft;

    private void Start()
    {
        IsLeft = true;
        DirectionButtonText.text = "Left";
        currentSpline = splineContainer.Splines[0];
    }

    public void HitJunction(List<int> rails)
    {
        Vector3 currentWorldPos = rb.position;

        // Récupère la tangente actuelle sur l'ancienne spline
        float tOld = distanceOnSpline / currentSpline.GetLength();
        Vector3 oldTangent = splineContainer.transform.TransformDirection(currentSpline.EvaluateTangent(tOld)).normalized;

        // Choisit la nouvelle spline
        int targetIndex = IsLeft ? rails[0] : rails[1];
        if (splineContainer.Splines[targetIndex] == currentSpline)
        {
            Debug.Log("Already on the target spline, no switch needed.");
            return;
        }
        else {
            currentSpline = splineContainer.Splines[targetIndex];

            // Trouve le t le plus proche sur la nouvelle spline
            float bestT = 0f;
            float bestDist = float.MaxValue;
            const int samples = 50;

            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples;
                Vector3 sampleWorldPos = splineContainer.transform.TransformPoint(currentSpline.EvaluatePosition(t));
                float dist = Vector3.SqrMagnitude(sampleWorldPos - currentWorldPos);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestT = t;
                }
            }

            // Détermine la tangente sur la nouvelle spline à ce point
            Vector3 newTangent = splineContainer.transform.TransformDirection(currentSpline.EvaluateTangent(bestT)).normalized;

            // Compare les directions
            float dot = Vector3.Dot(oldTangent, newTangent);

            // Si le train est orienté à l'envers sur la nouvelle spline → inverser la direction
            if (dot < 0f)
            {
                // Inverser le t (si la spline est parcourue dans le sens inverse)
                bestT = 1f - bestT;
                // Optionnel : inverser la vitesse pour éviter un petit à-coup
                // currentSpeed *= -1f;
            }

            // Met à jour la distance correspondante
            float splineLength = currentSpline.GetLength();
            distanceOnSpline = bestT * splineLength;

            Debug.Log($"Switched to spline {targetIndex} at t={bestT:F2}, direction {(dot < 0 ? "reversed" : "normal")}");
        }
    }


    void FixedUpdate()
    {
        // 1. Vitesse cible
        float targetSpeed = speedSlider.value * maxSpeed;

        // 2. Ajustement de la vitesse actuelle
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // 3. Avancer sur la spline
        distanceOnSpline += currentSpeed * Time.fixedDeltaTime;

        // 4. Boucler si nécessaire
        Spline spline = currentSpline;
        float splineLength = spline.GetLength();
        if (distanceOnSpline > splineLength)
            distanceOnSpline -= splineLength;

        // 5. Normaliser la distance pour obtenir t [0,1]
        float t = distanceOnSpline / splineLength;

        // 6. Position et rotation
        Vector3 localPos = spline.EvaluatePosition(t);
        Vector3 newPos = splineContainer.transform.TransformPoint(localPos);

        // Pour la rotation, on récupère le tangente et on normalise correctement
        Vector3 tangent = spline.EvaluateTangent(t);
        if (tangent != Vector3.zero)
            tangent.Normalize();

        Quaternion newRot = Quaternion.LookRotation(tangent);

        // 7. Appliquer au Rigidbody
        rb.MovePosition(newPos);
        rb.MoveRotation(newRot);
    }


    public void ChangeDirection()
    {
        if(IsLeft)
        {
            IsLeft = false;
            DirectionButtonText.text = "Right";
        }
        else
        {
            IsLeft = true;
            DirectionButtonText.text = "Left";
        }
    }
}
