using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using Unity.Mathematics;

public class WagonController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody rb;
    public Slider speedSlider;
    public SplineContainer splineContainer;
    [SerializeField] private Transform startPosition;
    [SerializeField] private TMPro.TextMeshProUGUI DirectionButtonText;
    [SerializeField] private GameObject trainObject;

    private Spline currentSpline;
    public int startingSplineID;

    [Header("Paramètres du train")]
    [SerializeField] public Station startingGare;
    public float maxSpeed = 20f;
    public float acceleration = 10f;

    private float currentSpeed = 0f;
    private float distanceOnSpline = 0f;

    private bool IsLeft;
    private bool IsPlaced;

    private void Start()
    {
        IsPlaced = false;
        trainObject.SetActive(false);
        IsLeft = true;
        DirectionButtonText.text = "Left";
        currentSpline = splineContainer.Splines[0];
    }

    public void StartGame()
    {
        currentSpline = splineContainer.Splines[startingSplineID];
        Debug.Log("startingSplineID: " + startingSplineID);
        startPosition = startingGare.StartingPosition;

        SetPosition();
    }



    private void SetPosition()
    {
        if (currentSpline == null || splineContainer == null)
        {
            Debug.LogError("Spline ou SplineContainer non défini !");
            return;
        }

        // 1. Déplace le train directement à la gare
        trainObject.SetActive(true);
        Vector3 startWorldPos = startPosition.position;
        rb.position = startWorldPos;

        // 2. Convertit en espace local de la splineContainer
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(startWorldPos);

        // 3. Trouve le point le plus proche sur la spline
        SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

        // 4. Convertit la position locale en monde pour être parfaitement sur la spline
        Vector3 newWorldPos = splineContainer.transform.TransformPoint(nearestPoint);

        // 5. Calcule la tangente et la rotation
        Vector3 worldTangent = splineContainer.transform.TransformDirection(currentSpline.EvaluateTangent(nearestT)).normalized;
        Quaternion newRot = Quaternion.LookRotation(worldTangent, Vector3.up);

        // 6. Applique la position et la rotation sur le Rigidbody
        rb.position = newWorldPos;
        rb.rotation = newRot;

        // 7. Stocke la distance sur la spline
        distanceOnSpline = nearestT * currentSpline.GetLength();


        IsPlaced = true;

        //Debug.Log($"Train placé à la gare '{startingGare.name}' (t = {nearestT:F3})");
    }






    public void HitJunction(List<int> rails)
    {

        int targetIndex = IsLeft ? rails[0] : rails[1];
        if (splineContainer.Splines[targetIndex] == currentSpline)
        {
            Debug.Log("Already on the target spline, no switch needed.");
            return;
        }
        else
        {
            currentSpline = splineContainer.Splines[targetIndex];


        }
    }


    private void FixedUpdate()
    {
        if (IsPlaced)
        {
            // 1. Vérifie les références
            if (currentSpline == null || splineContainer == null || rb == null)
                return;

            // 2. Calcule la vitesse cible selon le slider
            float targetSpeed = speedSlider.value * maxSpeed;

            // 3. Fait une transition douce vers la vitesse cible
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

            // 4. Récupère la position actuelle du train en espace local
            Vector3 worldPos = rb.position;
            Vector3 localPos = splineContainer.transform.InverseTransformPoint(worldPos);

            // 5. Trouve le point le plus proche sur la spline
            SplineUtility.GetNearestPoint(currentSpline, localPos, out float3 nearestPoint, out float nearestT);

            // 6. Avance le long de la spline selon la vitesse
            float splineLength = currentSpline.GetLength();
            distanceOnSpline = nearestT * splineLength; // met à jour la position actuelle sur la spline
            distanceOnSpline += currentSpeed * Time.fixedDeltaTime; // avance selon la vitesse

            // 7. Clamp ou boucle la distance
            if (distanceOnSpline > splineLength)
            {
                distanceOnSpline %= splineLength; // Boucle automatiquement
            }

            // 8. Convertit la distance en facteur local (de 0 à 1)
            float normalizedDistance = distanceOnSpline / splineLength;

            // 9. Évalue la nouvelle position et la tangente
            float3 newLocalPos = currentSpline.EvaluatePosition(normalizedDistance);
            float3 localTangent = currentSpline.EvaluateTangent(normalizedDistance);

            // 10. Convertit en espace monde
            Vector3 newWorldPos = splineContainer.transform.TransformPoint(newLocalPos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent).normalized;

            // 11. Calcule la rotation orientée selon la tangente
            Quaternion newRot = Quaternion.LookRotation(worldTangent, Vector3.up);

            // 12. Applique la position et la rotation via Rigidbody
            rb.MovePosition(newWorldPos);
            rb.MoveRotation(newRot);
        }
    }






    public void ChangeDirection()
    {
        if (IsLeft)
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
