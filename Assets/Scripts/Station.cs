using UnityEngine;

public class Station : MonoBehaviour
{
    [Header("Station Settings")]
    [SerializeField] private int id;
    [SerializeField] private int idSpline;

    private void OnTriggerEnter(Collider other)
    {
        WagonController wagon = other.GetComponent<WagonController>();
        if (wagon != null)
        {
            GameManager.Instance.ArrivedInStation(id);
        }
    }

    
    public int GetStationID()
    {
        return id;
    }

    public Vector3 GetDockPosition()
    {
        return transform.GetChild(0).position;
    }

    public int GetSplineID()
    {
        return idSpline;
    }
}
