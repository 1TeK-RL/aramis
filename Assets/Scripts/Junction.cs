using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Junction : MonoBehaviour
{
    [SerializeField] private List<int> idSplines;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<WagonController>().HitJunction(idSplines);
        }
    }
}
