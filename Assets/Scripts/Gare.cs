using UnityEngine;

public class Gare : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] public int gareID;
    [SerializeField] public Transform StartingPosition;
    [SerializeField] public int StartingSpline;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Junction triggered");
        TrainController train = other.GetComponent<TrainController>();
        if (train != null)
        {
            GameManager.ArriverGare(gareID);
        }
    }
}
