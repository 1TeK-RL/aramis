using UnityEngine;

public class Station : MonoBehaviour
{
    
    [SerializeField] public int gareID;
    [SerializeField] public Transform StartingPosition;
    [SerializeField] public int StartingSpline;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Junction triggered");
        WagonController train = other.GetComponent<WagonController>();
        if (train != null)
        {
            GameManager.Instance.ArriverGare(gareID);
        }
    }
}
