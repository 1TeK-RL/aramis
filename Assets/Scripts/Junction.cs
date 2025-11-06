using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public class Junction : MonoBehaviour
{

    [SerializeField] private SplineContainer rail;

    [Tooltip("List of indices representing positions in the list of splines in the spline container that " +
             "are available  to switch to from this junction")]
    [SerializeField] private List<int> rails;

    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Junction triggered");
        WagonController train = other.GetComponent<WagonController>();
        if (train != null)
        {
            train.HitJunction(rails);
        }
    }
}
