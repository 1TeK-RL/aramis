using UnityEngine;

public class Gare : MonoBehaviour
{
    [SerializeField] private GameManagerv2 GameManager;
    [SerializeField] public int gareID;
    [SerializeField] public Transform StartingPosition;
    [SerializeField] public int StartingSpline;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Junction triggered");
        WagonController train = other.GetComponent<WagonController>();
        if (train != null)
        {
            GameManager.ArriverGare(gareID);
        }
    }
}
