using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Junction : MonoBehaviour
{
    [SerializeField] private List<int> idSplines;

    private void OnTriggerEnter(Collider other)
    {
        WagonController wagon = other.GetComponent<WagonController>();
        if (wagon != null)
        {
            wagon.HitJunction(idSplines);
        }
    }
}
